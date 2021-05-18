using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	/// <summary>
	/// Selection data class (Track, Item or both can be selected)
	/// Currently supports only single-selection
	/// </summary>
	public class Selection
	{
		public Selection(Track inTrack, Item inItem, bool collapseHit)
		{
			Track = inTrack;
			Item = inItem;
			CollapseAreaHit = collapseHit;
		}

		public bool CollapseAreaHit
		{
			get { return collapseAreaHit; }
			set { collapseAreaHit = value; }
		}

		public Track Track
		{
			get { return track; }
			set { track = value; }
		}

		public Item Item
		{
			get { return item; }
			set { item = value; }
		}
		
		private Track track;
		private Item item;
		private bool collapseAreaHit;
	}
}
