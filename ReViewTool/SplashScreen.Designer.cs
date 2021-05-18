namespace ReViewTool
{
	partial class SplashScreen
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
			this.mainTable = new System.Windows.Forms.TableLayoutPanel();
			this.tbCopyrightNotice = new System.Windows.Forms.TextBox();
			this.lblTitle = new System.Windows.Forms.Label();
			this.tbDisclaimer = new System.Windows.Forms.TextBox();
			this.lblVersion = new System.Windows.Forms.Label();
			this.mainTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainTable
			// 
			this.mainTable.ColumnCount = 1;
			this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.mainTable.Controls.Add(this.tbDisclaimer, 0, 4);
			this.mainTable.Controls.Add(this.tbCopyrightNotice, 0, 3);
			this.mainTable.Controls.Add(this.lblTitle, 0, 0);
			this.mainTable.Controls.Add(this.lblVersion, 0, 1);
			this.mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTable.Location = new System.Drawing.Point(0, 0);
			this.mainTable.Margin = new System.Windows.Forms.Padding(0);
			this.mainTable.Name = "mainTable";
			this.mainTable.RowCount = 5;
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainTable.Size = new System.Drawing.Size(364, 473);
			this.mainTable.TabIndex = 1;
			// 
			// tbCopyrightNotice
			// 
			this.tbCopyrightNotice.BackColor = System.Drawing.Color.DimGray;
			this.tbCopyrightNotice.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbCopyrightNotice.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbCopyrightNotice.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbCopyrightNotice.ForeColor = System.Drawing.Color.White;
			this.tbCopyrightNotice.Location = new System.Drawing.Point(0, 180);
			this.tbCopyrightNotice.Margin = new System.Windows.Forms.Padding(0);
			this.tbCopyrightNotice.Multiline = true;
			this.tbCopyrightNotice.Name = "tbCopyrightNotice";
			this.tbCopyrightNotice.ReadOnly = true;
			this.tbCopyrightNotice.ShortcutsEnabled = false;
			this.tbCopyrightNotice.Size = new System.Drawing.Size(364, 175);
			this.tbCopyrightNotice.TabIndex = 1;
			this.tbCopyrightNotice.TabStop = false;
			this.tbCopyrightNotice.Text = resources.GetString("tbCopyrightNotice.Text");
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblTitle.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTitle.ForeColor = System.Drawing.Color.White;
			this.lblTitle.Image = global::ReViewTool.Properties.Resources.flowicon32x32_transparent;
			this.lblTitle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblTitle.Location = new System.Drawing.Point(3, 0);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(358, 40);
			this.lblTitle.TabIndex = 3;
			this.lblTitle.Text = "     ReView";
			this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbDisclaimer
			// 
			this.tbDisclaimer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.tbDisclaimer.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbDisclaimer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbDisclaimer.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbDisclaimer.ForeColor = System.Drawing.Color.White;
			this.tbDisclaimer.Location = new System.Drawing.Point(0, 355);
			this.tbDisclaimer.Margin = new System.Windows.Forms.Padding(0);
			this.tbDisclaimer.Multiline = true;
			this.tbDisclaimer.Name = "tbDisclaimer";
			this.tbDisclaimer.ReadOnly = true;
			this.tbDisclaimer.ShortcutsEnabled = false;
			this.tbDisclaimer.Size = new System.Drawing.Size(364, 118);
			this.tbDisclaimer.TabIndex = 0;
			this.tbDisclaimer.TabStop = false;
			this.tbDisclaimer.Text = resources.GetString("tbDisclaimer.Text");
			// 
			// lblVersion
			// 
			this.lblVersion.AutoSize = true;
			this.lblVersion.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblVersion.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblVersion.ForeColor = System.Drawing.Color.White;
			this.lblVersion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblVersion.Location = new System.Drawing.Point(3, 40);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(358, 20);
			this.lblVersion.TabIndex = 4;
			this.lblVersion.Text = "VERSION 1.0";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SplashScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.ClientSize = new System.Drawing.Size(364, 473);
			this.Controls.Add(this.mainTable);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SplashScreen";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SplashScreen";
			this.mainTable.ResumeLayout(false);
			this.mainTable.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel mainTable;
		private System.Windows.Forms.TextBox tbCopyrightNotice;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.TextBox tbDisclaimer;
		private System.Windows.Forms.Label lblVersion;
	}
}