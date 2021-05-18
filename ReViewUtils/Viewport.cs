using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public class Viewport
	{
		public double x
		{
			get { return data[0]; }
		}
		
		public double y
		{
			get { return data[1]; }
		}

		public double width
		{
			get { return data[2]; }
		}
		
		public double height
		{
			get { return data[3]; }
		}

		public int[] data = new int[4];
	}
}
