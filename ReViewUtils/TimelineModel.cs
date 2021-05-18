using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public delegate void PlaybackPositionChanged(int newPosition);
	public delegate void DurationChanged();
 
	public interface TimelineModel
	{
		int Duration { get; set; }
		int PlaybackPosition { get; set; }
		bool SuspendPlaybackPositionChangedNotifications { get; set; }
		bool SuspendDurationChangedNotifications { get; set; }

		event PlaybackPositionChanged OnPlaybackPositionChanged;
		event DurationChanged OnDurationChanged;
	}
}
