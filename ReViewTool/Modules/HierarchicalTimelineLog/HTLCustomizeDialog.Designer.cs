namespace ReViewTool.Modules.HierarchicalTimelineLog
{
	partial class HTLCustomizeDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HTLCustomizeDialog));
			this.saveButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.mainTable = new System.Windows.Forms.TableLayoutPanel();
			this.logFlagColorGrid = new System.Windows.Forms.DataGridView();
			this.dialogButtonsFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.mainTable.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.logFlagColorGrid)).BeginInit();
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
			this.mainTable.ColumnCount = 1;
			this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTable.Controls.Add(this.logFlagColorGrid, 0, 0);
			this.mainTable.Controls.Add(this.dialogButtonsFlowLayout, 0, 1);
			this.mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTable.Location = new System.Drawing.Point(0, 0);
			this.mainTable.Margin = new System.Windows.Forms.Padding(0);
			this.mainTable.Name = "mainTable";
			this.mainTable.RowCount = 2;
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.mainTable.Size = new System.Drawing.Size(691, 522);
			this.mainTable.TabIndex = 11;
			// 
			// logFlagColorGrid
			// 
			this.logFlagColorGrid.AllowUserToAddRows = false;
			this.logFlagColorGrid.AllowUserToDeleteRows = false;
			this.logFlagColorGrid.AllowUserToResizeRows = false;
			this.logFlagColorGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.logFlagColorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.logFlagColorGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logFlagColorGrid.Location = new System.Drawing.Point(0, 0);
			this.logFlagColorGrid.Margin = new System.Windows.Forms.Padding(0);
			this.logFlagColorGrid.Name = "logFlagColorGrid";
			this.logFlagColorGrid.RowHeadersVisible = false;
			this.logFlagColorGrid.Size = new System.Drawing.Size(691, 472);
			this.logFlagColorGrid.TabIndex = 14;
			// 
			// dialogButtonsFlowLayout
			// 
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
			// HTLCustomizeDialog
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
			this.Name = "HTLCustomizeDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Customize Hierarchical Timeline Log";
			this.mainTable.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.logFlagColorGrid)).EndInit();
			this.dialogButtonsFlowLayout.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TableLayoutPanel mainTable;
		private System.Windows.Forms.FlowLayoutPanel dialogButtonsFlowLayout;
		private System.Windows.Forms.DataGridView logFlagColorGrid;
	}
}