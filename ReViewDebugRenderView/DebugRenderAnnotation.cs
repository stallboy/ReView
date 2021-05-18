using ReView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ReViewDebugRenderView
{
	public class DebugRenderAnnotation : INotifyPropertyChanged
	{
		public DebugRenderAnnotation(string text, Color32 color, int time, int duration)
		{
			Text = text;
			Color = color;
			StartTime = time;
			Duration = duration;
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				if (text != value)
				{
					text = value;

					NotifyPropertyChanged("Text");
				}
			}
		}

		public Color32 Color
		{
			get
			{
				return color;
			}
			set
			{
				if (color != value)
				{
					color = value;

					NotifyPropertyChanged("Color");
				}
			}
		}

		public int StartTime
		{
			get
			{
				return startTime;
			}
			set
			{
				if (startTime != value)
				{
					startTime = value;

					NotifyPropertyChanged("StartTime");
					NotifyPropertyChanged("Duration");
				}
			}
		}

		public int Duration
		{
			get
			{
				return InfiniteLength ? int.MaxValue : (EndTime - StartTime);
			}
			set
			{
				EndTime = (value < 0) ? -1 : StartTime + value;
			}
		}

		public bool InfiniteLength
		{
			get
			{
				return EndTime < 0;
			}
		}

		public int EndTime
		{
			get
			{
				return endTime;
			}
			set
			{
				if (endTime != value)
				{
					endTime = value;

					NotifyPropertyChanged("EndTime");
					NotifyPropertyChanged("Duration");
					NotifyPropertyChanged("InfiniteLength");
				}
			}
		}

		public bool IsValidAt(int time)
		{
			return time >= startTime && (endTime < 0 || time < endTime);
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		private int startTime;
		private int endTime;
		private Color32 color;
		private string text;
	}
}
