namespace ReView
{
	partial class TimelineControl
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
			this.components = new System.ComponentModel.Container();
			this.ReViewTimelineControlContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.SuspendLayout();
			// 
			// ReViewTimelineControlContextMenu
			// 
			this.ReViewTimelineControlContextMenu.Name = "ReViewTimelineControlContextMenu";
			this.ReViewTimelineControlContextMenu.Size = new System.Drawing.Size(61, 4);
			this.ReViewTimelineControlContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ReViewTimelineControlContextMenu_Opening);
			// 
			// ReViewTimelineControlComponent
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ContextMenuStrip = this.ReViewTimelineControlContextMenu;
			this.Name = "ReViewTimelineControlComponent";
			this.Size = new System.Drawing.Size(480, 48);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip ReViewTimelineControlContextMenu;

	}
}
