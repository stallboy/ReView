using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReViewBinaryStorage
{
	public struct BinaryData : IComparable<BinaryData>
	{
		public BinaryData(long id, int time, byte[] data) : this()
		{
			Id = id;
			Time = time;
			Data = data;
		}

		public int CompareTo(BinaryData other)
		{
			return Time < other.Time ? -1 : Time == other.Time ? 0 : 1;
		}


		/// <summary>
		/// Time for the entry
		/// </summary>
		public int Time
		{
			get;
			set;
		}

		/// <summary>
		/// Id for the entry
		/// </summary>
		public long Id
		{
			get;
			set;
		}

		/// <summary>
		/// Data for the entry
		/// </summary>
		public byte[] Data
		{
			get;
			set;
		}

		public int Size
		{
			get
			{
				return Data.Length;
			}
		}
	}
}
