using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Threading;
using ReView;
using ReViewRPC;

namespace ReViewTool.Modules.HierarchicalTimelineLog
{
	public partial class HierarhicalTimelineLog : DebugModule, IReView_HierarchicalTimelineLog
	{
		public HierarhicalTimelineLog()
		{
			InitializeComponent();

			AutoSize = true;
			BackColor = System.Drawing.Color.White;

			LogFilter = "";
			ShowGenericTracks = true;

			this.tracker.OnZoomChanged += OnSequencerControlZoomChanged;
			this.tracker.OnPanOffsetChanged += OnSequencerControlPanOffsetChanged;
			this.tracker.OnItemSelectionChanged += OnSequencerControlItemSelectionChanged;
			this.tracker.OnTrackSelectionChanged += OnSequencerControlTrackSelectionChanged;

			this.ReViewOverviewControl.OverviewInterface = this.tracker;

			this.viewportContainer.HorizontalScrollBarMarginsChanged += OnHorizontalSrollBarMarginsChanged;

			textUpdateEvent = new AutoResetEvent(false);
			textUpdateRefreshThread = new Thread(new ThreadStart(UpdateText));
			textUpdateRefreshThread.IsBackground = true;
			textUpdateRefreshThread.Start();

			UserPreferencesManager.Instance.OnUserPreferencesChanged += OnUserPreferencesChanged;

			HTLButtonContainer = new HTLButtonContainer();
		}

		#region Event Listeners

		private void OnHorizontalSrollBarMarginsChanged()
		{
			Rectangle horizontalScrollBarRectangle = viewportContainer.GetHorizontalScrollBarRectangle();
			RequestTimelineMarginChange(horizontalScrollBarRectangle.X, Width - (horizontalScrollBarRectangle.X + horizontalScrollBarRectangle.Width));
		}

		/// <summary>
		/// Callback invoked when user preferences are changed -> Recreates buttons and updates colors to the logTracker component.
		/// </summary>
		private void OnUserPreferencesChanged(object sender)
		{
			foreach (LogFlagColor logFlagColor in UserPreferencesManager.Instance.UserPreferences.LogFlagColors)
			{
				if (logFlagColor.LogFlagIndex == 0)
				{
					tracker.ItemActiveBackColor = logFlagColor.Color;
				}
				tracker.SetFlagColor(logFlagColor.LogFlagIndex, logFlagColor.Color);
			}

			CreateLogFlagFilterButtons();
		}

		protected void OnSequencerControlZoomChanged(SequencerControl sender)
		{
			NotifyTimePixelRatioChanged(sender.TimePixelRatio);
		}

		protected void OnSequencerControlPanOffsetChanged(int x, int y, bool userChange)
		{
			NotifyPanOffsetChanged(x, userChange);
		}

		protected void OnUpdateSequencerControlSelection(Track selectedTrack, Item selectedItem)
		{
			if (disableSendSelection)
			{
				// If disabled send selection then bail out (this done when receiving selection from the feed so that there is no endless ping-pong)
				return;
			}
			long selectedTrackId = selectedItem != null ? selectedItem.Parent.Id : selectedTrack != null ? selectedTrack.Id : -1;
			if (selectedTrackId >= 0)
			{
				IReView_Feed proxy = RPC_Manager.Instance.Get_Client_Proxy<IReView_Feed>();
				if (proxy != null)
				{
					proxy.SelectionChanged(selectedTrackId);
				}
			}
		}

		protected void OnSequencerControlTrackSelectionChanged(SequencerControl sender)
		{
			OnUpdateSequencerControlSelection(sender.SelectedTrack, sender.SelectedItem);
		}

		protected void OnSequencerControlItemSelectionChanged(SequencerControl sender)
		{
			textUpdateEvent.Set();

			OnUpdateSequencerControlSelection(sender.SelectedTrack, sender.SelectedItem);
		}

		#endregion

		#region IReView_Debug implementation

		/// <summary>
		/// Called when selection is changed in the feed
		/// </summary>
		public void SelectionChanged(long selectedId)
		{
			disableSendSelection = true;

			Track selectedTrack = Session.GetTrack(selectedId);
			Item selectedItem = Session.GetItem(selectedId);
			if (selectedTrack != null)
			{
				if (SetSelection(selectedTrack))
				{
					ScrollToSelection();
				}
			}
			else if (selectedItem != null)
			{
				if (SetSelection(selectedItem))
				{
					ScrollToSelection();
				}
			}
			else
			{
				SetSelection((Item)null);
			}

			disableSendSelection = false;
		}

		private bool SetSelection(Track selectedTrack)
		{
			if (tracker.SelectedTrack != selectedTrack)
			{
				tracker.SelectedTrack = selectedTrack;
				tracker.SelectedItem = null;
				return true; // Changed
			}
			return false; // No change
		}

		private bool SetSelection(Item selectedItem)
		{
			if (tracker.SelectedItem != selectedItem)
			{
				tracker.SelectedTrack = selectedItem != null ? selectedItem.Parent : null;
				tracker.SelectedItem = selectedItem;
				return true; // Changed
			}
			return false; // No change
		}

		private void ScrollToSelection()
		{
			tracker.ScrollToSelection();
		}

		/// <summary>
		/// Called when generic item is being added to given parent track. This item ends up either on an existing child-track or a new child-track is created if none available
		/// </summary>
		public void AddGenericItem(long parentId, long id, int startTime, String name)
		{
			Track parentTrack = Session.GetTrack(parentId);
			if (parentTrack != null)
			{
				Item item = new Item(name, id, startTime, startTime);
				if (item.Length < UserPreferencesManager.Instance.UserPreferences.MinItemLengthInMillis)
				{
					item.EndTime = item.StartTime + UserPreferencesManager.Instance.UserPreferences.MinItemLengthInMillis;
				}
				item.Active = true;
				Session.AddGenericItem(parentTrack, item);

				NotifyDurationChanged(startTime);
			}
		}

		/// <summary>
		/// Called when track is being added, optionally may have parent track specified (-1 if root)
		/// </summary>
		public void AddTrack(long parentId, long id, String name)
		{
			Track track = new Track(name, id);
			Session.AddTrack(Session.GetTrack(parentId), track);
		}

		/// <summary>
		/// Called when item is being added to parent track
		/// </summary>
		public void AddItem(long parentId, long id, int startTime, String name)
		{
			Track parentTrack = Session.GetTrack(parentId);
			if (parentTrack != null)
			{
				Item lastItem = parentTrack.LastItem;
				Item item = new Item(name, id, startTime, startTime);
				item.Active = true;

				if (lastItem != null && lastItem.EndTime > startTime)
				{
					Session.MergeItem(lastItem, item);
				}
				else
				{
					Session.AddItem(parentTrack, item);
				}

				NotifyDurationChanged(startTime);
			}
		}

		/// <summary>
		/// Called when given item should be closed
		/// </summary>
		public void EndItem(long id, int endTime)
		{
			Item item = Session.GetItem(id);
			if (item != null && item.Active)
			{
				item.EndTime = endTime;
				if (item.Length < UserPreferencesManager.Instance.UserPreferences.MinItemLengthInMillis)
				{
					item.EndTime = item.StartTime + UserPreferencesManager.Instance.UserPreferences.MinItemLengthInMillis;
				}
				item.Active = false;

				NotifyDurationChanged(endTime);
			}
		}

		/// <summary>
		/// Called when log entry should be appended to given item
		/// </summary>
		public void AppendLog(long id, int time, uint flags, string content)
		{
			Item item = Session.GetItem(id);
			if (item != null && item.Active)
			{
				item.EndTime = time;
				item.Log.AddLogEntry(new LogEntry(time, content, flags));

				NotifyDurationChanged(time);
			}
		}

		#endregion

		#region DebugModule Implementation

		public override string ModuleName
		{
			get
			{
				return "Timeline Log";
			}
		}

		public override void OnInitDebugModule()
		{
			Session = new HTLSession();
		}

		public override void OnActivateDebugModule()
		{
			Rectangle horizontalScrollBarRectangle = viewportContainer.GetHorizontalScrollBarRectangle();
			RequestTimelineMarginChange(horizontalScrollBarRectangle.X, Width - (horizontalScrollBarRectangle.X + horizontalScrollBarRectangle.Width));

			NotifyDurationChanged(Session.Duration);
		}

		public override void OnDeactivateDebugModule()			
		{
		}

		public override void OnRPCStateChanged(bool connected)
		{
			if (connected)
			{
				RPC_Manager.Instance.Create_Server_Proxy<RPC_Server_Proxy_IReView_HierarchicalTimelineLog, IReView_HierarchicalTimelineLog>(this);
			}
		}

		public override void OnResetSessions()
		{
			Session = new HTLSession();
		}

		public override void OnHeartbeat(int time)
		{
			Session.UpdateDuration(time);
		}

		public override void OnTimelinePlaybackPositionChanged(int playbackPosition)
		{
			Session.PlaybackPosition = playbackPosition;

			textUpdateEvent.Set(); // Request text to be updated

			this.tracker.Invalidate();
		}

		public override void OnTimelineZoomChanged(float timePixelRatio)
		{
			this.tracker.ForceTimePixelRatio(timePixelRatio);
		}

		public override void OnTimelinePanOffsetChanged(Point panOffset)
		{
			this.tracker.ForcePanOffset(panOffset);
		}

		public override FlowLayoutPanel GetToolbarButtonFlowLayout() 
		{
			return HTLButtonContainer.GetFlowLayoutPanel();
		}

		/// <summary>
		/// Someone clicks / checks generic tracks toggle
		/// </summary>
		private void toggleGenericTracks_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox cb = sender as CheckBox;
			ShowGenericTracks = cb.Checked;
			cb.Text = cb.Checked ? "+ Generic tracks" : "- Generic tracks";
		}

		/// <summary>
		/// Someone clicks / checks one of the log flag filter buttons
		/// </summary>
		private void logFlagFilterButton_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox cb = sender as CheckBox;
			if (cb != null)
			{
				LogFlagColor lfc = cb.Tag as LogFlagColor;
				SetShowFlag(lfc.LogFlagBit, cb.Checked);
				cb.Text = cb.Checked ? "+ " + lfc.Name : "- " + lfc.Name;
			}
		}

		/// <summary>
		/// Focus leaves regexp filter
		/// </summary>
		private void regexpFilter_Leave(object sender, EventArgs e)
		{
			TextBox tb = sender as TextBox;
			LogFilter = tb.Text;
		}

		/// <summary>
		/// Someone hits key while regexpFilter field has focus
		/// </summary>
		private void regexpFilter_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (e.KeyCode == Keys.Enter)
			{
				LogFilter = tb.Text;
			}
		}

		#endregion

		/// <summary>
		/// Create log flag filter buttons based on user preferences
		/// </summary>
		private void CreateLogFlagFilterButtons()
		{
			FlowLayoutPanel logFlagFilterButtonFlowLayout = HTLButtonContainer.GetLogFlagFilterFlowLayoutPanel();

			logFlagFilterButtonFlowLayout.SuspendLayout();

			logFlagFilterButtonFlowLayout.Controls.Clear();

			foreach (LogFlagColor logFlagColor in UserPreferencesManager.Instance.UserPreferences.LogFlagColors)
			{
				if (logFlagColor.DisplayFilterButton)
				{
					System.Windows.Forms.CheckBox logFlagFilterButton = new System.Windows.Forms.CheckBox();
					logFlagFilterButton.Appearance = Appearance.Button;
					logFlagFilterButton.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
					logFlagFilterButton.Size = new Size(75, 64);
					logFlagFilterButton.Text = "+" + logFlagColor.Name;
					logFlagFilterButton.FlatStyle = FlatStyle.Flat;
					logFlagFilterButton.FlatAppearance.BorderSize = 0;
					logFlagFilterButton.FlatAppearance.CheckedBackColor = logFlagColor.Color;
					logFlagFilterButton.BackColor = logFlagFilterButton.FlatAppearance.CheckedBackColor;
					logFlagFilterButton.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
					logFlagFilterButton.ForeColor = logFlagFilterButton.BackColor.GetBrightness() <= 0.5f ? Color.White : Color.Black;
					logFlagFilterButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
					logFlagFilterButton.TextImageRelation = TextImageRelation.ImageAboveText;
					logFlagFilterButton.Tag = logFlagColor;
					logFlagFilterButton.CheckedChanged += logFlagFilterButton_CheckedChanged;
					logFlagFilterButton.CheckState = CheckState.Checked;
					logFlagFilterButton.Checked = true;
					logFlagFilterButton.UseVisualStyleBackColor = false;
					logFlagFilterButton.FlatAppearance.BorderColor = logFlagFilterButton.Checked ? Color.Black : logFlagFilterButton.FlatAppearance.CheckedBackColor;
					logFlagFilterButtonFlowLayout.Controls.Add(logFlagFilterButton);
				}
			}

			logFlagFilterButtonFlowLayout.ResumeLayout();
		}

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

			textUpdateRunning = false;
			textUpdateEvent.Set();
			textUpdateRefreshThread.Join();
			textUpdateRefreshThread = null;
		}

		private void UpdateText()
		{
			textUpdateRunning = true;
			while (textUpdateRunning)
			{
				textUpdateEvent.WaitOne();

				try
				{
					this.BeginInvoke((MethodInvoker)delegate
					{
						UpdateLogText();
					});
				}
				catch (Exception)
				{
					textUpdateRunning = false;
				}
			}
		}

		public HTLSession Session
		{
			get { return tracker.Model as HTLSession; }
			set 
			{
				tracker.Model = value;
				if (InvokeRequired)
				{
					this.BeginInvoke((MethodInvoker)delegate
					{
						logTextBox.Clear();
					});
				}
				else
				{
					logTextBox.Clear();
				}
			}
		}

		public HTLButtonContainer HTLButtonContainer
		{
			get
			{
				return htlButtonContainer;
			}
			set
			{
				if (htlButtonContainer != value)
				{
					if (htlButtonContainer != null)
					{
						htlButtonContainer.GetToggleGenericTracks().CheckedChanged -= toggleGenericTracks_CheckedChanged;
						htlButtonContainer.GetRegExpFilter().KeyDown -= regexpFilter_KeyDown;
						htlButtonContainer.GetRegExpFilter().Leave -= regexpFilter_Leave;
					}

					htlButtonContainer = value;

					if (htlButtonContainer != null)
					{
						htlButtonContainer.GetToggleGenericTracks().CheckedChanged += toggleGenericTracks_CheckedChanged;
						htlButtonContainer.GetRegExpFilter().KeyDown += regexpFilter_KeyDown;
						htlButtonContainer.GetRegExpFilter().Leave += regexpFilter_Leave;
					}
				}
			}
		}

		protected void UpdateLogText()
		{
			int playbackTime = Session.PlaybackPosition;
			rtfDummy.SuspendLayout();
			this.rtfDummy.Text = "";

			BindingSource logFlagColors = UserPreferencesManager.Instance.UserPreferences.LogFlagColors;

			if (tracker.SelectedItem != null)
			{
				int current = 0;
				int count = 0;
				List<LogEntry> entries = new List<LogEntry>(tracker.SelectedItem.Log.LogEntries);
				foreach (LogEntry entry in entries)
				{
					int colorIndex = (entry.HasFlags() ? (entry.GetHighestFlagBit() + 1) : 0);
					string newEntry = entry.TimeAsText + " " + entry.Content + "\n";
					rtfDummy.AppendText(newEntry);
					rtfDummy.Select(current, newEntry.Length);

					bool inPast = playbackTime >= entry.Time;

					LogFlagColor lfc = logFlagColors[colorIndex] as LogFlagColor;
					Color textColor = Color.FromArgb(inPast ? 255 : 128, lfc.Color.R / 3, lfc.Color.G / 3, lfc.Color.B / 3);

					rtfDummy.SelectionFont = inPast ? logFont : logFontTransparent;
					rtfDummy.SelectionColor = textColor;

					current += newEntry.Length;
					count++;
				}
			}
			this.logTextBox.Rtf = rtfDummy.Rtf;
		}

		public bool ShowGenericTracks
		{
			get
			{
				return tracker.ShowGenericTracks;
			}
			set
			{
				tracker.ShowGenericTracks = value;
			}
		}

		public string LogFilter
		{
			get
			{
				return tracker.LogFilter;
			}
			set
			{
				tracker.LogFilter = value;
			}
		}

		public void SetShowFlag(int logFlagIndex, bool value)
		{
			if (value)
			{
				tracker.SetShowFlag(logFlagIndex);
			}
			else
			{
				tracker.ClearShowFlag(logFlagIndex);
			}
		}

		private Font logFont = new Font("Consolas", 10, FontStyle.Regular);
		private Font logFontTransparent = new Font("Consolas", 10, FontStyle.Italic);

		private bool disableSendSelection = false;

		private Thread textUpdateRefreshThread = null;
		private AutoResetEvent textUpdateEvent;
		private bool textUpdateRunning = false;

		private HTLButtonContainer htlButtonContainer;
	}
}