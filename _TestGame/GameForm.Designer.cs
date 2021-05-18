using _TestGame.TestGame;
namespace _TestGame
{
	partial class GameForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
			this.mainTableContainer = new System.Windows.Forms.TableLayoutPanel();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.resetButton = new System.Windows.Forms.ToolStripButton();
			this.playToggle = new System.Windows.Forms.ToolStripButton();
			this.debugToggle = new System.Windows.Forms.ToolStripButton();
			this.gamePanel = new _TestGame.TestGame.GamePanel();
			this.mainTableContainer.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainTableContainer
			// 
			this.mainTableContainer.ColumnCount = 1;
			this.mainTableContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTableContainer.Controls.Add(this.toolStrip, 0, 0);
			this.mainTableContainer.Controls.Add(this.gamePanel, 0, 1);
			this.mainTableContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTableContainer.Location = new System.Drawing.Point(0, 0);
			this.mainTableContainer.Margin = new System.Windows.Forms.Padding(0);
			this.mainTableContainer.Name = "mainTableContainer";
			this.mainTableContainer.RowCount = 2;
			this.mainTableContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainTableContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.mainTableContainer.Size = new System.Drawing.Size(1024, 537);
			this.mainTableContainer.TabIndex = 0;
			// 
			// toolStrip
			// 
			this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetButton,
            this.playToggle,
            this.debugToggle});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(1024, 25);
			this.toolStrip.TabIndex = 0;
			this.toolStrip.Text = "Tool Strip";
			// 
			// resetButton
			// 
			this.resetButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.resetButton.Image = ((System.Drawing.Image)(resources.GetObject("resetButton.Image")));
			this.resetButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.resetButton.Name = "resetButton";
			this.resetButton.Size = new System.Drawing.Size(39, 22);
			this.resetButton.Text = "Reset";
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// playToggle
			// 
			this.playToggle.CheckOnClick = true;
			this.playToggle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.playToggle.Image = ((System.Drawing.Image)(resources.GetObject("playToggle.Image")));
			this.playToggle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.playToggle.Name = "playToggle";
			this.playToggle.Size = new System.Drawing.Size(33, 22);
			this.playToggle.Text = "Play";
			this.playToggle.CheckedChanged += new System.EventHandler(this.playToggle_CheckedChanged);
			// 
			// debugToggle
			// 
			this.debugToggle.CheckOnClick = true;
			this.debugToggle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.debugToggle.Image = ((System.Drawing.Image)(resources.GetObject("debugToggle.Image")));
			this.debugToggle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.debugToggle.Name = "debugToggle";
			this.debugToggle.Size = new System.Drawing.Size(46, 22);
			this.debugToggle.Text = "Debug";
			this.debugToggle.CheckedChanged += new System.EventHandler(this.debugToggle_CheckedChanged);
			// 
			// gamePanel
			// 
			this.gamePanel.BackColor = System.Drawing.Color.White;
			this.gamePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gamePanel.Game = null;
			this.gamePanel.Location = new System.Drawing.Point(0, 25);
			this.gamePanel.Margin = new System.Windows.Forms.Padding(0);
			this.gamePanel.Name = "gamePanel";
			this.gamePanel.Size = new System.Drawing.Size(1024, 512);
			this.gamePanel.TabIndex = 1;
			// 
			// GameForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1024, 537);
			this.Controls.Add(this.mainTableContainer);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "GameForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "_TestGame";
			this.mainTableContainer.ResumeLayout(false);
			this.mainTableContainer.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel mainTableContainer;
		private GamePanel gamePanel;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton playToggle;
		private System.Windows.Forms.ToolStripButton debugToggle;
		private System.Windows.Forms.ToolStripButton resetButton;

	}
}