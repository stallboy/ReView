using ReView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ReViewTool
{
	public enum ShowNormalsMode
	{
		None,
		Lines,
		Triangles,
	}

	public enum TimelineNavigationSnapMode
	{
		BinaryStorage,
		DebugModule,
	}

	/// <summary>
	/// User preferences managed by this class (Singleton)
	/// </summary>
	public class UserPreferencesManager
	{
		private UserPreferencesManager()
		{
		}

		public static UserPreferencesManager Instance
		{
			get
			{
				// Double-checked locking
				if (sUserPreferences == null)
				{
					lock (sInstanceLock)
					{
						if (sUserPreferences == null)
						{
							sUserPreferences = new UserPreferencesManager();
						}
					}
				}
				return sUserPreferences;
			}
			private set { }
		}

		public void ResetToDefaults()
		{
			UserPreferences = new UserPreferences();

			UserPreferences.MinItemLengthInMillis = 200;

			UserPreferences.LogFlagColors.Add(new LogFlagColor(0, "Default", Color.White, false));
			UserPreferences.LogFlagColors.Add(new LogFlagColor(1, "Info", Color.DarkSeaGreen, true));
			UserPreferences.LogFlagColors.Add(new LogFlagColor(2, "Warning", Color.DarkKhaki, true));
			UserPreferences.LogFlagColors.Add(new LogFlagColor(3, "Error", Color.MediumOrchid, true));
			for (int i = 4; i <= 32; i++)
			{
				UserPreferences.LogFlagColors.Add(new LogFlagColor(i, "", Color.White, false));
			}
		}

		private void OnUserPreferencesPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyUserPreferencesChanged();
		}

		public Color GetLogFlagColor(UInt32 logFlag)
		{
			if (logFlag == 0)
			{
				return (Color)UserPreferences.LogFlagColors[0];
			}
			int highestBitSet = (int)Math.Floor(Math.Log(logFlag, 2));
			return (Color)UserPreferences.LogFlagColors[highestBitSet];
		}

		private void TryLoad()
		{
			if (!File.Exists(sFileName))
			{
				return;
			}

			try
			{
				UserPreferences newPrefs = null;
				using (FileStream stream = File.OpenRead(sFileName))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(UserPreferences));
					newPrefs = serializer.Deserialize(stream) as UserPreferences;
				}
				if (newPrefs != null)
				{
					UserPreferences = newPrefs;
				}

				NotifyUserPreferencesChanged();
			}
			catch (Exception)
			{
				ResetToDefaults();
			}
		}

		public void Load()
		{
			ResetToDefaults();
			TryLoad();
		}

		public UserPreferences UserPreferences
		{
			get
			{
				return userPreferences;
			}

			private set
			{
				if (userPreferences != value)
				{
					if (userPreferences != null)
					{
						userPreferences.PropertyChanged -= OnUserPreferencesPropertyChanged;
					}
					userPreferences = value;

					userPreferences.PropertyChanged += OnUserPreferencesPropertyChanged;

					NotifyUserPreferencesChanged();
				}
			}
		}

		public void Save()
		{
			using (StreamWriter writer = new StreamWriter(sFileName))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(UserPreferences));
				serializer.Serialize(writer, UserPreferences);
				writer.Flush();
			}
		}

		public delegate void UserPreferencesChanged(UserPreferences source);
		public event UserPreferencesChanged OnUserPreferencesChanged;

		private void NotifyUserPreferencesChanged()
		{
			UserPreferencesChanged handler = OnUserPreferencesChanged;
			if (handler != null)
			{
				handler(UserPreferences);
			}
		}

		private UserPreferences userPreferences;

		private static object sInstanceLock = new Object();
		private static UserPreferencesManager sUserPreferences = null;
		private static string sFileName = "user-preferences.xml";
	}

	/// <summary>
	/// Class to specify a color for given log-flag entry
	/// </summary>
	public class LogFlagColor : INotifyPropertyChanged
	{
		public LogFlagColor()
		{
			LogFlagIndex = 0;
			Color = Color.White;
		}

		public LogFlagColor(int logFlagIndex, String name, String htmlColorString, bool displayFilterButton)
		{
			this.LogFlagIndex = logFlagIndex;
			this.Name = name;
			this.DisplayFilterButton = displayFilterButton;
			HTMLColor = htmlColorString;
		}

		public LogFlagColor(int logFlagIndex, String name, Color color, bool displayFilterButton)
		{
			this.LogFlagIndex = logFlagIndex;
			this.Name = name;
			this.DisplayFilterButton = displayFilterButton;
			Color = color;
		}

		public int LogFlagIndex
		{
			get
			{
				return logFlagIndex;
			}
			set
			{
				if (logFlagIndex != value)
				{
					logFlagIndex = value;

					NotifyPropertyChanged("LogFlagIndex");
				}
			}
		}

		[XmlIgnore]
		public int LogFlagBit
		{
			get
			{
				return logFlagIndex - 1;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (name != value)
				{
					name = value;

					NotifyPropertyChanged("Name");
				}
			}
		}

		public bool DisplayFilterButton
		{
			get
			{
				return displayFilterButton;
			}
			set
			{
				if (displayFilterButton != value)
				{
					displayFilterButton = value;

					NotifyPropertyChanged("DisplayFilterButton");
				}
			}
		}

		public string HTMLColor
		{
			get
			{
				return System.Drawing.ColorTranslator.ToHtml(Color);
			}
			set
			{
				Color = System.Drawing.ColorTranslator.FromHtml(value);
			}
		}

		[XmlIgnore]
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				if (color != value)
				{
					color = value;

					NotifyPropertyChanged("Color");
				}
			}
		}

		public static implicit operator Color(LogFlagColor fc)
		{
			return fc.Color;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs("")); // If propertyName is given ListBox will completely refresh
			}
		}

		private int logFlagIndex;
		private string name;
		private Color color;
		private bool displayFilterButton;
	}

	/// <summary>
	/// Container for user preferences, UserPreferenceManager will own this
	/// </summary>
	[XmlInclude(typeof(LogFlagColor))]
	[XmlInclude(typeof(StorageServerConnection))]
	[XmlInclude(typeof(UserCommand))]
	[XmlInclude(typeof(Color32))]
	public class UserPreferences : INotifyPropertyChanged
	{
		public UserPreferences()
		{
			userCommands = new BindingSource();
			logFlagColors = new BindingSource();
			connectionSettings = new BindingSource();
			minItemLengthInMillis = 0;
			serverMode = false;
			serverListenPort = 5000;
			autoAttachToLiveSession = false;
			builtInBinaryStorageMegaBytes = 1024;
			debugRenderClearColor = new Color32(32, 32, 32, 255);
			debugRender3DView = true;
			debugRenderAnnotations = true;
			debugRenderShowGrid = true;
			debugRenderFSAA = false;
			debugRenderShowNormals = ShowNormalsMode.Lines;

			timelinePlaybackLoop = false;
			timelineNavigationSnap = TimelineNavigationSnapMode.DebugModule;

			connectionSettings.ListChanged += OnConnectionSettingsListChanged;
			userCommands.ListChanged += OnUserCommandsListChanged;
			logFlagColors.ListChanged += OnLogFlagColorsListChanged;
		}

		private void OnLogFlagColorsListChanged(object sender, ListChangedEventArgs e)
		{
			foreach (LogFlagColor logFlagColor in logFlagColors)
			{
				logFlagColor.PropertyChanged -= OnLogFlagColorChanged;
				logFlagColor.PropertyChanged += OnLogFlagColorChanged;
			}
			NotifyPropertyChanged("LogFlagColorsItemsChange");
		}

		private void OnConnectionSettingsListChanged(object sender, ListChangedEventArgs e)
		{
			foreach (StorageServerConnection connection in connectionSettings)
			{
				connection.PropertyChanged -= OnConnectionChanged;
				connection.PropertyChanged += OnConnectionChanged;
			}
			NotifyPropertyChanged("ConnectionSettingsItemsChange");
		}

		private void OnUserCommandsListChanged(object sender, ListChangedEventArgs e)
		{
			foreach (UserCommand userCommand in userCommands)
			{
				userCommand.PropertyChanged -= OnUserCommandChanged;
				userCommand.PropertyChanged += OnUserCommandChanged;
			}
			NotifyPropertyChanged("UserCommandsItemsChange");
		}

		private void OnUserCommandChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged("UserCommandsItemsChange");
		}

		private void OnConnectionChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged("ConnectionSettingsItemChange");
		}

		private void OnLogFlagColorChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged("LogFlagColorsItemsChange");
		}

		public int MinItemLengthInMillis
		{
			get
			{
				return minItemLengthInMillis;
			}
			set
			{
				if (minItemLengthInMillis != value)
				{
					minItemLengthInMillis = value;
					NotifyPropertyChanged("MinItemLengthInMillis");
				}
			}
		}

		public bool TimelinePlaybackLoop
		{
			get
			{
				return timelinePlaybackLoop;
			}
			set
			{
				if (timelinePlaybackLoop != value)
				{
					timelinePlaybackLoop = value;
					NotifyPropertyChanged("TimelinePlaybackLoop");
				}
			}
		}

		public TimelineNavigationSnapMode TimelineNavigationSnap
		{
			get
			{
				return timelineNavigationSnap;
			}
			set
			{
				if (timelineNavigationSnap != value)
				{
					timelineNavigationSnap = value;
					NotifyPropertyChanged("TimelineNavigationSnap");
				}
			}
		}

		public bool ServerMode
		{
			get
			{
				return serverMode;
			}
			set
			{
				if (serverMode != value)
				{
					serverMode = value;
					NotifyPropertyChanged("ServerMode");
				}
			}
		}

		public bool DebugRenderAnnotations
		{
			get
			{
				return debugRenderAnnotations;
			}
			set
			{
				if (debugRenderAnnotations != value)
				{
					debugRenderAnnotations = value;
					NotifyPropertyChanged("DebugRenderAnnotations");
				}
			}
		}

		public bool DebugRenderShowGrid
		{
			get
			{
				return debugRenderShowGrid;
			}
			set
			{
				if (debugRenderShowGrid != value)
				{
					debugRenderShowGrid = value;
					NotifyPropertyChanged("DebugRenderShowGrid");
				}
			}
		}

		public bool DebugRenderFSAA
		{
			get
			{
				return debugRenderFSAA;
			}
			set
			{
				if (debugRenderFSAA != value)
				{
					debugRenderFSAA = value;
					NotifyPropertyChanged("DebugRenderFSAA");
				}
			}
		}

		public bool DebugRender3DView
		{
			get
			{
				return debugRender3DView;
			}
			set
			{
				if (debugRender3DView != value)
				{
					debugRender3DView = value;
					NotifyPropertyChanged("DebugRender3DView");
				}
			}
		}

		public ShowNormalsMode DebugRenderShowNormals
		{
			get
			{
				return debugRenderShowNormals;
			}
			set
			{
				if (debugRenderShowNormals != value)
				{
					debugRenderShowNormals = value;
					NotifyPropertyChanged("DebugRenderShowNormals");
				}
			}
		}
		
		public Color32 DebugRenderClearColor
		{
			get
			{
				return debugRenderClearColor;
			}
			set
			{
				if (debugRenderClearColor != value)
				{
					debugRenderClearColor = value;
					NotifyPropertyChanged("DebugRenderClearColor");
				}
			}
		}

		public bool AutoAttachToLiveSession
		{
			get
			{
				return autoAttachToLiveSession;
			}
			set
			{
				if (autoAttachToLiveSession != value)
				{
					autoAttachToLiveSession = value;
					NotifyPropertyChanged("AutoAttachToLiveSession");
				}
			}
		}

		public int ServerListenPort
		{
			get
			{
				return serverListenPort;
			}
			set
			{
				if (serverListenPort != value)
				{
					serverListenPort = value;
					NotifyPropertyChanged("ServerListenPort");
				}
			}
		}

		public long BuiltInBinaryStorageMegaBytes
		{
			get
			{
				return builtInBinaryStorageMegaBytes;
			}
			set
			{
				if (builtInBinaryStorageMegaBytes != value)
				{
					builtInBinaryStorageMegaBytes = value;
					NotifyPropertyChanged("BuildInBinaryStorageMegaBytes");
				}
			}
		}

		public long BuiltInBinaryStorageBytes
		{
			get
			{
				return builtInBinaryStorageMegaBytes * 1024 * 1024;
			}
		}

		public BindingSource LogFlagColors
		{
			get
			{
				return logFlagColors;
			}
			set
			{
				if (logFlagColors != value)
				{
					logFlagColors = value;
					NotifyPropertyChanged("LogFlagColors");
				}
			}
		}

		public BindingSource UserCommands
		{
			get
			{
				return userCommands;
			}
			set
			{
				if (userCommands != value)
				{
					userCommands = value;
					NotifyPropertyChanged("UserCommands");
				}
			}
		}

		public BindingSource ConnectionSettings
		{
			get
			{
				return connectionSettings;
			}
			set
			{
				if (connectionSettings != value)
				{
					connectionSettings = value;
					NotifyPropertyChanged("ConnectionSettings");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs("")); // If propertyName is given ListBox will completely refresh
			}
		}

		// Misc settings
		private int minItemLengthInMillis;
		// Log item flag-color setup
		private BindingSource logFlagColors;
		// User commands (console commands etc) setup
		private BindingSource userCommands;
		// Connection setup
		private BindingSource connectionSettings;
		private bool serverMode;
		private int serverListenPort;
		// Session setup
		private bool autoAttachToLiveSession;
		// Binary storage setup
		private long builtInBinaryStorageMegaBytes;
		// Debug render
		private bool debugRenderAnnotations;
		private bool debugRenderFSAA;
		private bool debugRender3DView;
		private Color32 debugRenderClearColor;
		private ShowNormalsMode debugRenderShowNormals;
		private bool debugRenderShowGrid;
		// Timeline playback control
		private bool timelinePlaybackLoop;
		private TimelineNavigationSnapMode timelineNavigationSnap;
	}

	/// <summary>
	/// Storage server connection configuration container class
	/// </summary>
	public class StorageServerConnection : INotifyPropertyChanged
	{
		public StorageServerConnection()
		{
			IPAddress = "127.0.0.1";
			Name = "localhost";
			Port = 5001;
			AutoConnect = false;
		}

		public string IPAddress
		{
			get
			{
				return ipAddress;
			}
			set
			{
				if (ipAddress != value)
				{
					ipAddress = value;
					NotifyPropertyChanged("IPAddress");
				}
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (name != value)
				{
					name = value;
					NotifyPropertyChanged("Name");
				}
			}
		}

		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				if (port != value)
				{
					port = value;
					NotifyPropertyChanged("Port");
				}
			}
		}

		public bool AutoConnect
		{
			get
			{
				return autoConnect;
			}
			set
			{
				if (autoConnect != value)
				{
					autoConnect = value;
					NotifyPropertyChanged("AutoConnect");
				}
			}
		}

		public override string ToString()
		{
			return Name + " [" + IPAddress + ":" + Port + "]" + (AutoConnect ? " (Auto-connect)" : "");
		}

		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs("")); // If propertyName is given ListBox will completely refresh
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private string name;
		private string ipAddress;
		private int port;
		private bool autoConnect;
	}

	/// <summary>
	/// User command entry, UserPreferences contain list of these
	/// </summary>
	public class UserCommand : INotifyPropertyChanged
	{
		public UserCommand()
		{
			Name = String.Empty;
			Command = String.Empty;
			IsToggle = false;
			IsEnabled = false;
			Color = Color.MediumSeaGreen;
		}

		public UserCommand(string name, string inCommand, bool toggle, bool enabled, Color color)
		{
			Name = name;
			Command = inCommand;
			IsToggle = toggle;
			IsEnabled = enabled;
			Color = color;
		}

		public bool IsEmpty
		{
			get
			{
				return (Name.Length == 0 && Command.Length == 0 && !IsToggle && !IsEnabled);
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (name != value)
				{
					name = value;
					NotifyPropertyChanged("Name");
				}
			}
		}

		public bool IsToggle
		{
			get
			{
				return isToggle;
			}
			set
			{
				if (isToggle != value)
				{
					isToggle = value;
					NotifyPropertyChanged("IsToggle");
				}
			}
		}

		public bool IsEnabled
		{
			get
			{
				return isEnabled;
			}
			set
			{
				if (isEnabled != value)
				{
					isEnabled = value;
					NotifyPropertyChanged("IsEnabled");
				}
			}
		}

		public string Command
		{
			get
			{
				return command;
			}
			set
			{
				if (command != value)
				{
					command = value;
					NotifyPropertyChanged("Command");
				}
			}
		}

		public string HTMLColor
		{
			get
			{
				return System.Drawing.ColorTranslator.ToHtml(Color);
			}
			set
			{
				Color = System.Drawing.ColorTranslator.FromHtml(value);
			}
		}

		[XmlIgnore]
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				if (color != value)
				{
					color = value;

					NotifyPropertyChanged("Color");
				}
			}
		}

		public static implicit operator Color(UserCommand command)
		{
			return command.Color;
		}

		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs("")); // If propertyName is given ListBox will completely refresh
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private Color color;
		private string command;
		private string name;
		private bool isEnabled;
		private bool isToggle;
	}
}
