using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public enum DataChangeType
	{
		DCT_Added,
		DCT_Removed,
		DCT_Modified
	}

	public class DataChangedEventArgs
	{
		public DataChangedEventArgs(Track parent, DataChangeType type)
		{
			this.type = type;
			this.parent = parent;
		}

		public DataChangeType Type
		{
			get { return type; }
		}

		public Track Parent
		{
			get { return parent; }
		}

		private DataChangeType type;
		private Track parent;
	}

	public delegate void TrackDataChanged(Track source, DataChangedEventArgs args);
	public delegate void ItemDataChanged(Item source, DataChangedEventArgs args);

	public interface SequencerDataModel : TimelineModel
	{
		List<Track> Tracks { get; set; }
		Track GetTrack(long id);
		Item GetItem(long id);
		void GetActiveItems(Track track, List<Item> items);
		List<Item> GetActiveItems();

		bool SuspendTrackDataChangedNotifications { get; set; }
		bool SuspendItemDataChangedNotifications { get; set; }

		event TrackDataChanged OnTrackDataChanged;
		event ItemDataChanged OnItemDataChanged;
	}
}
