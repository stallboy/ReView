using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReViewTool.Modules.HierarchicalTimelineLog
{
	public partial class HTLCustomizeDialog : Form
	{
		public HTLCustomizeDialog()
		{
			InitializeComponent();

			SetupLogFlagColorGrid();

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

		private void SetupLogFlagColorGrid()
		{
			// Setup log flag color grid
			this.logFlagColorGrid.AutoGenerateColumns = false;
			this.logFlagColorGrid.AutoSize = true;
			this.logFlagColorGrid.DataSource = UserPreferencesManager.Instance.UserPreferences.LogFlagColors;
			this.logFlagColorGrid.CellContentClick += logFlagColorGrid_CellContentClick;

			// Bit index
			DataGridViewColumn column = new DataGridViewTextBoxColumn();
			column.DataPropertyName = "LogFlagIndex";
			column.Name = "Flag bit (0 = no bits set)";
			column.ReadOnly = true;
			this.logFlagColorGrid.Columns.Add(column);

			// Name
			column = new DataGridViewTextBoxColumn();
			column.DataPropertyName = "Name";
			column.Name = "Name";
			this.logFlagColorGrid.Columns.Add(column);

			// Initialize and add a check box column.
			column = new DataGridViewColorPickerColumn();
			column.DataPropertyName = "Color";
			column.Name = "Color";
			this.logFlagColorGrid.Columns.Add(column);

			// Display filter button
			column = new DataGridViewCheckBoxColumn();
			column.DataPropertyName = "DisplayFilterButton";
			column.Name = "Display filter button";
			this.logFlagColorGrid.Columns.Add(column);
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
