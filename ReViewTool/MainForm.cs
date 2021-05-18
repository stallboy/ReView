using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using ReView;
using System.Collections;
using System.IO;
using ReViewBinaryStorage;
using System.Net;
using Lemniscate;
using ReViewRPC;
using ReViewTool.Modules.HierarchicalTimelineLog;
using System.Net.Sockets;

namespace ReViewTool
{
	/// <summary>
	/// ReView main form
	/// </summary>
	public partial class MainForm : Form, IReView_Tool
	{
		public MainForm()
		{
			Log.AddConsoleWriter();

			this.Visible = false;
			InitializeComponent();

			// Show splash screen for one second (click on the splash screen to make it disappear immediately)
			SplashScreen splash = new SplashScreen(5000);
			splash.ShowDialog(this);

			// Register all debug modules
			RegisterDebugModules();

			// Load user preferences
			UserPreferencesManager.Instance.OnUserPreferencesChanged += OnUserPreferencesChanged;
			UserPreferencesManager.Instance.Load();

			timelineControl.OnPanOffsetChanged += OnTimelinePanOffsetChanged;
			timelineControl.OnPlaybackPositionChanged += OnTimelinePlaybackPositionChanged;
			timelineControl.OnZoomChanged += OnTimelineZoomChanged;
			timelineControl.PropertyChanged += OnTimelinePropertyChanged;

			DisableUIEvents = false;
			RPC_Manager.Instance.ConnectionStateChanged += RPC_Manager_OnConnectionStateChanged;

			SynchronizeButtons();
		}

		#region Event Listeners

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) 
		{
			if (keyData == (Keys.Right | Keys.Alt)) 
			{
				NextTimelineEvent();
				return true;
			}
			else if (keyData == (Keys.Left | Keys.Alt)) 
			{
				PrevTimelineEvent();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// Callback invoked when user preferences are changed -> Recreates buttons and updates colors to the logTracker component.
		/// </summary>
		private void OnUserPreferencesChanged(object sender)
		{
			CreateUserCommandButtons();

			SynchronizeButtons();
		}

		private void SendBackBinaryData(int time)
		{
			try
			{
				IReView_Feed proxy = RPC_Manager.Instance.Get_Client_Proxy<RPC_Client_Proxy_IReView_Feed>();
				if (proxy != null && BinaryStorage != null)
				{
					// Collect all data entries and create a package to send
					List<BinaryData> dataEntries = BinaryStorage.GetData(time);
					List<long> idList = new List<long>();
					List<int> timeList = new List<int>();
					List<byte[]> dataList = new List<byte[]>();
					foreach (BinaryData data in dataEntries)
					{
						idList.Add(data.Id);
						timeList.Add(data.Time);
						dataList.Add(data.Data);
					}
					proxy.SendBackBinaryData(idList.ToArray(), timeList.ToArray(), dataList.ToArray());
				}
			}
			catch (Exception e)
			{
				RPC_Manager.Instance.Close();
			}
		}

		private void OnTimelinePanOffsetChanged(TimelineControl sender, bool userAction)
		{
			DebugModuleManager.Instance.TimelinePanOffsetChanged(sender.PanOffset);
		}

		private void OnTimelinePlaybackPositionChanged(bool userChange)
		{
			int playbackPosition = timelineControl.Model.PlaybackPosition;

			CurrentDebugModule.OnTimelinePlaybackPositionChanged(playbackPosition);

			if (userChange)
			{
				SendBackBinaryData(playbackPosition);
			}
		}

		private void OnTimelineZoomChanged(TimelineControl sender)
		{
			DebugModuleManager.Instance.TimelineZoomChanged(sender.TimePixelRatio);
		}

		#endregion

		#region Debug Modules

		private void RegisterDebugModules()
		{
			// Register modules in order (buttons appear in this order from top -> down)
			DebugModuleManager.Instance.RegisterDebugModule(AddDebugModuleButton(new HierarhicalTimelineLog()), true);
			DebugModuleManager.Instance.RegisterDebugModule(AddDebugModuleButton(new RemoteDebugRenderer()));

			CurrentDebugModule = DebugModuleManager.Instance.GetDefaultModule();
		}

		private DebugModule AddDebugModuleButton(DebugModule module)
		{
			VerticalButton button = new VerticalButton();
			button.BackColor = System.Drawing.Color.White;
			button.FlatAppearance.BorderSize = 0;
			button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			button.ForeColor = System.Drawing.Color.Black;
			button.Location = new System.Drawing.Point(4, 0);
			button.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			button.Name = "timelineLogButton";
			button.Size = new System.Drawing.Size(24, 100);
			button.TabIndex = 1;
			button.UseVisualStyleBackColor = false;
			button.VerticalText = module.ModuleName;
			button.Click += new System.EventHandler(this.debugModuleButton_Click);

			this.componenFlowLayout.Controls.Add(button);

			debugModuleButtonMap.Add(button, module);

			return module;
		}

		private void debugModuleButton_Click(object sender, EventArgs e)
		{
			VerticalButton vb = sender as VerticalButton;
			DebugModule debugModule = debugModuleButtonMap[vb];

			CurrentDebugModule = debugModule;
		}

		private VerticalButton GetDebugModuleButton(DebugModule module)	
		{
			foreach (KeyValuePair<VerticalButton, DebugModule> pair in debugModuleButtonMap)
			{
				if (pair.Value == module)
					return pair.Key;
			}
			return null;
		}

		private void DebugModule_DurationChanged(int duration)
		{
			if (TimelineSession != null)
			{
				this.TimelineSession.UpdateDuration(duration);
			}
		}

		private DebugModule CurrentDebugModule
		{
			get
			{
				return currentDebugModule;
			}
			set
			{
				if (currentDebugModule == value)
				{
					return;
				}

				DebugModule previousDebugModule = currentDebugModule;

				if (previousDebugModule != null)
				{
					VerticalButton vb = GetDebugModuleButton(currentDebugModule);
					vb.BackColor = Color.White;
					vb.ForeColor = Color.Black;

					previousDebugModule.TimelineMarginChangeRequested -= DebugModule_OnTimelineMarginChangeRequested;
					previousDebugModule.TimePixelRatioChanged -= DebugModule_OnTimePixelRatioChanged;
					previousDebugModule.PanOffsetChanged -= DebugModule_OnPanOffsetChanged;
					previousDebugModule.DurationChanged -= DebugModule_DurationChanged;

					FlowLayoutPanel debugModuleToolbarLayout = previousDebugModule.GetToolbarButtonFlowLayout();
					if (debugModuleToolbarLayout != null)
					{
						debugModuleButtonContainer.Controls.Remove(debugModuleToolbarLayout);
					}
				}

				currentDebugModule = value;

				if (currentDebugModule != null)
				{
					VerticalButton vb = GetDebugModuleButton(currentDebugModule);
					vb.BackColor = Color.DarkGray;
					vb.ForeColor = Color.White;

					currentDebugModule.Dock = DockStyle.Fill;
					currentDebugModule.Margin = new Padding(0, 0, 0, 0);

					currentDebugModule.TimelineMarginChangeRequested += DebugModule_OnTimelineMarginChangeRequested;
					currentDebugModule.TimePixelRatioChanged += DebugModule_OnTimePixelRatioChanged;
					currentDebugModule.PanOffsetChanged += DebugModule_OnPanOffsetChanged;
					currentDebugModule.DurationChanged += DebugModule_DurationChanged;

					FlowLayoutPanel debugModuleToolbarLayout = currentDebugModule.GetToolbarButtonFlowLayout();
					if (debugModuleToolbarLayout != null)
					{
						debugModuleButtonContainer.Controls.Add(debugModuleToolbarLayout);
					}
				}

				mainTable.SuspendLayout();
				if (previousDebugModule != null)
				{
					previousDebugModule.OnDeactivateDebugModule();
					mainTable.Controls.Remove(previousDebugModule);
				}
				if (currentDebugModule != null)
				{
					mainTable.Controls.Add(currentDebugModule);
					currentDebugModule.OnActivateDebugModule();
				}
				mainTable.ResumeLayout();
			}
		}

		protected void DebugModule_OnTimePixelRatioChanged(float timePixelRatio)
		{
			timelineControl.ForceTimePixelRatio(timePixelRatio);
		}

		protected void DebugModule_OnPanOffsetChanged(int pixels, bool userChange)
		{
			if (userChange)
			{
				this.timelineControl.AutoPanToPlaybackHeader = false;
				this.timelineControl.AutoFollowTail = false;
			}
			timelineControl.ForcePanOffset(new Point(pixels, timelineControl.PanOffset.Y));
		}

		protected void DebugModule_OnTimelineMarginChangeRequested(int leftMargin, int rightMargin)
		{
			timelineControl.MinimumSize = new System.Drawing.Size(16, timelineControl.MinimumSize.Height);
			timelineControl.Margin = new Padding(leftMargin, 0, rightMargin, 0);
		}

		#endregion

		#region Form Management

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);

			StopServer();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (UserPreferencesManager.Instance.UserPreferences.ServerMode)
			{
				// Auto-start server if server mode enabled in user preferences
				StartServer();
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			StopServer();

			base.OnFormClosing(e);
		}

		#endregion

		#region Button event handlers

		private bool ignoreAutoFollowPlaybackHeaderChange = false;

		/// <summary>
		/// Someone clicks / checks auto-follow toggle
		/// </summary>
		private void toggleAutoFollow_CheckedChanged(object sender, EventArgs e)
		{
			ignoreAutoFollowPlaybackHeaderChange = true;

			this.timelineControl.AutoPanToPlaybackHeader = toggleAutoFollow.Checked;
			this.timelineControl.AutoFollowTail = toggleAutoFollow.Checked;

			ignoreAutoFollowPlaybackHeaderChange = false;

			toggleAutoFollow.Text = toggleAutoFollow.Checked ? "+ Auto follow" : "- Auto follow";
		}

		private void OnTimelinePropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "AutoFollowTail")
			{
				if (!ignoreAutoFollowPlaybackHeaderChange)
				{
					toggleAutoFollow.Checked = this.timelineControl.AutoFollowTail;
					toggleAutoFollow.Text = this.timelineControl.AutoFollowTail ? "+ Auto follow" : "- Auto follow";
				}
			}
		}

		/// <summary>
		/// Someone clicks any of the user action buttons
		/// </summary>
		private void userAction_Clicked(object sender, EventArgs e)
		{
			UserCommand command = null;
			bool isChecked = false;
			if (sender is Button)
			{
				Button button = (Button)sender;
				command = button.Tag as UserCommand;
				isChecked = true;
			}
			if (sender is CheckBox)
			{
				CheckBox cb = (CheckBox)sender;
				command = cb.Tag as UserCommand;
				isChecked = cb.Checked;
				cb.Text = cb.Checked ? "+ " + command.Name : "- " + command.Name;
			}

			if (command != null)
			{
				IReView_Feed proxy = RPC_Manager.Instance.Get_Client_Proxy<IReView_Feed>();
				if (proxy != null)
				{
					proxy.DebugToggleChanged(command.Command, isChecked);
				}
			}
		}

		/// <summary>
		/// Someone clicks 'Customize' button
		/// </summary>
		private void preferencesButton_Click(object sender, EventArgs e)
		{
			PreferencesDialog cd = new PreferencesDialog();
			if (cd.ShowDialog(this) == DialogResult.OK)
			{
				this.SuspendLayout();
				this.toolPanel.SuspendLayout();

				CreateUserCommandButtons();

				this.toolPanel.ResumeLayout();
				this.ResumeLayout();
			}
		}

		/// <summary>
		/// Someone clicks 'Connect' / 'Disconnect' button (same one changing text)
		/// </summary>
		private void connectButton_Click(object sender, EventArgs e)
		{
			if (Server != null)
			{
				StopServer();
			}
			else
			{
				ConnectDialog cd = new ConnectDialog();
				cd.ShowDialog(this);
				if (cd.OperationRequested == ConnectOperation.HostStorageServer)
				{
					StartServer();
				}
			}
		}

		private void loopToggle_CheckedChanged(object sender, EventArgs e)
		{
			if (!DisableUIEvents)
			{
				CheckBox cb = sender as CheckBox;

				UserPreferencesManager.Instance.UserPreferences.TimelinePlaybackLoop = cb.Checked;
				UserPreferencesManager.Instance.Save();
			}
		}

		private void timelineNavigationSnapButton_Click(object sender, EventArgs e)
		{
			if (!DisableUIEvents)
			{
				switch (UserPreferencesManager.Instance.UserPreferences.TimelineNavigationSnap)
				{
					case TimelineNavigationSnapMode.BinaryStorage:
						UserPreferencesManager.Instance.UserPreferences.TimelineNavigationSnap = TimelineNavigationSnapMode.DebugModule;
						break;
					case TimelineNavigationSnapMode.DebugModule:
						UserPreferencesManager.Instance.UserPreferences.TimelineNavigationSnap = TimelineNavigationSnapMode.BinaryStorage;
						break;
					default:
						break;
				}

				UserPreferencesManager.Instance.Save();
			}
		}

		private void playButton_Click(object sender, EventArgs e)
		{
			if (TimelineSession != null && TimelineSession.Duration > 0)
			{
				StartPlayback();
			}
		}

		private void timelineNextButton_Click(object sender, EventArgs e)
		{
			NextTimelineEvent();
		}

		private void timelinePrevButton_Click(object sender, EventArgs e)
		{
			PrevTimelineEvent();
		}

		private void SynchronizeButtons()
		{
			if (UserPreferencesManager.Instance.UserPreferences == null)
			{
				return;
			}

			DisableUIEvents = true;

			loopToggle.Checked = UserPreferencesManager.Instance.UserPreferences.TimelinePlaybackLoop;
			loopToggle.Text = loopToggle.Checked ? "+ Loop" : "- Loop";

			switch (UserPreferencesManager.Instance.UserPreferences.TimelineNavigationSnap)
			{
				case TimelineNavigationSnapMode.BinaryStorage:
					timelineNavigationSnapButton.Text = "Binary Storage";
					break;
				case TimelineNavigationSnapMode.DebugModule:
					timelineNavigationSnapButton.Text = "Debug Module";
					break;
				default:
					break;
			}

			DisableUIEvents = false;
		}

		private bool DisableUIEvents
		{
			get;
			set;
		}

		#endregion

		#region Playback

		private void StartPlayback()
		{
		}

		private void StopPlayback()
		{
		}

		private void NextTimelineEvent()
		{
			int setTime = -1;
			if (TimelineSession != null && TimelineSession.Duration > 0)
			{
				switch (UserPreferencesManager.Instance.UserPreferences.TimelineNavigationSnap)
				{
					case TimelineNavigationSnapMode.BinaryStorage:
					{
						if (BinaryStorage != null)
						{
							setTime = BinaryStorage.GetNextTimelineEventTime(TimelineSession.PlaybackPosition);
						}
					}
					break;
					case TimelineNavigationSnapMode.DebugModule:
					{
						if (CurrentDebugModule != null)
						{
							setTime = CurrentDebugModule.GetNextTimelineEventTime(TimelineSession.PlaybackPosition);
						}
					}
					break;
					default:
					break;
				}

				if (setTime != -1)
				{
					TimelineSession.PlaybackPosition = setTime;
					SendBackBinaryData(setTime);
				}
			}
		}

		private void PrevTimelineEvent()
		{
			int setTime = -1;
			if (TimelineSession != null && TimelineSession.Duration > 0)
			{
				switch (UserPreferencesManager.Instance.UserPreferences.TimelineNavigationSnap)
				{
					case TimelineNavigationSnapMode.BinaryStorage:
					{
						if (BinaryStorage != null)
						{
							setTime = BinaryStorage.GetPrevTimelineEventTime(TimelineSession.PlaybackPosition);
						}
					}
					break;
					case TimelineNavigationSnapMode.DebugModule:
					{
						if (CurrentDebugModule != null)
						{
							setTime = CurrentDebugModule.GetPrevTimelineEventTime(TimelineSession.PlaybackPosition);
						}
					}
					break;
					default:
					break;
				}

				if (setTime != -1)
				{
					TimelineSession.PlaybackPosition = setTime;
					SendBackBinaryData(setTime);
				}
			}
		}

		#endregion

		#region Timeline Session / Session Management

		private TimelineSession TimelineSession
		{
			get
			{
				return timelineSession;
			}
			set
			{
				if (timelineSession != value)
				{
					if (timelineSession != null)
					{
					}

					timelineSession = value;

					timelineControl.Model = timelineSession;

					if (timelineSession != null)
					{
					}
				}
			}
		}

		/// <summary>
		/// Active session data
		/// </summary>
		private void ResetSessions()
		{
			TimelineSession = new TimelineSession();

			DebugModuleManager.Instance.OnResetSessions();

			this.Invoke((MethodInvoker)delegate
			{
				if (Server != null)
				{
					EnableUserCommands(Server != null);
				}
			});

			StopPlayback();
		}

		private void UpdateDuration(int time)
		{
			if (TimelineSession.Duration < time)
			{
				TimelineSession.Duration = time;
			}

			DebugModuleManager.Instance.Heartbeat(time);
		}

		#endregion

		#region IReView_Tool Implementation

		/// <summary>
		/// Called everytime heartbeat is received
		/// </summary>
		public void Heartbeat(int time)
		{
			if (time >= 0)
			{
				UpdateDuration(time);
			}
		}

		/// <summary>
		/// Called when session starts
		/// </summary>
		public void SessionStart(long sessionId, string sessionName)
		{
			ResetSessions();
		}

		/// <summary>
		/// Called when session ends
		/// </summary>
		public void SessionEnd(long sessionId)
		{
		}

		/// <summary>
		/// Called when receiving binary data from feed (when using built-in server)
		/// </summary>
		public void StoreBinaryData(long id, int time, byte[] data)
		{
			byte[] dataCopy = new byte[data.Length];
			data.CopyTo(dataCopy, 0);
			BinaryStorage.StoreData(new BinaryData(id, time, dataCopy));
		}

		/// <summary>
		/// Called when debug toggle state is changed in the feed
		/// </summary>
		public void DebugToggleChanged(string name, bool state)
		{
			foreach (Control control in userCommandButtonFlowLayout.Controls)
			{
				UserCommand userCommand = control.Tag as UserCommand;
				if (userCommand != null && userCommand.Command == name)
				{
					// Found the control representing the debug toggle
					CheckBox cb = control as CheckBox;
					if (cb != null)
					{
						this.BeginInvoke((MethodInvoker)delegate
						{
							// Change checkbox state
							cb.Checked = state;
							cb.Text = cb.Checked ? "+ " + userCommand.Name : "- " + userCommand.Name;
						});
					}
				}
			}
		}
		
		#endregion

		#region Built-In Server

		/// <summary>
		/// Start built-in storage server
		/// </summary>
		private void StartServer()
		{
			Action<Stream, IPEndPoint> handler = new Action<Stream, IPEndPoint>(OnFeedConnecting);

			Server = new Network_Stream_Server(handler, (ushort)UserPreferencesManager.Instance.UserPreferences.ServerListenPort);

			connectButton.Text = "Disconnect";

			UpdateStatusLabel();
		}

		private void StopServer()
		{
			RPC_Manager.Instance.Close();

			DebugModuleManager.Instance.RPCStateChanged(false);

			if (Server != null)
			{
				Server.Dispose();
				Server = null;
			}

			BinaryStorage = null;

			if (IsHandleCreated)
			{
				this.BeginInvoke((MethodInvoker)delegate
				{
					connectButton.Text = "Connect";
				});

				UpdateStatusLabel();
			}
		}

		private Network_Stream_Server Server
		{
			get;
			set;
		}

		private void OnFeedConnecting(Stream stream, IPEndPoint endpoint)
		{
			BinaryStorage = new BinaryStorage(UserPreferencesManager.Instance.UserPreferences.BuiltInBinaryStorageBytes);

			if (!RPC_Manager.Instance.IsClosed)
			{
				RPC_Manager.Instance.Close();
			}

			RPC_Manager.Instance.Bind(stream);

			RPC_Manager.Instance.Create_Server_Proxy<RPC_Server_Proxy_IReView_Tool, IReView_Tool>(this);
			RPC_Manager.Instance.Create_Client_Proxy<RPC_Client_Proxy_IReView_Feed>();

			RPC_Manager.Instance.Start_Receiver();
		}

		private void RPC_Manager_OnConnectionStateChanged(bool connected)
		{
			DebugModuleManager.Instance.RPCStateChanged(connected);

			EnableUserCommands(connected);

			if (connected)
			{
				ResetSessions();
			}

			UpdateStatusLabel();
		}

		#endregion
		
		#region Create buttons for log flag filters and user commands

		/// <summary>
		/// Create user command buttons based on user preference setup
		/// </summary>
		public void CreateUserCommandButtons()
		{
			userCommandButtonFlowLayout.SuspendLayout();

			userCommandButtonFlowLayout.Controls.Clear();
			foreach (UserCommand userCommand in UserPreferencesManager.Instance.UserPreferences.UserCommands)
			{
				if (!userCommand.IsEmpty && userCommand.IsEnabled)
				{
					if (userCommand.IsToggle)
					{
						System.Windows.Forms.CheckBox userAction = new System.Windows.Forms.CheckBox();
						userAction.Appearance = Appearance.Button;
						userAction.Margin = new System.Windows.Forms.Padding(5, 5, 0, 5);
						userAction.Size = new Size(75, 64);
						userAction.Text = "- " + userCommand.Name;
						userAction.FlatStyle = FlatStyle.Flat;
						userAction.FlatAppearance.BorderSize = 0;
						userAction.FlatAppearance.CheckedBackColor = userCommand.Color;
						userAction.BackColor = userAction.FlatAppearance.CheckedBackColor;
						userAction.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
						userAction.ForeColor = userAction.BackColor.GetBrightness() <= 0.5f ? Color.White : Color.Black;
						userAction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
						userAction.TextImageRelation = TextImageRelation.ImageAboveText;
						userAction.Click += userAction_Clicked;
						userAction.AutoCheck = true;
						userAction.CheckState = CheckState.Unchecked;
						userAction.Checked = false;
						userAction.Tag = userCommand;
						userAction.FlatAppearance.BorderColor = userAction.Checked ? Color.Black : userAction.FlatAppearance.CheckedBackColor;
						userAction.Enabled = IsConnected();
						userCommandButtonFlowLayout.Controls.Add(userAction);
					}
					else
					{
						System.Windows.Forms.Button userAction = new System.Windows.Forms.Button();
						userAction.Margin = new System.Windows.Forms.Padding(5, 5, 0, 5);
						userAction.Size = new Size(75, 64);
						userAction.Text = userCommand.Name;
						userAction.FlatStyle = FlatStyle.Flat;
						userAction.FlatAppearance.BorderSize = 0;
						userAction.FlatAppearance.CheckedBackColor = userCommand.Color;
						userAction.FlatAppearance.BorderColor = userAction.FlatAppearance.CheckedBackColor;
						userAction.BackColor = userAction.FlatAppearance.CheckedBackColor;
						userAction.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
						userAction.ForeColor = Color.White;
						userAction.Tag = userCommand;
						userAction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
						userAction.TextImageRelation = TextImageRelation.ImageAboveText;
						userAction.Click += userAction_Clicked;
						userAction.Enabled = IsConnected();
						userCommandButtonFlowLayout.Controls.Add(userAction);
					}
				}
			}

			userCommandButtonFlowLayout.ResumeLayout();
		}

		/// <summary>
		/// Enable / disable user commands which depends on connection state, active only if connection established and live to the game
		/// </summary>
		/// <param name="enabled"></param>
		private void EnableUserCommands(bool enabled)
		{
			if (IsHandleCreated)
			{
				this.BeginInvoke((MethodInvoker)delegate
				{
					foreach (Control control in userCommandButtonFlowLayout.Controls)
					{
						control.Enabled = enabled;
						UserCommand userCommand = control.Tag as UserCommand;
					}
				});
			}
		}

		#endregion

		#region Miscellaneous

		/// <summary>
		/// Connected or running a server
		/// </summary>
		private bool IsConnected()
		{
			return RPC_Manager.Instance.IsConnected;
		}

		private void UpdateStatusLabel()
		{
			string newStatus = "Status is unknown";
			if (RPC_Manager.Instance.IsConnected)
			{
				if (Server != null)
				{
					newStatus = "Feed connected";
				}
				else
				{
					newStatus = "Connected to external storage server";
				}
			}
			else if (Server != null)
			{
				newStatus = "Listening for incoming feed connections...";
			}
			else
			{
				newStatus = "Server stopped";
			}

			if (IsHandleCreated)
			{
				this.BeginInvoke((MethodInvoker)delegate
				{
					statusLabel.Text = newStatus;
				});
			}
		}

		private BinaryStorage BinaryStorage
		{
			get
			{
				return binaryStorage;
			}
			set
			{
				if (binaryStorage != value)
				{
					binaryStorage = value;
				}
			}
		}

		#endregion

		private BindingSource sessionList;
		private BinaryStorage binaryStorage;
		private Dictionary<VerticalButton, DebugModule> debugModuleButtonMap = new Dictionary<VerticalButton, DebugModule>();
		private DebugModule currentDebugModule;
		private TimelineSession timelineSession;
	}
}
