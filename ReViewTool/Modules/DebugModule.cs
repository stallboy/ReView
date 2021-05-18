using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReViewTool
{
	public class DebugModule : UserControl
	{
		// Virtual methods to implement
		public virtual void OnInitDebugModule() { }

		public virtual void OnActivateDebugModule() { }

		public virtual void OnDeactivateDebugModule() { }

		public virtual void OnResetSessions() { }

		public virtual void OnHeartbeat(int time) { }

		public virtual void OnRPCStateChanged(bool connected) { }

		public virtual string ModuleName { get; set; }

		public virtual void OnTimelinePlaybackPositionChanged(int playbackPosition) { }

		public virtual void OnTimelineZoomChanged(float timePixelRatio) { }

		public virtual void OnTimelinePanOffsetChanged(System.Drawing.Point panOffset) { }

		public virtual FlowLayoutPanel GetToolbarButtonFlowLayout() { return null; }

		public virtual int GetNextTimelineEventTime(int time) { return -1; }

		public virtual int GetPrevTimelineEventTime(int time) { return -1; }

		// Methods to notify changes to timeline view back
		protected void NotifyTimePixelRatioChanged(float timePixelRatio) 
		{
			DlgTimePixelRatioChanged handler = TimePixelRatioChanged;
			if (handler != null)
			{
				handler(timePixelRatio);
			}
		}

		protected void NotifyPanOffsetChanged(int pixels, bool userChange) 
		{
			DlgPanOffsetChanged handler = PanOffsetChanged;
			if (handler != null)
			{
				handler(pixels, userChange);
			}
		}

		protected void NotifyDurationChanged(int duration)
		{
			DlgDurationChanged handler = DurationChanged;
			if (handler != null)
			{
				handler(duration);
			}
		}

		protected void RequestTimelineMarginChange(int leftMargin, int rightMargin)
		{
			DlgTimelineMarginChangeRequested handler = TimelineMarginChangeRequested;
			if (handler != null)
			{
				handler(leftMargin, rightMargin);
			}
		}

		public delegate void DlgTimePixelRatioChanged(float timePixelRatio);
		public delegate void DlgPanOffsetChanged(int pixels, bool userChange);
		public delegate void DlgTimelineMarginChangeRequested(int leftMargin, int rightMargin);
		public delegate void DlgDurationChanged(int duration);

		public event DlgDurationChanged DurationChanged;
		public event DlgTimePixelRatioChanged TimePixelRatioChanged;
		public event DlgPanOffsetChanged PanOffsetChanged;
		public event DlgTimelineMarginChangeRequested TimelineMarginChangeRequested;
	}
}
