using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public class TrackableObject
	{
		public TrackableObject(long id)
		{
			Id = id;
		}

		public long Id
		{
			get;
			private set;
		}
	}
}
