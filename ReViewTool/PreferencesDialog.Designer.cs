namespace ReViewTool
{
	partial class PreferencesDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesDialog));
			this.saveButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.mainTable = new System.Windows.Forms.TableLayoutPanel();
			this.mainTabControl = new System.Windows.Forms.TabControl();
			this.tabUserCommands = new System.Windows.Forms.TabPage();
			this.userCommandsTable = new System.Windows.Forms.TableLayoutPanel();
			this.userCommandsGridActionFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.addUserCommandButton = new System.Windows.Forms.Button();
			this.removeUserCommandButton = new System.Windows.Forms.Button();
			this.userCommandsGrid = new System.Windows.Forms.DataGridView();
			this.tabSettings = new System.Windows.Forms.TabPage();
			this.settingsMainTable = new System.Windows.Forms.TableLayoutPanel();
			this.builtInBinaryStorageSize = new System.Windows.Forms.TextBox();
			this.builtInBinaryStorageSizeLabel = new System.Windows.Forms.Label();
			this.minItemLengthInMillisLabel = new System.Windows.Forms.Label();
			this.minItemLengthInMillis = new System.Windows.Forms.TextBox();
			this.dialogButtonsFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.mainTable.SuspendLayout();
			this.mainTabControl.SuspendLayout();
			this.tabUserCommands.SuspendLayout();
			this.userCommandsTable.SuspendLayout();
			this.userCommandsGridActionFlowLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.userCommandsGrid)).BeginInit();
			this.tabSettings.SuspendLayout();
			this.settingsMainTable.SuspendLayout();
			this.dialogButtonsFlowLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// saveButton
			// 
			this.saveButton.BackColor = System.Drawing.Color.DodgerBlue;
			this.saveButton.FlatAppearance.BorderSize = 0;
			this.saveButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.saveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.saveButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.saveButton.ForeColor = System.Drawing.Color.White;
			this.saveButton.Location = new System.Drawing.Point(486, 5);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(99, 31);
			this.saveButton.TabIndex = 7;
			this.saveButton.Text = "SAVE";
			this.saveButton.UseVisualStyleBackColor = false;
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.BackColor = System.Drawing.Color.DodgerBlue;
			this.cancelButton.FlatAppearance.BorderSize = 0;
			this.cancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.cancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cancelButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cancelButton.ForeColor = System.Drawing.Color.White;
			this.cancelButton.Location = new System.Drawing.Point(591, 5);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(86, 31);
			this.cancelButton.TabIndex = 8;
			this.cancelButton.Text = "CANCEL";
			this.cancelButton.UseVisualStyleBackColor = false;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// mainTable
			// 
			this.mainTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
			this.mainTable.ColumnCount = 2;
			this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.mainTable.Controls.Add(this.mainTabControl, 0, 0);
			this.mainTable.Controls.Add(this.dialogButtonsFlowLayout, 0, 1);
			this.mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTable.Location = new System.Drawing.Point(0, 0);
			this.mainTable.Margin = new System.Windows.Forms.Padding(0);
			this.mainTable.Name = "mainTable";
			this.mainTable.RowCount = 2;
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.mainTable.Size = new System.Drawing.Size(691, 522);
			this.mainTable.TabIndex = 11;
			// 
			// mainTabControl
			// 
			this.mainTable.SetColumnSpan(this.mainTabControl, 2);
			this.mainTabControl.Controls.Add(this.tabUserCommands);
			this.mainTabControl.Controls.Add(this.tabSettings);
			this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTabControl.Location = new System.Drawing.Point(0, 0);
			this.mainTabControl.Margin = new System.Windows.Forms.Padding(0);
			this.mainTabControl.Name = "mainTabControl";
			this.mainTabControl.SelectedIndex = 0;
			this.mainTabControl.Size = new System.Drawing.Size(691, 472);
			this.mainTabControl.TabIndex = 15;
			// 
			// tabUserCommands
			// 
			this.tabUserCommands.Controls.Add(this.userCommandsTable);
			this.tabUserCommands.Location = new System.Drawing.Point(4, 24);
			this.tabUserCommands.Name = "tabUserCommands";
			this.tabUserCommands.Padding = new System.Windows.Forms.Padding(3);
			this.tabUserCommands.Size = new System.Drawing.Size(683, 444);
			this.tabUserCommands.TabIndex = 1;
			this.tabUserCommands.Text = "User Commands";
			this.tabUserCommands.UseVisualStyleBackColor = true;
			// 
			// userCommandsTable
			// 
			this.userCommandsTable.ColumnCount = 1;
			this.userCommandsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.userCommandsTable.Controls.Add(this.userCommandsGridActionFlowLayout, 0, 1);
			this.userCommandsTable.Controls.Add(this.userCommandsGrid, 0, 0);
			this.userCommandsTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userCommandsTable.Location = new System.Drawing.Point(3, 3);
			this.userCommandsTable.Margin = new System.Windows.Forms.Padding(0);
			this.userCommandsTable.Name = "userCommandsTable";
			this.userCommandsTable.RowCount = 2;
			this.userCommandsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.userCommandsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
			this.userCommandsTable.Size = new System.Drawing.Size(677, 438);
			this.userCommandsTable.TabIndex = 9;
			// 
			// userCommandsGridActionFlowLayout
			// 
			this.userCommandsGridActionFlowLayout.Controls.Add(this.addUserCommandButton);
			this.userCommandsGridActionFlowLayout.Controls.Add(this.removeUserCommandButton);
			this.userCommandsGridActionFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userCommandsGridActionFlowLayout.Location = new System.Drawing.Point(0, 400);
			this.userCommandsGridActionFlowLayout.Margin = new System.Windows.Forms.Padding(0);
			this.userCommandsGridActionFlowLayout.Name = "userCommandsGridActionFlowLayout";
			this.userCommandsGridActionFlowLayout.Size = new System.Drawing.Size(677, 38);
			this.userCommandsGridActionFlowLayout.TabIndex = 10;
			// 
			// addUserCommandButton
			// 
			this.addUserCommandButton.BackColor = System.Drawing.Color.DodgerBlue;
			this.addUserCommandButton.FlatAppearance.BorderSize = 0;
			this.addUserCommandButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.addUserCommandButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this.addUserCommandButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.addUserCommandButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.addUserCommandButton.ForeColor = System.Drawing.Color.White;
			this.addUserCommandButton.Location = new System.Drawing.Point(3, 3);
			this.addUserCommandButton.Name = "addUserCommandButton";
			this.addUserCommandButton.Size = new System.Drawing.Size(99, 31);
			this.addUserCommandButton.TabIndex = 9;
			this.addUserCommandButton.Text = "ADD";
			this.addUserCommandButton.UseVisualStyleBackColor = false;
			this.addUserCommandButton.Click += new System.EventHandler(this.addUserCommandButton_Click);
			// 
			// removeUserCommandButton
			// 
			this.removeUserCommandButton.BackColor = System.Drawing.Color.DodgerBlue;
			this.removeUserCommandButton.FlatAppearance.BorderSize = 0;
			this.removeUserCommandButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.removeUserCommandButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this.removeUserCommandButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.removeUserCommandButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.removeUserCommandButton.ForeColor = System.Drawing.Color.White;
			this.removeUserCommandButton.Location = new System.Drawing.Point(108, 3);
			this.removeUserCommandButton.Name = "removeUserCommandButton";
			this.removeUserCommandButton.Size = new System.Drawing.Size(99, 31);
			this.removeUserCommandButton.TabIndex = 10;
			this.removeUserCommandButton.Text = "REMOVE";
			this.removeUserCommandButton.UseVisualStyleBackColor = false;
			this.removeUserCommandButton.Click += new System.EventHandler(this.removeUserCommandButton_Click);
			// 
			// userCommandsGrid
			// 
			this.userCommandsGrid.AllowUserToAddRows = false;
			this.userCommandsGrid.AllowUserToDeleteRows = false;
			this.userCommandsGrid.AllowUserToResizeRows = false;
			this.userCommandsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.userCommandsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.userCommandsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userCommandsGrid.Location = new System.Drawing.Point(0, 0);
			this.userCommandsGrid.Margin = new System.Windows.Forms.Padding(0);
			this.userCommandsGrid.MultiSelect = false;
			this.userCommandsGrid.Name = "userCommandsGrid";
			this.userCommandsGrid.RowHeadersVisible = false;
			this.userCommandsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.userCommandsGrid.Size = new System.Drawing.Size(677, 400);
			this.userCommandsGrid.TabIndex = 15;
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.settingsMainTable);
			this.tabSettings.Location = new System.Drawing.Point(4, 24);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
			this.tabSettings.Size = new System.Drawing.Size(683, 444);
			this.tabSettings.TabIndex = 2;
			this.tabSettings.Text = "Settings";
			this.tabSettings.UseVisualStyleBackColor = true;
			// 
			// settingsMainTable
			// 
			this.settingsMainTable.ColumnCount = 3;
			this.settingsMainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.settingsMainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.settingsMainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.settingsMainTable.Controls.Add(this.builtInBinaryStorageSize, 1, 1);
			this.settingsMainTable.Controls.Add(this.builtInBinaryStorageSizeLabel, 0, 1);
			this.settingsMainTable.Controls.Add(this.minItemLengthInMillisLabel, 0, 0);
			this.settingsMainTable.Controls.Add(this.minItemLengthInMillis, 1, 0);
			this.settingsMainTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.settingsMainTable.Location = new System.Drawing.Point(3, 3);
			this.settingsMainTable.Margin = new System.Windows.Forms.Padding(0);
			this.settingsMainTable.Name = "settingsMainTable";
			this.settingsMainTable.RowCount = 3;
			this.settingsMainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.settingsMainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.settingsMainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.settingsMainTable.Size = new System.Drawing.Size(677, 438);
			this.settingsMainTable.TabIndex = 0;
			// 
			// builtInBinaryStorageSize
			// 
			this.builtInBinaryStorageSize.Location = new System.Drawing.Point(193, 32);
			this.builtInBinaryStorageSize.Name = "builtInBinaryStorageSize";
			this.builtInBinaryStorageSize.Size = new System.Drawing.Size(100, 23);
			this.builtInBinaryStorageSize.TabIndex = 3;
			this.builtInBinaryStorageSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.builtInBinaryStorageSize_KeyDown);
			this.builtInBinaryStorageSize.Leave += new System.EventHandler(this.builtInBinaryStorageSize_Leave);
			// 
			// builtInBinaryStorageSizeLabel
			// 
			this.builtInBinaryStorageSizeLabel.AutoSize = true;
			this.builtInBinaryStorageSizeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.builtInBinaryStorageSizeLabel.Location = new System.Drawing.Point(3, 29);
			this.builtInBinaryStorageSizeLabel.Name = "builtInBinaryStorageSizeLabel";
			this.builtInBinaryStorageSizeLabel.Size = new System.Drawing.Size(184, 29);
			this.builtInBinaryStorageSizeLabel.TabIndex = 2;
			this.builtInBinaryStorageSizeLabel.Text = "Built-in binary storage size (MB)";
			this.builtInBinaryStorageSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// minItemLengthInMillisLabel
			// 
			this.minItemLengthInMillisLabel.AutoSize = true;
			this.minItemLengthInMillisLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.minItemLengthInMillisLabel.Location = new System.Drawing.Point(3, 0);
			this.minItemLengthInMillisLabel.Name = "minItemLengthInMillisLabel";
			this.minItemLengthInMillisLabel.Size = new System.Drawing.Size(184, 29);
			this.minItemLengthInMillisLabel.TabIndex = 0;
			this.minItemLengthInMillisLabel.Text = "Min item length in milliseconds";
			this.minItemLengthInMillisLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// minItemLengthInMillis
			// 
			this.minItemLengthInMillis.Location = new System.Drawing.Point(193, 3);
			this.minItemLengthInMillis.Name = "minItemLengthInMillis";
			this.minItemLengthInMillis.Size = new System.Drawing.Size(100, 23);
			this.minItemLengthInMillis.TabIndex = 1;
			this.minItemLengthInMillis.KeyDown += new System.Windows.Forms.KeyEventHandler(this.minItemLengthInMillis_KeyDown);
			this.minItemLengthInMillis.Leave += new System.EventHandler(this.minItemLengthInMillis_Leave);
			// 
			// dialogButtonsFlowLayout
			// 
			this.mainTable.SetColumnSpan(this.dialogButtonsFlowLayout, 2);
			this.dialogButtonsFlowLayout.Controls.Add(this.cancelButton);
			this.dialogButtonsFlowLayout.Controls.Add(this.saveButton);
			this.dialogButtonsFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dialogButtonsFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.dialogButtonsFlowLayout.Location = new System.Drawing.Point(3, 475);
			this.dialogButtonsFlowLayout.Name = "dialogButtonsFlowLayout";
			this.dialogButtonsFlowLayout.Padding = new System.Windows.Forms.Padding(0, 2, 5, 0);
			this.dialogButtonsFlowLayout.Size = new System.Drawing.Size(685, 44);
			this.dialogButtonsFlowLayout.TabIndex = 15;
			this.dialogButtonsFlowLayout.WrapContents = false;
			// 
			// PreferencesDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(691, 522);
			this.Controls.Add(this.mainTable);
			this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximumSize = new System.Drawing.Size(1195, 886);
			this.MinimumSize = new System.Drawing.Size(583, 346);
			this.Name = "PreferencesDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preferences";
			this.mainTable.ResumeLayout(false);
			this.mainTabControl.ResumeLayout(false);
			this.tabUserCommands.ResumeLayout(false);
			this.userCommandsTable.ResumeLayout(false);
			this.userCommandsGridActionFlowLayout.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.userCommandsGrid)).EndInit();
			this.tabSettings.ResumeLayout(false);
			this.settingsMainTable.ResumeLayout(false);
			this.settingsMainTable.PerformLayout();
			this.dialogButtonsFlowLayout.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TableLayoutPanel mainTable;
		private System.Windows.Forms.TabControl mainTabControl;
		private System.Windows.Forms.TabPage tabUserCommands;
		private System.Windows.Forms.TabPage tabSettings;
		private System.Windows.Forms.FlowLayoutPanel dialogButtonsFlowLayout;
		private System.Windows.Forms.DataGridView userCommandsGrid;
		private System.Windows.Forms.TableLayoutPanel userCommandsTable;
		private System.Windows.Forms.FlowLayoutPanel userCommandsGridActionFlowLayout;
		private System.Windows.Forms.Button addUserCommandButton;
		private System.Windows.Forms.Button removeUserCommandButton;
		private System.Windows.Forms.TableLayoutPanel settingsMainTable;
		private System.Windows.Forms.Label minItemLengthInMillisLabel;
		private System.Windows.Forms.TextBox minItemLengthInMillis;
		private System.Windows.Forms.TextBox builtInBinaryStorageSize;
		private System.Windows.Forms.Label builtInBinaryStorageSizeLabel;
	}
}