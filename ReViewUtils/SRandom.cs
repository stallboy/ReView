using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReView
{
	public class SRandom
	{
		public static double Double(double inMax)
		{
			return randomGenerator.NextDouble() * inMax;
		}

		public static float Float()
		{
			return (float)randomGenerator.NextDouble();
		}

		public static float Float(float inMax)
		{
			return (float)randomGenerator.NextDouble() * inMax;
		}

		public static float Float(float inMin, float inMax)
		{
			return (float)randomGenerator.NextDouble() * (inMax - inMin) + inMin;
		}

		public static int Index(int inSize)
		{
			return ((int)Math.Round(randomGenerator.NextDouble() * Int32.MaxValue)) % inSize;
		}

		public static int Int(int inMax)
		{
			return (int)Math.Round(randomGenerator.NextDouble() * inMax);
		}

		public static int Int(int inMin, int inMax)
		{
			return (int)(randomGenerator.NextDouble() * (inMax - inMin) + inMin);
		}

		private static System.Random randomGenerator = new System.Random();
	}
}
