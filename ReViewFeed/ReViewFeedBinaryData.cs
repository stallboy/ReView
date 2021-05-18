using System;
using System.Collections.Generic;
using System.Text;

namespace ReView
{
	public class ReViewFeedBinaryData
	{
		public void Store(int time, ref byte[] data)
		{
			ReViewFeedManager.Instance.StoreBinaryData(DebugID, time, ref data);
		}

		public void NotifyDataReceived(int time, ref byte[] data)
		{
			DlgDataReceived handler = OnDataReceived;
			if (handler != null)
			{
				handler(time, ref data);
			}
		}

		public delegate void DlgDataReceived(int time, ref byte[] data);

		// Events
		public event DlgDataReceived OnDataReceived;

		/// <summary>
		/// DebugID created for this ReViewFeedObject
		/// </summary>
		public long DebugID
		{
			get
			{
				return debugID;
			}
			set
			{
				if (debugID == -1)
				{
					debugID = value;
				}
			}
		}

		private long debugID = -1;
	}
}
