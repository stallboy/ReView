using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public interface LogWriter
	{
		void Write(string text);
	}

	public class ConsoleLogWriter : LogWriter
	{
		public ConsoleLogWriter()
		{
		}

		public void Write(string text)
		{
			Console.Write(text);
		}
	}

	public class FileLogWriter : LogWriter
	{
		public FileLogWriter(string path)
		{
			Path = path;
		}

		public string Path
		{
			get;
			private set;
		}

		public void Write(string text)
		{
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(text))
			{
				file.Write(text);
			}
		}
	}

	public class Log
	{
		public static void AddConsoleWriter()
		{
			if (!HasConsoleWriter())
			{
				AddWriter(new ConsoleLogWriter());
			}
		}

		public static void AddFileWriter(string path)
		{
			if (!HasFileWriter(path))
			{
				AddWriter(new FileLogWriter(path));
			}
		}

		public static bool HasConsoleWriter()
		{
			foreach (LogWriter writer in LogWriters)
			{
				if (writer is ConsoleLogWriter)
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasFileWriter(string path)
		{
			foreach (LogWriter writer in LogWriters)
			{
				FileLogWriter fileWriter = writer as FileLogWriter;
				if (fileWriter != null && fileWriter.Path.ToLower().CompareTo(path.ToLower()) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public static string GetTime()
		{
			return ShowTime ? "[" + DateTime.Now.ToShortTimeString() + "]" : "";
		}

		public static void WriteLine(string line)
		{
			Write(GetTime() + line + Environment.NewLine);
		}

		public static void Write(string text)
		{
			foreach (LogWriter writer in LogWriters)
			{
				writer.Write(text);
			}
		}

		public static void WriteException(Exception e)
		{
			WriteLine(e.Message + Environment.NewLine + e.Source + Environment.NewLine + e.StackTrace);
		}

		public static void AddWriter(LogWriter writer)
		{
			LogWriters.Add(writer);
		}

		public static void RemoveWriter(LogWriter writer)
		{
			LogWriters.Remove(writer);
		}

		public static bool ShowTime
		{
			get;
			set;
		}

		public static List<LogWriter> LogWriters = new List<LogWriter>();
	}
}
