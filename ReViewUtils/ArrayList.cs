using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public class ArrayList<T>
	{
		public ArrayList(int capacity = 32)
		{
			data = new T[capacity];
			count = 0;
		}

		public void Add(T item)
		{
			if (data.Length == count)
			{
				// Need to resize (double size)
				Array.Resize(ref data, data.Length * 2);
			}

			data[count] = item;
			count++;
		}

		public T[] BackingArray 
		{ 
			get 
			{ 
				return data; 
			}
		}

		public T this[int index]
		{
			get
			{
				return data[index];
			}
			set
			{
				data[index] = value;
			}
		}

		public int Count 
		{ 
			get 
			{ 
				return count; 
			}
		}

		private T[] data;
		private int count;
	}
}
