using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public class Item : TrackableObject
	{
		public Item(String name, long id, int inStartTime, int inEndTime) : base(id)
		{
			StartTime = inStartTime;
			EndTime = inEndTime;
			Title = name;
			Active = false;
			Visible = true;
			Log = new LogData();
		}

		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		public LogData Log
		{
			get { return logData; }
			set { logData = value; }
		}

		public String Title
		{
			get { return title; }
			set 
			{ 
				title = value;
				if (ItemChangedNotification != null)
					ItemChangedNotification(this);
			}
		}

		public int Length
		{
			get
			{
				return (EndTime - StartTime);
			}
			private set { }
		}

		public int StartTime
		{
			get { return startTime; }
			set 
			{ 
				startTime = value;
				if (ItemChangedNotification != null)
					ItemChangedNotification(this);
			}
		}

		public int EndTime
		{
			get { return endTime; }
			set 
			{ 
				endTime = value;
				if (ItemChangedNotification != null)
					ItemChangedNotification(this);
			}
		}

		public bool Visible
		{
			get { return visible; }
			set { visible = value; }
		}

		public Track Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public delegate void ItemChanged(Item changedItem);

		public ItemChanged ItemChangedNotification;

		private Track parent;
		private bool visible;
		private int startTime;
		private int endTime;
		private bool active;
		private String title;
		private LogData logData;
	}
}
