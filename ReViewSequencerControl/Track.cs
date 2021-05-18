using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public class Track : TrackableObject
	{
		public Track(String inTitle, long id) : base(id)
		{
			Title = inTitle;
			Collapsed = false;
			Visible = true;
			genericTrack = false;
			items = new List<Item>();
			children = new List<Track>();
		}

		public bool Collapsed
		{
			get { return collapsed; }
			set 
			{
				collapsed = value && HasChildren(); // Can only collapse if has children
			}
		}

		public bool Visible
		{
			get { return visible; }
			set { visible = value; }
		}

		public Item LastItem
		{
			get
			{
				return items.Count > 0 ? items.Last() : null;
			}
		}

		public Item ActiveItem
		{
			get
			{
				Item lastItem = items.Count > 0 ? items.Last() : null;
				if (lastItem != null && lastItem.Active)
				{
					return lastItem;
				}
				return null;
			}
		}

		public List<Item> Items
		{
			get { return items; }
			set { items = value;  }
		}

		public List<Track> Children
		{
			get { return children; }
			set { children = value;  }
		}

		public Track Parent
		{
			get { return parent; }
			set 
			{
				if (parent != null)
				{
					parent.RemoveChild(this);
				}
				parent = value;
			}
		}

		public void AddChild(Track track)
		{
			track.Parent = this;
			children.Add(track);
		}

		public void RemoveChild(Track track)
		{
			track.Parent = null;
			children.Remove(track);
		}

		public void AddItem(Item item)
		{
			item.Parent = this;
			items.Add(item);
		}

		public void RemoveItem(Item item)
		{
			item.Parent = null;
			items.Remove(item);
		}

		public void Clear()
		{
			items.Clear();
		}

		public bool HasChildren()
		{
			return children.Count > 0;
		}

		public bool HasItems()
		{
			return items.Count > 0;
		}

		public String Title
		{
			get { return title; }
			set { title = value; }
		}

		public bool IsGenericTrack
		{
			get { return genericTrack; }
			set { genericTrack = value; }
		}

		private String title;
		private Track parent = null;
		private List<Track> children;
		private List<Item> items;
		private bool collapsed;
		private bool visible;
		private bool genericTrack;
	}
}
