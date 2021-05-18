using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReView
{
	public class SMath
	{
		public static float RAD2DEG = (float)(180.0 / Math.PI);
		public static float DEG2RAD = (float)(Math.PI / 180.0);

		public static float GetDeltaAngle(float inAngleA, float inAngleB)
		{
			float deltaAngle = inAngleB - inAngleA;
			
			if (deltaAngle > 180.0f)
				deltaAngle -= 360.0f;
			if (deltaAngle < -180.0f)
				deltaAngle += 360.0f;

			return deltaAngle;
		}

		public static int Clamp(int value, int min, int max)
		{
			return Math.Min(Math.Max(value, min), max);
		}

		public static float Clamp(float value, float min, float max)
		{
			return Math.Min(Math.Max(value, min), max);
		}

		public static double Clamp(double value, double min, double max)
		{
			return Math.Min(Math.Max(value, min), max);
		}

		/// <summary>
		/// Return angular difference (0..PI)
		/// </summary>
		public static double ShortestAngle(double angleA, double angleB)
		{
			double delta = Math.Abs(angleB - angleA) % (Math.PI * 2.0);
			if (delta > Math.PI)
			{
				delta = Math.PI * 2.0 - delta;
			}

			return delta;
		}

		/// <summary>
		/// Return angular difference (-PI .. PI)
		/// </summary>
		public static double ShortestAngleSigned(double angleA, double angleB)
		{
			double delta = angleA - angleB;
			if (delta > Math.PI)
			{
				return -(Math.PI * 2.0 - delta);
			}
			else if (delta < -Math.PI)
			{
				return (Math.PI * 2.0 + delta);
			}
			return delta;
		}
	}
}
