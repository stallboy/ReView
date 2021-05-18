using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ReView
{
	/// <summary>
	/// Single log entry, LogData is the container for these
	/// </summary>
	public class LogEntry
	{
		public LogEntry(int inTime, string inContent, uint inFlags)
		{
			time = inTime;
			content = inContent;
			flags = inFlags;
		}

		public string TimeAsText
		{
			get
			{
				TimeSpan span = new TimeSpan(time * 1000);
				return "" + string.Format("{0:00}:{1:00}:{2:00},{3:000}", span.TotalHours, span.Minutes, span.Seconds, span.Milliseconds) + "]"; // .NET 2.0
//				return "[" + span.ToString(@"hh\:mm\:ss\,fff") + "]"; // .NET 4.0 ->
			}
		}

		public bool Match(String filter)
		{
			return content.Contains(filter);
		}

		public bool HasFlag(int index)
		{
			return (flags & (1 << index)) != 0;
		}

		public bool HasFlags()
		{
			return flags != 0;
		}

		public int GetHighestFlagBit()
		{
			if (flags == 0)
				return 0;
			int highestBitSet = (int)Math.Floor(Math.Log(flags, 2));
			return highestBitSet;
		}

		public int Time
		{
			get { return time; }
			set { time = value; }
		}

		public string Content
		{
			get { return content; }
			set { content = value; }
		}

		public uint Flags
		{
			get { return flags; }
			set { flags = value; }
		}
		
		private int time;
		private string content;
		private uint flags;
	}
}
