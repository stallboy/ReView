using ReViewDebugRenderView;
namespace ReViewTool
{
	partial class RemoteDebugRenderer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteDebugRenderer));
			this.debugRenderControl = new ReViewDebugRenderView.DebugRenderControl();
			this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.highlightPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.verticalSplitContainer = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
			this.mainSplitContainer.Panel1.SuspendLayout();
			this.mainSplitContainer.Panel2.SuspendLayout();
			this.mainSplitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.verticalSplitContainer)).BeginInit();
			this.verticalSplitContainer.Panel1.SuspendLayout();
			this.verticalSplitContainer.Panel2.SuspendLayout();
			this.verticalSplitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// debugRenderControl
			// 
			this.debugRenderControl.CameraAimSpeed = 0.25D;
			this.debugRenderControl.CameraMoveSpeed = 1D;
			this.debugRenderControl.ClearColor = ((ReView.Color32)(resources.GetObject("debugRenderControl.ClearColor")));
			this.debugRenderControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.debugRenderControl.FSAA = false;
			this.debugRenderControl.GridColor = ((ReView.Color32)(resources.GetObject("debugRenderControl.GridColor")));
			this.debugRenderControl.IsPerspective = true;
			this.debugRenderControl.Location = new System.Drawing.Point(0, 0);
			this.debugRenderControl.Margin = new System.Windows.Forms.Padding(0);
			this.debugRenderControl.Name = "debugRenderControl";
			this.debugRenderControl.OrthoAlignment = ReViewDebugRenderView.OrthoMode.XY;
			this.debugRenderControl.OrthoAlignmentInvert = true;
			this.debugRenderControl.PreviousMousePosition = null;
			this.debugRenderControl.Session = null;
			this.debugRenderControl.ShowAnnotations = false;
			this.debugRenderControl.ShowGrid = true;
			this.debugRenderControl.ShowLineNormals = true;
			this.debugRenderControl.ShowTriangleNormals = false;
			this.debugRenderControl.Size = new System.Drawing.Size(600, 423);
			this.debugRenderControl.TabIndex = 0;
			// 
			// mainSplitContainer
			// 
			this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.mainSplitContainer.Margin = new System.Windows.Forms.Padding(0);
			this.mainSplitContainer.Name = "mainSplitContainer";
			// 
			// mainSplitContainer.Panel1
			// 
			this.mainSplitContainer.Panel1.Controls.Add(this.debugRenderControl);
			// 
			// mainSplitContainer.Panel2
			// 
			this.mainSplitContainer.Panel2.Controls.Add(this.verticalSplitContainer);
			this.mainSplitContainer.Size = new System.Drawing.Size(787, 423);
			this.mainSplitContainer.SplitterDistance = 600;
			this.mainSplitContainer.TabIndex = 1;
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Margin = new System.Windows.Forms.Padding(0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(183, 300);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.ToolbarVisible = false;
			// 
			// highlightPropertyGrid
			// 
			this.highlightPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.highlightPropertyGrid.HelpVisible = false;
			this.highlightPropertyGrid.Location = new System.Drawing.Point(0, 0);
			this.highlightPropertyGrid.Margin = new System.Windows.Forms.Padding(0);
			this.highlightPropertyGrid.Name = "highlightPropertyGrid";
			this.highlightPropertyGrid.Size = new System.Drawing.Size(183, 119);
			this.highlightPropertyGrid.TabIndex = 0;
			this.highlightPropertyGrid.ToolbarVisible = false;
			// 
			// verticalSplitContainer
			// 
			this.verticalSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verticalSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.verticalSplitContainer.Margin = new System.Windows.Forms.Padding(0);
			this.verticalSplitContainer.Name = "verticalSplitContainer";
			this.verticalSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// verticalSplitContainer.Panel1
			// 
			this.verticalSplitContainer.Panel1.Controls.Add(this.propertyGrid);
			// 
			// verticalSplitContainer.Panel2
			// 
			this.verticalSplitContainer.Panel2.Controls.Add(this.highlightPropertyGrid);
			this.verticalSplitContainer.Size = new System.Drawing.Size(183, 423);
			this.verticalSplitContainer.SplitterDistance = 300;
			this.verticalSplitContainer.TabIndex = 1;
			// 
			// RemoteDebugRenderer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mainSplitContainer);
			this.Name = "RemoteDebugRenderer";
			this.Size = new System.Drawing.Size(787, 423);
			this.mainSplitContainer.Panel1.ResumeLayout(false);
			this.mainSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
			this.mainSplitContainer.ResumeLayout(false);
			this.verticalSplitContainer.Panel1.ResumeLayout(false);
			this.verticalSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.verticalSplitContainer)).EndInit();
			this.verticalSplitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private DebugRenderControl debugRenderControl;
		private System.Windows.Forms.SplitContainer mainSplitContainer;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.PropertyGrid highlightPropertyGrid;
		private System.Windows.Forms.SplitContainer verticalSplitContainer;
	}
}
