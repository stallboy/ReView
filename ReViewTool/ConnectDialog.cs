using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReViewTool
{
	public enum ConnectOperation
	{
		None,
		ConnectToStorageServer,
		HostStorageServer
	}

	public partial class ConnectDialog : Form
	{
		public ConnectDialog()
		{
			InitializeComponent();

			listenPort.Text = "" + UserPreferencesManager.Instance.UserPreferences.ServerListenPort;
			hostOnStartUpCheckBox.Checked = UserPreferencesManager.Instance.UserPreferences.ServerMode;

			UserPreferencesManager.Instance.UserPreferences.PropertyChanged += OnUserPreferencesChanged;

			operationRequested = ConnectOperation.None;
		
			SetDirty(false);
		}

		protected override void OnFormClosed(FormClosedEventArgs args)
		{
			UserPreferencesManager.Instance.UserPreferences.PropertyChanged -= OnUserPreferencesChanged;
		}

		private void OnUserPreferencesChanged(object sender, PropertyChangedEventArgs e)
		{
			SetDirty(true);
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			UserPreferencesManager.Instance.Load();
			DialogResult = DialogResult.Cancel;
		}

		private void cancelHostButton_Click(object sender, EventArgs e)
		{
			UserPreferencesManager.Instance.Load();
			DialogResult = DialogResult.Cancel;
		}

		private void connectButton_Click(object sender, EventArgs e)
		{
			UserPreferencesManager.Instance.Save();
			SetDirty(false);
			operationRequested = ConnectOperation.ConnectToStorageServer;
			DialogResult = DialogResult.OK;
		}

		private void hostButton_Click(object sender, EventArgs e)
		{
			UserPreferencesManager.Instance.Save();
			SetDirty(false);
			operationRequested = ConnectOperation.HostStorageServer;
			DialogResult = DialogResult.OK;
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			UserPreferencesManager.Instance.Save();
			SetDirty(false);
		}

		private void applyHostSettingsButton_Click(object sender, EventArgs e)
		{
			UserPreferencesManager.Instance.Save();
			SetDirty(false);
		}

		private void SetDirty(bool dirty)
		{
			applyHostSettingsButton.Enabled = dirty;
			this.dirty = dirty;
		}

		public ConnectOperation OperationRequested
		{
			get
			{
				return operationRequested;
			}
		}

		public int ListenPort
		{
			get
			{
				return UserPreferencesManager.Instance.UserPreferences.ServerListenPort;
			}
		}

		// Listen UI component changes

		private void listenPort_LostFocus(object sender, EventArgs e)
		{
			UpdateListenPort();
		}

		private void listenPort_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				UpdateListenPort();
				e.Handled = true;
				SelectNextControl(sender as Control, true, true, true, true);
			}
		}

		private void UpdateListenPort()
		{
			int newListenPort = -1;
			if (Int32.TryParse(listenPort.Text, out newListenPort))
			{
				UserPreferencesManager.Instance.UserPreferences.ServerListenPort = newListenPort;
			}
			else
			{
				listenPort.Text = "" + UserPreferencesManager.Instance.UserPreferences.ServerListenPort;
			}
		}

		private void hostOnStartUpCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			UserPreferencesManager.Instance.UserPreferences.ServerMode = hostOnStartUpCheckBox.Checked; 
		}

		private bool dirty;
		private ConnectOperation operationRequested;
	}
}
