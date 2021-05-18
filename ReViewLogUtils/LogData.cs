using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ReView
{
	/// <summary>
	/// Log data container, used for example by track items to store all log output
	/// </summary>
	public class LogData
	{
		public LogData()
		{
			logEntries = new List<LogEntry>();
			mergedFlags = 0;
			matchesFilter = true;
		}

		public List<LogEntry> LogEntries
		{
			get { return logEntries; }
		}

		public void AddLogEntry(LogEntry logEntry)
		{
			logEntries.Add(logEntry);
			mergedFlags = (mergedFlags | logEntry.Flags);
		}

		/// <summary>
		/// Perform regexp match for log entries, used to test if owner of this log data should be displayed or not
		/// </summary>
		public bool Match(String filter)
		{
			matchesFilter = (logEntries.Count == 0 && filter.Length == 0);
			foreach (LogEntry entry in logEntries)
			{
				matchesFilter = matchesFilter || entry.Match(filter);
				if (matchesFilter)
				{
					break;
				}
			}
			return matchesFilter;
		}

		/// <summary>
		/// Test flag match, used to test if owner of this log data should be displayed or not
		/// </summary>
		public bool FlagsMatch(uint showFlags)
		{
			return mergedFlags == 0 || (mergedFlags & showFlags) != 0;
		}

		public bool HasFlags()
		{
			return mergedFlags != 0;
		}

		public int GetHighestFlagBit()
		{
			if (mergedFlags == 0)
				return 0;
			int highestBitSet = (int)Math.Floor(Math.Log(mergedFlags, 2));
			return highestBitSet;
		}

		/// <summary>
		/// Cached result from the last call to Match(String) function
		/// </summary>
		public bool MatchesFilter
		{
			get { return matchesFilter; }
		}

		private List<LogEntry> logEntries;
		private uint mergedFlags;
		private bool matchesFilter;
	}
}
