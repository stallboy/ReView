using ReView;
namespace ReViewTool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.mainTable = new System.Windows.Forms.TableLayoutPanel();
			this.horizontalFillerPanel = new System.Windows.Forms.Panel();
			this.timelineControl = new ReView.TimelineControl();
			this.toolPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.connectButton = new System.Windows.Forms.Button();
			this.preferencesButton = new System.Windows.Forms.Button();
			this.toggleAutoFollow = new System.Windows.Forms.CheckBox();
			this.playbackControlTable = new System.Windows.Forms.TableLayoutPanel();
			this.loopToggle = new System.Windows.Forms.CheckBox();
			this.playButton = new System.Windows.Forms.Button();
			this.timelineNextButton = new System.Windows.Forms.Button();
			this.timelinePrevButton = new System.Windows.Forms.Button();
			this.timelineNavigationSnapButton = new System.Windows.Forms.Button();
			this.debugModuleButtonContainer = new System.Windows.Forms.FlowLayoutPanel();
			this.userCommandButtonFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.statusFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.statusLabel = new System.Windows.Forms.Label();
			this.componentLayoutTable = new System.Windows.Forms.TableLayoutPanel();
			this.verticalFillerPanel = new System.Windows.Forms.Panel();
			this.componenFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.timelineLogButton = new ReViewTool.VerticalButton();
			this.debugRendererButton = new ReViewTool.VerticalButton();
			this.mainTable.SuspendLayout();
			this.toolPanel.SuspendLayout();
			this.playbackControlTable.SuspendLayout();
			this.statusFlowLayout.SuspendLayout();
			this.componentLayoutTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainTable
			// 
			this.mainTable.BackColor = System.Drawing.Color.White;
			this.mainTable.ColumnCount = 2;
			this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTable.Controls.Add(this.horizontalFillerPanel, 1, 1);
			this.mainTable.Controls.Add(this.timelineControl, 0, 2);
			this.mainTable.Controls.Add(this.toolPanel, 0, 0);
			this.mainTable.Controls.Add(this.statusFlowLayout, 0, 4);
			this.mainTable.Controls.Add(this.componentLayoutTable, 0, 1);
			this.mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTable.Location = new System.Drawing.Point(0, 0);
			this.mainTable.Margin = new System.Windows.Forms.Padding(0);
			this.mainTable.Name = "mainTable";
			this.mainTable.RowCount = 5;
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.mainTable.Size = new System.Drawing.Size(1264, 681);
			this.mainTable.TabIndex = 1;
			// 
			// horizontalFillerPanel
			// 
			this.horizontalFillerPanel.AutoSize = true;
			this.horizontalFillerPanel.BackColor = System.Drawing.Color.DarkGray;
			this.horizontalFillerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.horizontalFillerPanel.Location = new System.Drawing.Point(30, 74);
			this.horizontalFillerPanel.Margin = new System.Windows.Forms.Padding(0);
			this.horizontalFillerPanel.Name = "horizontalFillerPanel";
			this.horizontalFillerPanel.Size = new System.Drawing.Size(1234, 2);
			this.horizontalFillerPanel.TabIndex = 21;
			// 
			// timelineControl
			// 
			this.timelineControl.AnnotationColor = System.Drawing.Color.MediumAquamarine;
			this.timelineControl.AutoFollowTail = true;
			this.timelineControl.AutoPanToPlaybackHeader = true;
			this.timelineControl.BackColor = System.Drawing.Color.White;
			this.timelineControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.timelineControl.ForeColor = System.Drawing.Color.White;
			this.timelineControl.ForegroundColor = System.Drawing.Color.Black;
			this.timelineControl.Location = new System.Drawing.Point(30, 76);
			this.timelineControl.Margin = new System.Windows.Forms.Padding(0);
			this.timelineControl.MinimumSize = new System.Drawing.Size(706, 32);
			this.timelineControl.Model = null;
			this.timelineControl.Name = "timelineControl";
			this.timelineControl.OrientationUp = true;
			this.timelineControl.PanOffset = new System.Drawing.Point(0, 0);
			this.timelineControl.PlaybackHeaderColor = System.Drawing.Color.DodgerBlue;
			this.timelineControl.ScrollBarPlacement = ReView.HorizontalScrollBarPlacement.HSP_Top;
			this.timelineControl.SelectedAnnotationColor = System.Drawing.Color.Gold;
			this.timelineControl.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.timelineControl.Size = new System.Drawing.Size(1234, 32);
			this.timelineControl.TabIndex = 7;
			this.timelineControl.TimelineBackColor = System.Drawing.Color.White;
			this.timelineControl.TimePixelRatio = 10.9F;
			// 
			// toolPanel
			// 
			this.toolPanel.BackColor = System.Drawing.Color.White;
			this.mainTable.SetColumnSpan(this.toolPanel, 2);
			this.toolPanel.Controls.Add(this.connectButton);
			this.toolPanel.Controls.Add(this.preferencesButton);
			this.toolPanel.Controls.Add(this.toggleAutoFollow);
			this.toolPanel.Controls.Add(this.playbackControlTable);
			this.toolPanel.Controls.Add(this.debugModuleButtonContainer);
			this.toolPanel.Controls.Add(this.userCommandButtonFlowLayout);
			this.toolPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolPanel.Location = new System.Drawing.Point(0, 0);
			this.toolPanel.Margin = new System.Windows.Forms.Padding(0);
			this.toolPanel.Name = "toolPanel";
			this.toolPanel.Size = new System.Drawing.Size(1264, 74);
			this.toolPanel.TabIndex = 1;
			this.toolPanel.WrapContents = false;
			// 
			// connectButton
			// 
			this.connectButton.BackColor = System.Drawing.Color.Silver;
			this.connectButton.FlatAppearance.BorderSize = 0;
			this.connectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.connectButton.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.connectButton.ForeColor = System.Drawing.Color.White;
			this.connectButton.Location = new System.Drawing.Point(5, 5);
			this.connectButton.Margin = new System.Windows.Forms.Padding(5, 5, 0, 5);
			this.connectButton.Name = "connectButton";
			this.connectButton.Size = new System.Drawing.Size(78, 64);
			this.connectButton.TabIndex = 0;
			this.connectButton.Text = "Connect";
			this.connectButton.UseVisualStyleBackColor = false;
			this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
			// 
			// preferencesButton
			// 
			this.preferencesButton.BackColor = System.Drawing.Color.Silver;
			this.preferencesButton.FlatAppearance.BorderSize = 0;
			this.preferencesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.preferencesButton.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.preferencesButton.ForeColor = System.Drawing.Color.White;
			this.preferencesButton.Location = new System.Drawing.Point(88, 5);
			this.preferencesButton.Margin = new System.Windows.Forms.Padding(5, 5, 0, 5);
			this.preferencesButton.Name = "preferencesButton";
			this.preferencesButton.Size = new System.Drawing.Size(80, 64);
			this.preferencesButton.TabIndex = 17;
			this.preferencesButton.Text = "Preferences";
			this.preferencesButton.UseVisualStyleBackColor = false;
			this.preferencesButton.Click += new System.EventHandler(this.preferencesButton_Click);
			// 
			// toggleAutoFollow
			// 
			this.toggleAutoFollow.Appearance = System.Windows.Forms.Appearance.Button;
			this.toggleAutoFollow.BackColor = System.Drawing.Color.DeepSkyBlue;
			this.toggleAutoFollow.Checked = true;
			this.toggleAutoFollow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toggleAutoFollow.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.toggleAutoFollow.FlatAppearance.BorderSize = 0;
			this.toggleAutoFollow.FlatAppearance.CheckedBackColor = System.Drawing.Color.DeepSkyBlue;
			this.toggleAutoFollow.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DodgerBlue;
			this.toggleAutoFollow.FlatAppearance.MouseOverBackColor = System.Drawing.Color.PaleTurquoise;
			this.toggleAutoFollow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.toggleAutoFollow.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toggleAutoFollow.ForeColor = System.Drawing.Color.White;
			this.toggleAutoFollow.Location = new System.Drawing.Point(173, 5);
			this.toggleAutoFollow.Margin = new System.Windows.Forms.Padding(5, 5, 0, 5);
			this.toggleAutoFollow.Name = "toggleAutoFollow";
			this.toggleAutoFollow.Size = new System.Drawing.Size(70, 64);
			this.toggleAutoFollow.TabIndex = 16;
			this.toggleAutoFollow.Text = "+ Auto follow";
			this.toggleAutoFollow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toggleAutoFollow.UseVisualStyleBackColor = false;
			this.toggleAutoFollow.CheckedChanged += new System.EventHandler(this.toggleAutoFollow_CheckedChanged);
			// 
			// playbackControlTable
			// 
			this.playbackControlTable.AutoSize = true;
			this.playbackControlTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.playbackControlTable.ColumnCount = 4;
			this.playbackControlTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.playbackControlTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.playbackControlTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.playbackControlTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.playbackControlTable.Controls.Add(this.loopToggle, 0, 1);
			this.playbackControlTable.Controls.Add(this.playButton, 2, 1);
			this.playbackControlTable.Controls.Add(this.timelineNextButton, 3, 0);
			this.playbackControlTable.Controls.Add(this.timelinePrevButton, 0, 0);
			this.playbackControlTable.Controls.Add(this.timelineNavigationSnapButton, 1, 0);
			this.playbackControlTable.Location = new System.Drawing.Point(248, 5);
			this.playbackControlTable.Margin = new System.Windows.Forms.Padding(5, 5, 0, 5);
			this.playbackControlTable.Name = "playbackControlTable";
			this.playbackControlTable.RowCount = 2;
			this.playbackControlTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.playbackControlTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.playbackControlTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.playbackControlTable.Size = new System.Drawing.Size(161, 64);
			this.playbackControlTable.TabIndex = 23;
			// 
			// loopToggle
			// 
			this.loopToggle.Appearance = System.Windows.Forms.Appearance.Button;
			this.loopToggle.BackColor = System.Drawing.Color.DeepSkyBlue;
			this.loopToggle.Checked = true;
			this.loopToggle.CheckState = System.Windows.Forms.CheckState.Checked;
			this.playbackControlTable.SetColumnSpan(this.loopToggle, 2);
			this.loopToggle.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.loopToggle.FlatAppearance.BorderSize = 0;
			this.loopToggle.FlatAppearance.CheckedBackColor = System.Drawing.Color.DeepSkyBlue;
			this.loopToggle.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DodgerBlue;
			this.loopToggle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.PaleTurquoise;
			this.loopToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.loopToggle.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.loopToggle.ForeColor = System.Drawing.Color.White;
			this.loopToggle.Location = new System.Drawing.Point(0, 34);
			this.loopToggle.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.loopToggle.Name = "loopToggle";
			this.loopToggle.Size = new System.Drawing.Size(79, 30);
			this.loopToggle.TabIndex = 28;
			this.loopToggle.Text = "+ Loop";
			this.loopToggle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.loopToggle.UseVisualStyleBackColor = false;
			this.loopToggle.CheckedChanged += new System.EventHandler(this.loopToggle_CheckedChanged);
			// 
			// playButton
			// 
			this.playButton.BackColor = System.Drawing.Color.DeepSkyBlue;
			this.playbackControlTable.SetColumnSpan(this.playButton, 2);
			this.playButton.FlatAppearance.BorderSize = 0;
			this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.playButton.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.playButton.ForeColor = System.Drawing.Color.White;
			this.playButton.Location = new System.Drawing.Point(82, 34);
			this.playButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
			this.playButton.Name = "playButton";
			this.playButton.Size = new System.Drawing.Size(79, 30);
			this.playButton.TabIndex = 27;
			this.playButton.Text = "Play";
			this.playButton.UseVisualStyleBackColor = false;
			this.playButton.Click += new System.EventHandler(this.playButton_Click);
			// 
			// timelineNextButton
			// 
			this.timelineNextButton.BackColor = System.Drawing.Color.DeepSkyBlue;
			this.timelineNextButton.FlatAppearance.BorderSize = 0;
			this.timelineNextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.timelineNextButton.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.timelineNextButton.ForeColor = System.Drawing.Color.White;
			this.timelineNextButton.Location = new System.Drawing.Point(131, 0);
			this.timelineNextButton.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.timelineNextButton.Name = "timelineNextButton";
			this.timelineNextButton.Size = new System.Drawing.Size(30, 31);
			this.timelineNextButton.TabIndex = 24;
			this.timelineNextButton.Text = ">";
			this.timelineNextButton.UseVisualStyleBackColor = false;
			this.timelineNextButton.Click += new System.EventHandler(this.timelineNextButton_Click);
			// 
			// timelinePrevButton
			// 
			this.timelinePrevButton.BackColor = System.Drawing.Color.DeepSkyBlue;
			this.timelinePrevButton.FlatAppearance.BorderSize = 0;
			this.timelinePrevButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.timelinePrevButton.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.timelinePrevButton.ForeColor = System.Drawing.Color.White;
			this.timelinePrevButton.Location = new System.Drawing.Point(0, 0);
			this.timelinePrevButton.Margin = new System.Windows.Forms.Padding(0);
			this.timelinePrevButton.Name = "timelinePrevButton";
			this.timelinePrevButton.Size = new System.Drawing.Size(30, 31);
			this.timelinePrevButton.TabIndex = 25;
			this.timelinePrevButton.Text = "<";
			this.timelinePrevButton.UseVisualStyleBackColor = false;
			this.timelinePrevButton.Click += new System.EventHandler(this.timelinePrevButton_Click);
			// 
			// timelineNavigationSnapButton
			// 
			this.timelineNavigationSnapButton.BackColor = System.Drawing.Color.DeepSkyBlue;
			this.playbackControlTable.SetColumnSpan(this.timelineNavigationSnapButton, 2);
			this.timelineNavigationSnapButton.FlatAppearance.BorderSize = 0;
			this.timelineNavigationSnapButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.timelineNavigationSnapButton.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.timelineNavigationSnapButton.ForeColor = System.Drawing.Color.White;
			this.timelineNavigationSnapButton.Location = new System.Drawing.Point(33, 0);
			this.timelineNavigationSnapButton.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.timelineNavigationSnapButton.Name = "timelineNavigationSnapButton";
			this.timelineNavigationSnapButton.Size = new System.Drawing.Size(95, 31);
			this.timelineNavigationSnapButton.TabIndex = 26;
			this.timelineNavigationSnapButton.Text = "Debug Module";
			this.timelineNavigationSnapButton.UseVisualStyleBackColor = false;
			this.timelineNavigationSnapButton.Click += new System.EventHandler(this.timelineNavigationSnapButton_Click);
			// 
			// debugModuleButtonContainer
			// 
			this.debugModuleButtonContainer.AutoSize = true;
			this.debugModuleButtonContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.debugModuleButtonContainer.BackColor = System.Drawing.Color.DarkGray;
			this.debugModuleButtonContainer.Location = new System.Drawing.Point(414, 0);
			this.debugModuleButtonContainer.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.debugModuleButtonContainer.MinimumSize = new System.Drawing.Size(0, 74);
			this.debugModuleButtonContainer.Name = "debugModuleButtonContainer";
			this.debugModuleButtonContainer.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
			this.debugModuleButtonContainer.Size = new System.Drawing.Size(10, 74);
			this.debugModuleButtonContainer.TabIndex = 21;
			// 
			// userCommandButtonFlowLayout
			// 
			this.userCommandButtonFlowLayout.AutoSize = true;
			this.userCommandButtonFlowLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.userCommandButtonFlowLayout.Location = new System.Drawing.Point(424, 0);
			this.userCommandButtonFlowLayout.Margin = new System.Windows.Forms.Padding(0);
			this.userCommandButtonFlowLayout.MinimumSize = new System.Drawing.Size(0, 74);
			this.userCommandButtonFlowLayout.Name = "userCommandButtonFlowLayout";
			this.userCommandButtonFlowLayout.Size = new System.Drawing.Size(0, 74);
			this.userCommandButtonFlowLayout.TabIndex = 19;
			// 
			// statusFlowLayout
			// 
			this.statusFlowLayout.BackColor = System.Drawing.Color.DodgerBlue;
			this.mainTable.SetColumnSpan(this.statusFlowLayout, 2);
			this.statusFlowLayout.Controls.Add(this.statusLabel);
			this.statusFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.statusFlowLayout.Location = new System.Drawing.Point(0, 661);
			this.statusFlowLayout.Margin = new System.Windows.Forms.Padding(0);
			this.statusFlowLayout.Name = "statusFlowLayout";
			this.statusFlowLayout.Size = new System.Drawing.Size(1264, 20);
			this.statusFlowLayout.TabIndex = 6;
			// 
			// statusLabel
			// 
			this.statusLabel.AutoSize = true;
			this.statusLabel.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.statusLabel.ForeColor = System.Drawing.Color.White;
			this.statusLabel.Location = new System.Drawing.Point(3, 0);
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(46, 18);
			this.statusLabel.TabIndex = 4;
			this.statusLabel.Text = "Status";
			// 
			// componentLayoutTable
			// 
			this.componentLayoutTable.BackColor = System.Drawing.Color.DodgerBlue;
			this.componentLayoutTable.ColumnCount = 2;
			this.componentLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.componentLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
			this.componentLayoutTable.Controls.Add(this.verticalFillerPanel, 1, 0);
			this.componentLayoutTable.Controls.Add(this.componenFlowLayout, 0, 0);
			this.componentLayoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.componentLayoutTable.Location = new System.Drawing.Point(0, 74);
			this.componentLayoutTable.Margin = new System.Windows.Forms.Padding(0);
			this.componentLayoutTable.Name = "componentLayoutTable";
			this.componentLayoutTable.RowCount = 1;
			this.mainTable.SetRowSpan(this.componentLayoutTable, 3);
			this.componentLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.componentLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 635F));
			this.componentLayoutTable.Size = new System.Drawing.Size(30, 587);
			this.componentLayoutTable.TabIndex = 3;
			// 
			// verticalFillerPanel
			// 
			this.verticalFillerPanel.AutoSize = true;
			this.verticalFillerPanel.BackColor = System.Drawing.Color.DarkGray;
			this.verticalFillerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verticalFillerPanel.Location = new System.Drawing.Point(28, 0);
			this.verticalFillerPanel.Margin = new System.Windows.Forms.Padding(0);
			this.verticalFillerPanel.Name = "verticalFillerPanel";
			this.verticalFillerPanel.Size = new System.Drawing.Size(2, 587);
			this.verticalFillerPanel.TabIndex = 22;
			// 
			// componenFlowLayout
			// 
			this.componenFlowLayout.BackColor = System.Drawing.Color.White;
			this.componenFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.componenFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.componenFlowLayout.Location = new System.Drawing.Point(0, 0);
			this.componenFlowLayout.Margin = new System.Windows.Forms.Padding(0);
			this.componenFlowLayout.Name = "componenFlowLayout";
			this.componenFlowLayout.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.componenFlowLayout.Size = new System.Drawing.Size(28, 587);
			this.componenFlowLayout.TabIndex = 7;
			// 
			// timelineLogButton
			// 
			this.timelineLogButton.Location = new System.Drawing.Point(0, 0);
			this.timelineLogButton.Name = "timelineLogButton";
			this.timelineLogButton.Size = new System.Drawing.Size(75, 23);
			this.timelineLogButton.TabIndex = 0;
			this.timelineLogButton.VerticalText = null;
			// 
			// debugRendererButton
			// 
			this.debugRendererButton.Location = new System.Drawing.Point(0, 0);
			this.debugRendererButton.Name = "debugRendererButton";
			this.debugRendererButton.Size = new System.Drawing.Size(75, 23);
			this.debugRendererButton.TabIndex = 0;
			this.debugRendererButton.VerticalText = null;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
			this.ClientSize = new System.Drawing.Size(1264, 681);
			this.Controls.Add(this.mainTable);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(300, 300);
			this.Name = "MainForm";
			this.Text = "ReView";
			this.mainTable.ResumeLayout(false);
			this.mainTable.PerformLayout();
			this.toolPanel.ResumeLayout(false);
			this.toolPanel.PerformLayout();
			this.playbackControlTable.ResumeLayout(false);
			this.statusFlowLayout.ResumeLayout(false);
			this.statusFlowLayout.PerformLayout();
			this.componentLayoutTable.ResumeLayout(false);
			this.componentLayoutTable.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel mainTable;
		private System.Windows.Forms.FlowLayoutPanel toolPanel;
		private System.Windows.Forms.Button connectButton;
		private System.Windows.Forms.CheckBox toggleAutoFollow;
		private System.Windows.Forms.FlowLayoutPanel statusFlowLayout;
		private System.Windows.Forms.Label statusLabel;
		private System.Windows.Forms.Button preferencesButton;
		private System.Windows.Forms.FlowLayoutPanel userCommandButtonFlowLayout;
		private System.Windows.Forms.FlowLayoutPanel componenFlowLayout;
		private VerticalButton timelineLogButton;
		private VerticalButton debugRendererButton;
		private System.Windows.Forms.TableLayoutPanel componentLayoutTable;
		private TimelineControl timelineControl;
		private System.Windows.Forms.Panel horizontalFillerPanel;
		private System.Windows.Forms.FlowLayoutPanel debugModuleButtonContainer;
		private System.Windows.Forms.Panel verticalFillerPanel;
		private System.Windows.Forms.TableLayoutPanel playbackControlTable;
		private System.Windows.Forms.Button timelineNextButton;
		private System.Windows.Forms.Button timelinePrevButton;
		private System.Windows.Forms.Button timelineNavigationSnapButton;
		private System.Windows.Forms.Button playButton;
		private System.Windows.Forms.CheckBox loopToggle;
	}
}

