using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public class Annotation
	{
		public Annotation(string content, int time)
		{
			this.content = content;
			this.time = time;
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

		private string content;
		private int time;
	}
}
