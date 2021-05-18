using _TestGame.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Data
{
	public class Blackboard
	{
		public float GetFloat(string inName)
		{
			if (floatValues.ContainsKey(inName))
			{
				return floatValues[inName];
			}
			return -1.0f;
		}

		public void SetFloat(string inName, float inValue)
		{
			floatValues[inName] = inValue;
		}

		public GameObject Target
		{
			get;
			set;
		}

		private Dictionary<string, float> floatValues = new Dictionary<string,float>();
	}
}
