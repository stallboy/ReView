namespace ReViewTool
{
	partial class ConnectDialog
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectDialog));
			this.connectDialogTooltip = new System.Windows.Forms.ToolTip(this.components);
			this.hostTab = new System.Windows.Forms.TabPage();
			this.hostManagementTable = new System.Windows.Forms.TableLayoutPanel();
			this.hostOnStartUpCheckBox = new System.Windows.Forms.CheckBox();
			this.dialogButtonsFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.applyHostSettingsButton = new System.Windows.Forms.Button();
			this.cancelHostButton = new System.Windows.Forms.Button();
			this.listenPortLabel = new System.Windows.Forms.Label();
			this.listenPort = new System.Windows.Forms.TextBox();
			this.hostOnStartUpLabel = new System.Windows.Forms.Label();
			this.descriptionHostLabel = new System.Windows.Forms.Label();
			this.hostButton = new System.Windows.Forms.Button();
			this.mainTabControl = new System.Windows.Forms.TabControl();
			this.hostTab.SuspendLayout();
			this.hostManagementTable.SuspendLayout();
			this.dialogButtonsFlowLayout.SuspendLayout();
			this.mainTabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// hostTab
			// 
			this.hostTab.Controls.Add(this.hostManagementTable);
			this.hostTab.Location = new System.Drawing.Point(4, 22);
			this.hostTab.Margin = new System.Windows.Forms.Padding(0);
			this.hostTab.Name = "hostTab";
			this.hostTab.Size = new System.Drawing.Size(582, 341);
			this.hostTab.TabIndex = 1;
			this.hostTab.Text = "Host";
			this.hostTab.UseVisualStyleBackColor = true;
			// 
			// hostManagementTable
			// 
			this.hostManagementTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
			this.hostManagementTable.ColumnCount = 3;
			this.hostManagementTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.hostManagementTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.hostManagementTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.hostManagementTable.Controls.Add(this.hostButton, 2, 1);
			this.hostManagementTable.Controls.Add(this.descriptionHostLabel, 0, 0);
			this.hostManagementTable.Controls.Add(this.hostOnStartUpLabel, 0, 2);
			this.hostManagementTable.Controls.Add(this.listenPort, 1, 1);
			this.hostManagementTable.Controls.Add(this.listenPortLabel, 0, 1);
			this.hostManagementTable.Controls.Add(this.dialogButtonsFlowLayout, 0, 4);
			this.hostManagementTable.Controls.Add(this.hostOnStartUpCheckBox, 1, 2);
			this.hostManagementTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.hostManagementTable.Location = new System.Drawing.Point(0, 0);
			this.hostManagementTable.Margin = new System.Windows.Forms.Padding(0);
			this.hostManagementTable.Name = "hostManagementTable";
			this.hostManagementTable.RowCount = 5;
			this.hostManagementTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.hostManagementTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.hostManagementTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.hostManagementTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.hostManagementTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.hostManagementTable.Size = new System.Drawing.Size(582, 341);
			this.hostManagementTable.TabIndex = 12;
			// 
			// hostOnStartUpCheckBox
			// 
			this.hostOnStartUpCheckBox.AutoSize = true;
			this.hostOnStartUpCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
			this.hostOnStartUpCheckBox.Location = new System.Drawing.Point(116, 85);
			this.hostOnStartUpCheckBox.Name = "hostOnStartUpCheckBox";
			this.hostOnStartUpCheckBox.Size = new System.Drawing.Size(15, 26);
			this.hostOnStartUpCheckBox.TabIndex = 2;
			this.connectDialogTooltip.SetToolTip(this.hostOnStartUpCheckBox, "Start hosting automatically on program start-up");
			this.hostOnStartUpCheckBox.UseVisualStyleBackColor = true;
			this.hostOnStartUpCheckBox.CheckedChanged += new System.EventHandler(this.hostOnStartUpCheckBox_CheckedChanged);
			// 
			// dialogButtonsFlowLayout
			// 
			this.hostManagementTable.SetColumnSpan(this.dialogButtonsFlowLayout, 3);
			this.dialogButtonsFlowLayout.Controls.Add(this.cancelHostButton);
			this.dialogButtonsFlowLayout.Controls.Add(this.applyHostSettingsButton);
			this.dialogButtonsFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dialogButtonsFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.dialogButtonsFlowLayout.Location = new System.Drawing.Point(3, 294);
			this.dialogButtonsFlowLayout.Name = "dialogButtonsFlowLayout";
			this.dialogButtonsFlowLayout.Padding = new System.Windows.Forms.Padding(0, 4, 5, 0);
			this.dialogButtonsFlowLayout.Size = new System.Drawing.Size(576, 44);
			this.dialogButtonsFlowLayout.TabIndex = 13;
			// 
			// applyHostSettingsButton
			// 
			this.applyHostSettingsButton.BackColor = System.Drawing.Color.DodgerBlue;
			this.applyHostSettingsButton.FlatAppearance.BorderSize = 0;
			this.applyHostSettingsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.applyHostSettingsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this.applyHostSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.applyHostSettingsButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.applyHostSettingsButton.ForeColor = System.Drawing.Color.White;
			this.applyHostSettingsButton.Location = new System.Drawing.Point(414, 7);
			this.applyHostSettingsButton.Name = "applyHostSettingsButton";
			this.applyHostSettingsButton.Size = new System.Drawing.Size(74, 26);
			this.applyHostSettingsButton.TabIndex = 4;
			this.applyHostSettingsButton.Text = "APPLY";
			this.connectDialogTooltip.SetToolTip(this.applyHostSettingsButton, "Save all changes made");
			this.applyHostSettingsButton.UseVisualStyleBackColor = false;
			this.applyHostSettingsButton.Click += new System.EventHandler(this.applyHostSettingsButton_Click);
			// 
			// cancelHostButton
			// 
			this.cancelHostButton.BackColor = System.Drawing.Color.DodgerBlue;
			this.cancelHostButton.FlatAppearance.BorderSize = 0;
			this.cancelHostButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.cancelHostButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this.cancelHostButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cancelHostButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cancelHostButton.ForeColor = System.Drawing.Color.White;
			this.cancelHostButton.Location = new System.Drawing.Point(494, 7);
			this.cancelHostButton.Name = "cancelHostButton";
			this.cancelHostButton.Size = new System.Drawing.Size(74, 26);
			this.cancelHostButton.TabIndex = 5;
			this.cancelHostButton.Text = "CANCEL";
			this.connectDialogTooltip.SetToolTip(this.cancelHostButton, "Cancel hosting, also do not save any changes");
			this.cancelHostButton.UseVisualStyleBackColor = false;
			this.cancelHostButton.Click += new System.EventHandler(this.cancelHostButton_Click);
			// 
			// listenPortLabel
			// 
			this.listenPortLabel.AutoSize = true;
			this.listenPortLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.listenPortLabel.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listenPortLabel.ForeColor = System.Drawing.Color.Black;
			this.listenPortLabel.Location = new System.Drawing.Point(3, 50);
			this.listenPortLabel.Name = "listenPortLabel";
			this.listenPortLabel.Size = new System.Drawing.Size(74, 32);
			this.listenPortLabel.TabIndex = 8;
			this.listenPortLabel.Text = "Listen Port";
			this.listenPortLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.connectDialogTooltip.SetToolTip(this.listenPortLabel, "Storage server listen port");
			// 
			// listenPort
			// 
			this.listenPort.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listenPort.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listenPort.Location = new System.Drawing.Point(116, 53);
			this.listenPort.MaxLength = 32;
			this.listenPort.Name = "listenPort";
			this.listenPort.Size = new System.Drawing.Size(153, 26);
			this.listenPort.TabIndex = 1;
			this.connectDialogTooltip.SetToolTip(this.listenPort, "Storage server listen port");
			this.listenPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listenPort_KeyDown);
			this.listenPort.Leave += new System.EventHandler(this.listenPort_LostFocus);
			// 
			// hostOnStartUpLabel
			// 
			this.hostOnStartUpLabel.AutoSize = true;
			this.hostOnStartUpLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.hostOnStartUpLabel.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.hostOnStartUpLabel.ForeColor = System.Drawing.Color.Black;
			this.hostOnStartUpLabel.Location = new System.Drawing.Point(3, 82);
			this.hostOnStartUpLabel.Name = "hostOnStartUpLabel";
			this.hostOnStartUpLabel.Size = new System.Drawing.Size(107, 32);
			this.hostOnStartUpLabel.TabIndex = 14;
			this.hostOnStartUpLabel.Text = "Host on start-up";
			this.hostOnStartUpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.connectDialogTooltip.SetToolTip(this.hostOnStartUpLabel, "Start hosting automatically on program start-up");
			// 
			// descriptionHostLabel
			// 
			this.hostManagementTable.SetColumnSpan(this.descriptionHostLabel, 3);
			this.descriptionHostLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.descriptionHostLabel.Location = new System.Drawing.Point(3, 0);
			this.descriptionHostLabel.Name = "descriptionHostLabel";
			this.descriptionHostLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.descriptionHostLabel.Size = new System.Drawing.Size(576, 50);
			this.descriptionHostLabel.TabIndex = 9;
			this.descriptionHostLabel.Text = "Here you can define host settings for integrated storage server. If you are not r" +
    "unning separate storage server use this to start and connect to integrated stora" +
    "ge server.";
			// 
			// hostButton
			// 
			this.hostButton.BackColor = System.Drawing.Color.DodgerBlue;
			this.hostButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.hostButton.FlatAppearance.BorderSize = 0;
			this.hostButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.hostButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
			this.hostButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.hostButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.hostButton.ForeColor = System.Drawing.Color.White;
			this.hostButton.Location = new System.Drawing.Point(497, 53);
			this.hostButton.Margin = new System.Windows.Forms.Padding(3, 3, 12, 3);
			this.hostButton.Name = "hostButton";
			this.hostManagementTable.SetRowSpan(this.hostButton, 2);
			this.hostButton.Size = new System.Drawing.Size(73, 58);
			this.hostButton.TabIndex = 3;
			this.hostButton.Text = "HOST";
			this.connectDialogTooltip.SetToolTip(this.hostButton, "Host storage server");
			this.hostButton.UseVisualStyleBackColor = false;
			this.hostButton.Click += new System.EventHandler(this.hostButton_Click);
			// 
			// mainTabControl
			// 
			this.mainTabControl.Controls.Add(this.hostTab);
			this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTabControl.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mainTabControl.ItemSize = new System.Drawing.Size(63, 18);
			this.mainTabControl.Location = new System.Drawing.Point(0, 0);
			this.mainTabControl.Margin = new System.Windows.Forms.Padding(0);
			this.mainTabControl.Name = "mainTabControl";
			this.mainTabControl.SelectedIndex = 0;
			this.mainTabControl.Size = new System.Drawing.Size(590, 367);
			this.mainTabControl.TabIndex = 0;
			// 
			// ConnectDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(590, 367);
			this.Controls.Add(this.mainTabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximumSize = new System.Drawing.Size(1024, 768);
			this.MinimumSize = new System.Drawing.Size(595, 400);
			this.Name = "ConnectDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Connection Settings";
			this.hostTab.ResumeLayout(false);
			this.hostManagementTable.ResumeLayout(false);
			this.hostManagementTable.PerformLayout();
			this.dialogButtonsFlowLayout.ResumeLayout(false);
			this.mainTabControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip connectDialogTooltip;
		private System.Windows.Forms.TabPage hostTab;
		private System.Windows.Forms.TableLayoutPanel hostManagementTable;
		private System.Windows.Forms.Button hostButton;
		private System.Windows.Forms.Label descriptionHostLabel;
		private System.Windows.Forms.Label hostOnStartUpLabel;
		private System.Windows.Forms.TextBox listenPort;
		private System.Windows.Forms.Label listenPortLabel;
		private System.Windows.Forms.FlowLayoutPanel dialogButtonsFlowLayout;
		private System.Windows.Forms.Button cancelHostButton;
		private System.Windows.Forms.Button applyHostSettingsButton;
		private System.Windows.Forms.CheckBox hostOnStartUpCheckBox;
		private System.Windows.Forms.TabControl mainTabControl;
	}
}