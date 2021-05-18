using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public class HTLSession : TimelineModel, SequencerDataModel
	{
		public HTLSession()
		{
			SuspendTrackDataChangedNotifications = false;
			SuspendItemDataChangedNotifications = false;
			SuspendPlaybackPositionChangedNotifications = false;
			SuspendDurationChangedNotifications = false;
		}

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

		public List<Track> Tracks
		{
			get { return tracks; }
			set { tracks = value; }
		}

		public Track GetTrack(long id)
		{
			if (trackableMap.ContainsKey(id))
			{
				return trackableMap[id] as Track;
			}
			return null;
		}

		public Item GetItem(long id)
		{
			if (trackableMap.ContainsKey(id))
			{
				return trackableMap[id] as Item;
			}
			return null;
		}

		public void GetActiveItems(Track track, List<Item> items)
		{
			if (track.Items.Count > 0 && track.Items.Last().Active)
			{
				items.Add(track.Items.Last());
			}
			foreach (Track children in track.Children)
			{
				GetActiveItems(children, items);
			}
		}

		public List<Item> GetActiveItems()
		{
			List<Item> activeItems = new List<Item>();
			foreach (Track track in Tracks)
			{
				GetActiveItems(track, activeItems);
			}
			return activeItems;
		}

		public void AddTrack(Track parent, Track track)
		{
			lock (this)
			{
				trackableMap.Add(track.Id, track);
				if (parent != null)
					parent.AddChild(track);
				else
					tracks.Add(track);
			}

			if (!SuspendTrackDataChangedNotifications && OnTrackDataChanged != null)
				OnTrackDataChanged(track, new DataChangedEventArgs(parent, DataChangeType.DCT_Added));
		}

		public void RemoveTrack(Track track)
		{
			Track parentTrack = track.Parent;

			lock (this)
			{
				trackableMap.Remove(track.Id);

				if (track.Parent == null)
				{
					tracks.Remove(track);
				}
				else
				{
					track.Parent.RemoveChild(track);
				}
			}

			if (!SuspendTrackDataChangedNotifications && OnTrackDataChanged != null)
				OnTrackDataChanged(track, new DataChangedEventArgs(parentTrack, DataChangeType.DCT_Removed));
		}

		public void MergeItem(Item item, Item itemToMerge)
		{
			lock (this)
			{
				item.Title = item.Title + " + " + itemToMerge.Title;
				item.Log.AddLogEntry(new LogEntry(itemToMerge.StartTime, ">>> Merged '" + itemToMerge.Title + "' with id(" + itemToMerge.Id + ") <<<", 0));

				trackableMap.Add(itemToMerge.Id, item);

				item.Active = true;
				item.EndTime = itemToMerge.StartTime;
			}
			UpdateDuration(itemToMerge.StartTime);

			ItemChanged(item);
		}

		public void AddItem(Track track, Item item)
		{
			lock (this)
			{
				item.ItemChangedNotification += ItemChanged;
				trackableMap.Add(item.Id, item);

				if (track.LastItem != null && track.LastItem.Active)
				{
					// If has active last item -> Deactivate and update end time
					track.LastItem.Active = false;
					track.LastItem.EndTime = item.StartTime;
				}

				track.AddItem(item);
			}
			UpdateDuration(item.StartTime);

			ItemChanged(item);

			if (!SuspendItemDataChangedNotifications && OnItemDataChanged != null)
				OnItemDataChanged(item, new DataChangedEventArgs(track, DataChangeType.DCT_Added));
		}

		public void RemoveItem(Track track, Item item)
		{
			lock (this)
			{
				item.ItemChangedNotification -= ItemChanged;
				trackableMap.Remove(item.Id);
				track.RemoveItem(item);
			}

			if (!SuspendItemDataChangedNotifications && OnItemDataChanged != null)
				OnItemDataChanged(item, new DataChangedEventArgs(track, DataChangeType.DCT_Removed));
		}

		public void AddGenericItem(Track parentTrack, Item item)
		{
			if (parentTrack == null || item == null)
			{
				return;
			}

			Track itemTrack = null;
			lock (this)
			{
				foreach (Track track in parentTrack.Children)
				{
					Item lastItem = track.Items.Count > 0 ? track.Items[track.Items.Count - 1] : null;
					if (lastItem == null || !lastItem.Active)
					{
						itemTrack = track;
						break;
					}
				}
			}

			if (itemTrack == null)
			{
				itemTrack = new Track("" + parentTrack.Children.Count, genericTrackAutoID++);
				itemTrack.IsGenericTrack = true;
				parentTrack.IsGenericTrack = true; // Mark parent as generic when we add a generic track under it
				AddTrack(parentTrack, itemTrack);
			}
			AddItem(itemTrack, item);
		}

		public void Clear()
		{
			lock (this)
			{
				tracks.Clear();
			}

			if (!SuspendTrackDataChangedNotifications && OnTrackDataChanged != null)
				OnTrackDataChanged(null, new DataChangedEventArgs(null, DataChangeType.DCT_Removed));
		}

		public void UpdateDuration(int time)
		{
			if (time > duration)
			{
				Duration = time;

				List<Item> activeItems = GetActiveItems();
				foreach (Item item in activeItems)
				{
					// Update end time of all active items to match the duration
					item.EndTime = Duration;
				}
			}
		}

		public void ItemChanged(Item item)
		{
			UpdateDuration(item.EndTime);

			if (!SuspendItemDataChangedNotifications && OnItemDataChanged != null)
				OnItemDataChanged(item, new DataChangedEventArgs(item.Parent, DataChangeType.DCT_Modified));
		}

		public bool SuspendTrackDataChangedNotifications
		{
			get;
			set;
		}

		public bool SuspendItemDataChangedNotifications
		{
			get;
			set;
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

		private Dictionary<long, TrackableObject> trackableMap = new Dictionary<long, TrackableObject>();
		private List<Track> tracks = new List<Track>();

		public event TrackDataChanged OnTrackDataChanged;
		public event ItemDataChanged OnItemDataChanged;
		public event PlaybackPositionChanged OnPlaybackPositionChanged;
		public event DurationChanged OnDurationChanged;

		public int genericTrackAutoID = (int)(0x7ff00000);

		protected int playbackPosition = 0;
		protected int duration = 0;
	}
}
