using ReView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReViewTool
{
	public class TimelineSession : TimelineModel
	{
		public int Duration
		{
			get { return duration; }
			set
			{
				duration = value;
				if (!SuspendDurationChangedNotifications)
				{
					if (OnDurationChanged != null)
						OnDurationChanged();
				}
			}
		}

		public int PlaybackPosition
		{
			get { return playbackPosition; }
			set
			{
				int clampedValue = value;
				if (clampedValue > duration)
					clampedValue = duration;
				if (clampedValue < 0)
					clampedValue = 0;

				if (playbackPosition != clampedValue)
				{
					playbackPosition = clampedValue;

					if (!SuspendPlaybackPositionChangedNotifications && OnPlaybackPositionChanged != null)
						OnPlaybackPositionChanged(playbackPosition);
				}
			}
		}

		public bool SuspendPlaybackPositionChangedNotifications
		{
			get;
			set;
		}

		public bool SuspendDurationChangedNotifications
		{
			get;
			set;
		}

		public void UpdateDuration(int inDuration)
		{
			if (inDuration > duration)
			{
				Duration = inDuration;
			}
		}

		protected int playbackPosition = 0;
		protected int duration = 0;

		public event PlaybackPositionChanged OnPlaybackPositionChanged;
		public event DurationChanged OnDurationChanged;
	}
}
