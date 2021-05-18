using ReView;

namespace ReViewTool.Modules.HierarchicalTimelineLog
{
	partial class HierarhicalTimelineLog
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SplitContainer = new System.Windows.Forms.SplitContainer();
			this.viewportContainer = new ReView.ViewportContainer();
			this.ReViewOverviewControl = new ReView.ReViewOverviewControl();
			this.tracker = new ReView.SequencerControl();
			this.rtfDummy = new System.Windows.Forms.RichTextBox();
			this.logTextBox = new System.Windows.Forms.RichTextBox();
			this.MainTableLayout = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
			this.SplitContainer.Panel1.SuspendLayout();
			this.SplitContainer.Panel2.SuspendLayout();
			this.SplitContainer.SuspendLayout();
			this.viewportContainer.SuspendLayout();
			this.MainTableLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// SplitContainer
			// 
			this.SplitContainer.BackColor = System.Drawing.Color.Silver;
			this.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SplitContainer.Location = new System.Drawing.Point(0, 0);
			this.SplitContainer.Margin = new System.Windows.Forms.Padding(0);
			this.SplitContainer.Name = "SplitContainer";
			this.SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// SplitContainer.Panel1
			// 
			this.SplitContainer.Panel1.BackColor = System.Drawing.Color.Transparent;
			this.SplitContainer.Panel1.Controls.Add(this.viewportContainer);
			// 
			// SplitContainer.Panel2
			// 
			this.SplitContainer.Panel2.BackColor = System.Drawing.Color.Silver;
			this.SplitContainer.Panel2.Controls.Add(this.rtfDummy);
			this.SplitContainer.Panel2.Controls.Add(this.logTextBox);
			this.SplitContainer.Size = new System.Drawing.Size(924, 572);
			this.SplitContainer.SplitterDistance = 458;
			this.SplitContainer.SplitterWidth = 2;
			this.SplitContainer.TabIndex = 0;
			// 
			// viewportContainer
			// 
			this.viewportContainer.BackColor = System.Drawing.Color.White;
			this.viewportContainer.Controls.Add(this.ReViewOverviewControl);
			this.viewportContainer.Controls.Add(this.tracker);
			this.viewportContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewportContainer.Location = new System.Drawing.Point(0, 0);
			this.viewportContainer.Margin = new System.Windows.Forms.Padding(0);
			this.viewportContainer.Name = "customScrollablePanel1";
			this.viewportContainer.Size = new System.Drawing.Size(924, 458);
			this.viewportContainer.TabIndex = 1;
			this.viewportContainer.VerticalScrollBar = this.ReViewOverviewControl;
			this.viewportContainer.Viewport = this.tracker;
			// 
			// ReViewOverviewControl
			// 
			this.ReViewOverviewControl.Location = new System.Drawing.Point(884, 32);
			this.ReViewOverviewControl.Margin = new System.Windows.Forms.Padding(0);
			this.ReViewOverviewControl.MinimumSize = new System.Drawing.Size(40, 426);
			this.ReViewOverviewControl.Name = "ReViewOverviewControl";
			this.ReViewOverviewControl.OverviewInterface = null;
			this.ReViewOverviewControl.ScrollBarPlacement = ReView.VerticalScrollBarPlacement.VSP_Right;
			this.ReViewOverviewControl.Size = new System.Drawing.Size(40, 426);
			this.ReViewOverviewControl.TabIndex = 2;
			// 
			// tracker
			// 
			this.tracker.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tracker.BackColor = System.Drawing.Color.Silver;
			this.tracker.ItemActiveBackColor = System.Drawing.Color.White;
			this.tracker.ItemSelectedBackColor = System.Drawing.Color.Gold;
			this.tracker.Location = new System.Drawing.Point(0, 32);
			this.tracker.LogFilter = "";
			this.tracker.Margin = new System.Windows.Forms.Padding(0);
			this.tracker.MinimumSize = new System.Drawing.Size(884, 426);
			this.tracker.Model = null;
			this.tracker.Name = "tracker";
			this.tracker.PanOffset = new System.Drawing.Point(0, 0);
			this.tracker.SelectedItem = null;
			this.tracker.SelectedTrack = null;
			this.tracker.ShowGenericTracks = true;
			this.tracker.Size = new System.Drawing.Size(884, 426);
			this.tracker.SplitterFraction = 0.2F;
			this.tracker.SplitterWidth = 2;
			this.tracker.TabIndex = 0;
			this.tracker.TimePixelRatio = 10.9F;
			this.tracker.TrackAreaBackColor = System.Drawing.Color.White;
			this.tracker.TrackFirstLevelBackColor = System.Drawing.Color.Black;
			this.tracker.TrackFirstLevelTextColor = System.Drawing.Color.White;
			this.tracker.TrackFourthLevelBackColor = System.Drawing.Color.SeaShell;
			this.tracker.TrackFourthLevelTextColor = System.Drawing.Color.Black;
			this.tracker.TrackHeaderBackColor = System.Drawing.Color.White;
			this.tracker.TrackHeight = 22;
			this.tracker.TrackIndent = 16;
			this.tracker.TrackMargin = 2;
			this.tracker.TrackSecondLevelBackColor = System.Drawing.Color.LemonChiffon;
			this.tracker.TrackSecondLevelTextColor = System.Drawing.Color.Black;
			this.tracker.TrackSelectedColor = System.Drawing.Color.SkyBlue;
			this.tracker.TrackThirdLevelBackColor = System.Drawing.Color.AliceBlue;
			this.tracker.TrackThirdLevelTextColor = System.Drawing.Color.Black;
			// 
			// rtfDummy
			// 
			this.rtfDummy.BackColor = System.Drawing.Color.White;
			this.rtfDummy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.rtfDummy.Location = new System.Drawing.Point(850, 179);
			this.rtfDummy.Margin = new System.Windows.Forms.Padding(0);
			this.rtfDummy.Name = "rtfDummy";
			this.rtfDummy.ReadOnly = true;
			this.rtfDummy.Size = new System.Drawing.Size(74, 77);
			this.rtfDummy.TabIndex = 1;
			this.rtfDummy.Text = "For text<->RTF conversion";
			this.rtfDummy.Visible = false;
			// 
			// logTextBox
			// 
			this.logTextBox.BackColor = System.Drawing.Color.White;
			this.logTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logTextBox.ForeColor = System.Drawing.Color.Black;
			this.logTextBox.Location = new System.Drawing.Point(0, 0);
			this.logTextBox.Margin = new System.Windows.Forms.Padding(0);
			this.logTextBox.Name = "logTextBox";
			this.logTextBox.ReadOnly = true;
			this.logTextBox.Size = new System.Drawing.Size(924, 112);
			this.logTextBox.TabIndex = 0;
			this.logTextBox.Text = "";
			// 
			// MainTableLayout
			// 
			this.MainTableLayout.AutoSize = true;
			this.MainTableLayout.BackColor = System.Drawing.Color.Transparent;
			this.MainTableLayout.ColumnCount = 1;
			this.MainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.MainTableLayout.Controls.Add(this.SplitContainer, 0, 0);
			this.MainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainTableLayout.Location = new System.Drawing.Point(0, 0);
			this.MainTableLayout.Margin = new System.Windows.Forms.Padding(0);
			this.MainTableLayout.Name = "MainTableLayout";
			this.MainTableLayout.RowCount = 1;
			this.MainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.MainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.MainTableLayout.Size = new System.Drawing.Size(924, 572);
			this.MainTableLayout.TabIndex = 0;
			// 
			// HierarhicalTimelineLog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.MainTableLayout);
			this.Name = "HierarhicalTimelineLog";
			this.Size = new System.Drawing.Size(924, 572);
			this.SplitContainer.Panel1.ResumeLayout(false);
			this.SplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
			this.SplitContainer.ResumeLayout(false);
			this.viewportContainer.ResumeLayout(false);
			this.MainTableLayout.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer SplitContainer;
		private SequencerControl tracker;
		private System.Windows.Forms.TableLayoutPanel MainTableLayout;
		private System.Windows.Forms.RichTextBox logTextBox;
		private System.Windows.Forms.RichTextBox rtfDummy;
		private ViewportContainer viewportContainer;
		private ReViewOverviewControl ReViewOverviewControl;
	}
}
