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
	public partial class PreferencesDialog : Form
	{
		public PreferencesDialog()
		{
			InitializeComponent();

			SetupUserCommandsGrid();

			SetupSettings();

			UserPreferencesManager.Instance.UserPreferences.PropertyChanged += OnUserPreferencesChanged;

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

		private void SetDirty(bool dirty)
		{
			saveButton.Enabled = dirty;
			this.dirty = dirty;
		}

		private void SetupUserCommandsGrid()
		{
			// Setup user commands grid
			this.userCommandsGrid.AutoGenerateColumns = false;
			this.userCommandsGrid.AutoSize = true;
			this.userCommandsGrid.DataSource = UserPreferencesManager.Instance.UserPreferences.UserCommands;
			this.userCommandsGrid.CellContentClick += userCommandsGrid_CellContentClick;

			// Name
			DataGridViewColumn column = new DataGridViewTextBoxColumn();
			column.DataPropertyName = "Name";
			column.Name = "Command Name";
			this.userCommandsGrid.Columns.Add(column);

			// Command
			column = new DataGridViewTextBoxColumn();
			column.DataPropertyName = "Command";
			column.Name = "Command";
			this.userCommandsGrid.Columns.Add(column);

			// Color
			column = new DataGridViewColorPickerColumn();
			column.DataPropertyName = "Color";
			column.Name = "Button color";
			this.userCommandsGrid.Columns.Add(column);

			// Toggle
			column = new DataGridViewCheckBoxColumn();
			column.DataPropertyName = "IsToggle";
			column.Name = "Toggle";
			this.userCommandsGrid.Columns.Add(column);

			// Enabled
			column = new DataGridViewCheckBoxColumn();
			column.DataPropertyName = "IsEnabled";
			column.Name = "Enabled";
			this.userCommandsGrid.Columns.Add(column);
		}

		private void SetupSettings()
		{
			minItemLengthInMillis.Text = "" + UserPreferencesManager.Instance.UserPreferences.MinItemLengthInMillis;
			builtInBinaryStorageSize.Text = "" + UserPreferencesManager.Instance.UserPreferences.BuiltInBinaryStorageMegaBytes;
		}

		private void minItemLengthInMillis_Leave(object sender, EventArgs e)
		{
			UpdateMinItemLengthInMillis();
		}

		private void minItemLengthInMillis_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				UpdateMinItemLengthInMillis();
				e.Handled = true;
			}
		}

		private void builtInBinaryStorageSize_Leave(object sender, EventArgs e)
		{
			UpdateBuiltInBinaryStorageSize();
		}

		private void builtInBinaryStorageSize_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				UpdateBuiltInBinaryStorageSize();
				e.Handled = true;
			}
		}

		private void UpdateMinItemLengthInMillis()
		{
			int newMinItemLength = -1;
			if (Int32.TryParse(minItemLengthInMillis.Text, out newMinItemLength))
			{
				UserPreferencesManager.Instance.UserPreferences.MinItemLengthInMillis = newMinItemLength;
			}
			else
			{
				minItemLengthInMillis.Text = "" + UserPreferencesManager.Instance.UserPreferences.MinItemLengthInMillis;
			}
		}

		private void UpdateBuiltInBinaryStorageSize()
		{
			int newBinaryStorageSize = -1;
			if (Int32.TryParse(builtInBinaryStorageSize.Text, out newBinaryStorageSize))
			{
				UserPreferencesManager.Instance.UserPreferences.BuiltInBinaryStorageMegaBytes = newBinaryStorageSize;
			}
			else
			{
				builtInBinaryStorageSize.Text = "" + UserPreferencesManager.Instance.UserPreferences.BuiltInBinaryStorageMegaBytes;
			}
		}

		private void userCommandsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 2 && e.RowIndex >= 0 && e.RowIndex < UserPreferencesManager.Instance.UserPreferences.LogFlagColors.Count)
			{
				UserCommand command = UserPreferencesManager.Instance.UserPreferences.UserCommands[e.RowIndex] as UserCommand;
				ColorDialog dlg = new ColorDialog();
				dlg.FullOpen = true;
				dlg.Color = command.Color;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					command.Color = dlg.Color;
				}
			}
		}

		private void logFlagColorGrid_CellContentClick(object sender, DataGridViewCellEventArgs e) 
		{
			if (e.ColumnIndex == 2 && e.RowIndex >= 0 && e.RowIndex < UserPreferencesManager.Instance.UserPreferences.LogFlagColors.Count)
			{
				LogFlagColor lfc = UserPreferencesManager.Instance.UserPreferences.LogFlagColors[e.RowIndex] as LogFlagColor;
				ColorDialog dlg = new ColorDialog();
				dlg.FullOpen = true;
				dlg.Color = lfc.Color;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					lfc.Color = dlg.Color;
				}
			}
		}

		private void addUserCommandButton_Click(object sender, EventArgs e)
		{
			UserPreferencesManager.Instance.UserPreferences.UserCommands.Add(new UserCommand());
		}

		private void removeUserCommandButton_Click(object sender, EventArgs e)
		{
			if (userCommandsGrid.SelectedRows.Count == 1)
			{
				UserPreferencesManager.Instance.UserPreferences.UserCommands.RemoveAt(userCommandsGrid.SelectedRows[0].Index);
			}
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			UserPreferencesManager.Instance.Load();
			DialogResult = DialogResult.Cancel;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			UserPreferencesManager.Instance.Save();
			SetDirty(false);
			DialogResult = DialogResult.OK;
		}

		private bool dirty;
	}
}
