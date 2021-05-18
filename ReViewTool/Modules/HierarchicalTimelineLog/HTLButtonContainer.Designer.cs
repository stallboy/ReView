namespace ReViewTool.Modules.HierarchicalTimelineLog
{
	partial class HTLButtonContainer
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.flowLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.customizeButton = new System.Windows.Forms.Button();
			this.toggleGenericTracks = new System.Windows.Forms.CheckBox();
			this.regexpFilterContainer = new System.Windows.Forms.FlowLayoutPanel();
			this.regexpFilter = new System.Windows.Forms.TextBox();
			this.filterLabel = new System.Windows.Forms.Label();
			this.logFlagFilterFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.flowLayout.SuspendLayout();
			this.regexpFilterContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayout
			// 
			this.flowLayout.AutoSize = true;
			this.flowLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayout.Controls.Add(this.customizeButton);
			this.flowLayout.Controls.Add(this.toggleGenericTracks);
			this.flowLayout.Controls.Add(this.regexpFilterContainer);
			this.flowLayout.Controls.Add(this.logFlagFilterFlowLayout);
			this.flowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayout.Location = new System.Drawing.Point(0, 0);
			this.flowLayout.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayout.Name = "flowLayout";
			this.flowLayout.Size = new System.Drawing.Size(712, 64);
			this.flowLayout.TabIndex = 0;
			// 
			// customizeButton
			// 
			this.customizeButton.BackColor = System.Drawing.Color.White;
			this.customizeButton.FlatAppearance.BorderSize = 0;
			this.customizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.customizeButton.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.customizeButton.ForeColor = System.Drawing.Color.Black;
			this.customizeButton.Location = new System.Drawing.Point(0, 0);
			this.customizeButton.Margin = new System.Windows.Forms.Padding(0);
			this.customizeButton.Name = "customizeButton";
			this.customizeButton.Size = new System.Drawing.Size(72, 64);
			this.customizeButton.TabIndex = 18;
			this.customizeButton.Text = "Customize";
			this.customizeButton.UseVisualStyleBackColor = false;
			this.customizeButton.Click += new System.EventHandler(this.customizeButton_Click);
			// 
			// toggleGenericTracks
			// 
			this.toggleGenericTracks.Appearance = System.Windows.Forms.Appearance.Button;
			this.toggleGenericTracks.BackColor = System.Drawing.Color.White;
			this.toggleGenericTracks.Checked = true;
			this.toggleGenericTracks.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toggleGenericTracks.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.toggleGenericTracks.FlatAppearance.BorderSize = 0;
			this.toggleGenericTracks.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this.toggleGenericTracks.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
			this.toggleGenericTracks.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
			this.toggleGenericTracks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.toggleGenericTracks.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toggleGenericTracks.ForeColor = System.Drawing.Color.Black;
			this.toggleGenericTracks.Location = new System.Drawing.Point(77, 0);
			this.toggleGenericTracks.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.toggleGenericTracks.MinimumSize = new System.Drawing.Size(70, 64);
			this.toggleGenericTracks.Name = "toggleGenericTracks";
			this.toggleGenericTracks.Size = new System.Drawing.Size(70, 64);
			this.toggleGenericTracks.TabIndex = 13;
			this.toggleGenericTracks.Text = "+ Generic tracks";
			this.toggleGenericTracks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toggleGenericTracks.UseVisualStyleBackColor = false;
			// 
			// regexpFilterContainer
			// 
			this.regexpFilterContainer.BackColor = System.Drawing.Color.WhiteSmoke;
			this.regexpFilterContainer.Controls.Add(this.regexpFilter);
			this.regexpFilterContainer.Controls.Add(this.filterLabel);
			this.regexpFilterContainer.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
			this.regexpFilterContainer.Location = new System.Drawing.Point(152, 0);
			this.regexpFilterContainer.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.regexpFilterContainer.MinimumSize = new System.Drawing.Size(144, 64);
			this.regexpFilterContainer.Name = "regexpFilterContainer";
			this.regexpFilterContainer.Size = new System.Drawing.Size(144, 64);
			this.regexpFilterContainer.TabIndex = 14;
			// 
			// regexpFilter
			// 
			this.regexpFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.regexpFilter.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.regexpFilter.Location = new System.Drawing.Point(3, 38);
			this.regexpFilter.Name = "regexpFilter";
			this.regexpFilter.Size = new System.Drawing.Size(136, 23);
			this.regexpFilter.TabIndex = 1;
			// 
			// filterLabel
			// 
			this.filterLabel.AutoSize = true;
			this.filterLabel.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.filterLabel.Location = new System.Drawing.Point(2, 18);
			this.filterLabel.Margin = new System.Windows.Forms.Padding(2);
			this.filterLabel.Name = "filterLabel";
			this.filterLabel.Size = new System.Drawing.Size(36, 15);
			this.filterLabel.TabIndex = 0;
			this.filterLabel.Text = "Filter";
			// 
			// logFlagFilterFlowLayout
			// 
			this.logFlagFilterFlowLayout.AutoSize = true;
			this.logFlagFilterFlowLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.logFlagFilterFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logFlagFilterFlowLayout.Location = new System.Drawing.Point(296, 0);
			this.logFlagFilterFlowLayout.Margin = new System.Windows.Forms.Padding(0);
			this.logFlagFilterFlowLayout.Name = "logFlagFilterFlowLayout";
			this.logFlagFilterFlowLayout.Size = new System.Drawing.Size(0, 64);
			this.logFlagFilterFlowLayout.TabIndex = 15;
			// 
			// HTLButtonContainer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.flowLayout);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "HTLButtonContainer";
			this.Size = new System.Drawing.Size(712, 64);
			this.flowLayout.ResumeLayout(false);
			this.flowLayout.PerformLayout();
			this.regexpFilterContainer.ResumeLayout(false);
			this.regexpFilterContainer.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayout;
		private System.Windows.Forms.CheckBox toggleGenericTracks;
		private System.Windows.Forms.FlowLayoutPanel regexpFilterContainer;
		private System.Windows.Forms.TextBox regexpFilter;
		private System.Windows.Forms.Label filterLabel;
		private System.Windows.Forms.FlowLayoutPanel logFlagFilterFlowLayout;
		private System.Windows.Forms.Button customizeButton;
	}
}
