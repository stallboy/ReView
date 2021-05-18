using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReViewBinaryStorage
{
	public class BinaryStorage
	{
		public BinaryStorage(long maximumSizeInBytes)
		{
			currentSize = 0;
			maximumSize = maximumSizeInBytes;
		}

		public void Clear()
		{
			data.Clear();
		}

		public long MaximumSize
		{
			get
			{
				return maximumSize;
			}
			set
			{
				if (maximumSize != value)
				{
					maximumSize = value;

					// Make sure we're within new memory limits
					EnsureMemoryLimit();
				}
			}
		}

		private void EnsureMemoryLimit()
		{
			while (currentSize > maximumSize && data.Count() > 0)
			{
				foreach (List<BinaryData> binaryDataHistory in data.Values)
				{
					if (binaryDataHistory.Count > 0)
					{
						// Remove first element (oldest) from each ID
						currentSize -= binaryDataHistory.First().Size;
						binaryDataHistory.RemoveAt(0);
					}
				}
			}
		}

		/// <summary>
		/// Store data to binary storage
		/// Storage has a dictionary where IDs are keys and values are lists of BinaryData. The list is time-sorted history of data for that particular ID.
		/// </summary>
		public void StoreData(BinaryData dataToStore)
		{
			lock (dataLock)
			{
				currentSize += dataToStore.Data.Count();

				// Make sure we're within allowed memory limits
				EnsureMemoryLimit();

				List<BinaryData> binaryDataHistory = null;
				if (!data.ContainsKey(dataToStore.Id))
				{
					binaryDataHistory = new List<BinaryData>();
					data.Add(dataToStore.Id, binaryDataHistory);
				}
				else
				{
					binaryDataHistory = data[dataToStore.Id];
				}

				if (binaryDataHistory.Count() == 0 || dataToStore.Time > binaryDataHistory.Last().Time)
				{
					// Add as last element
					binaryDataHistory.Add(dataToStore);
				}
				else
				{
					// Adding data somewhere in the middle of the history, find index by using binary search
					int index = binaryDataHistory.BinarySearch(dataToStore);
					if (index < 0)
					{
						// No exact match, take complement from the index to use as insertion point
						index = ~index;
					}
					binaryDataHistory.Insert(index, dataToStore);
				}
			}
		}

		/// <summary>
		/// Get all data for given time as a flat list, finds entry for each ID stored in the binary storage.
		/// Will fetch the closest matching data for this time.
		/// </summary>
		public List<BinaryData> GetData(int time)
		{
			BinaryData compareToData = new BinaryData(-1, time, null);
			List<BinaryData> binaryDataList = new List<BinaryData>();
			foreach (List<BinaryData> binaryDataHistory in data.Values)
			{
				int index = binaryDataHistory.BinarySearch(compareToData);
				if (index < 0)
				{
					index = ~index;
				}
				if (index < binaryDataHistory.Count())
				{
					binaryDataList.Add(binaryDataHistory[index]);
				}
			}
			
			return binaryDataList;
		}

		public int GetNextTimelineEventTime(int time) 
		{
			BinaryData compareToData = new BinaryData(-1, time, null);
			int nextTime = int.MaxValue;
			foreach (List<BinaryData> binaryDataHistory in data.Values)
			{
				int index = binaryDataHistory.BinarySearch(compareToData);
				if (index < 0)
				{
					index = ~index;
				}
				if (index < binaryDataHistory.Count() - 1)
				{
					nextTime = Math.Min(binaryDataHistory[index + 1].Time, nextTime);
					if (nextTime == time + 1)
					{
						// We can't get a better result
						return nextTime;
					}
				}
			}

			return nextTime != int.MaxValue ? nextTime : -1;
		}

		public int GetPrevTimelineEventTime(int time) 
		{
			BinaryData compareToData = new BinaryData(-1, time, null);
			int prevTime = -1;
			foreach (List<BinaryData> binaryDataHistory in data.Values)
			{
				int index = binaryDataHistory.BinarySearch(compareToData);
				if (index < 0)
				{
					index = ~index;
				}
				if (index < binaryDataHistory.Count() && index > 0)
				{
					prevTime = Math.Max(binaryDataHistory[index - 1].Time, prevTime);
					if (prevTime == time - 1)
					{
						// We can't get a better result
						return prevTime;
					}
				}
			}

			return prevTime;
		}

		private long maximumSize; // Maximum size of this binary storage (in bytes)
		private long currentSize; // Current (approximate) size of this binary storage (in bytes)

		private object dataLock = new object();
		private Dictionary<long, List<BinaryData>> data = new Dictionary<long, List<BinaryData>>();
	}
}
