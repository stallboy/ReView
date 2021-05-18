using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	/// <summary>
	/// Utility class to do some simple bounds calculations.
	/// </summary>
	[Serializable]
	public class Bounds
	{
		// Initialize to maximum inverse size (min = Double.MaxValue, max = -Double.MaxValue)
		public Bounds()
		{
			Min = new Vector3(Double.MaxValue, Double.MaxValue, Double.MaxValue);
			Max = new Vector3(-Double.MaxValue, -Double.MaxValue, -Double.MaxValue);
		}

		// Initialize with min, max
		public Bounds(Vector3 min, Vector3 max)
		{
			Min = min;
			Max = max;
		}

		public bool IsValid
		{
			get
			{
				return Min.x <= Max.x && Min.y <= Max.y && Min.z <= Max.z;
			}
		}

		// Encapsulate bounds into bounds
		public void Encapsulate(Bounds bounds)
		{
			if (bounds.IsValid)
			{
				Min = Vector3.Min(Min, bounds.Min);
				Max = Vector3.Max(Max, bounds.Max);
			}
		}

		// Encapsulate point into bounds
		public void Encapsulate(Vector3 point)
		{
			Min = Vector3.Min(Min, point);
			Max = Vector3.Max(Max, point);
		}

		// Get center of the bounds
		public Vector3 Center
		{
			get
			{
				return (Min + Max) * 0.5;
			}
		}

		// Get half size of the bounds
		public Vector3 HalfSize
		{
			get
			{
				return (Max - Min) * 0.5;
			}
		}

		public static Bounds operator *(Bounds bounds, double scale)
		{
			Vector3 newHalfSize = bounds.HalfSize * scale;
			return new Bounds(bounds.Center - newHalfSize, bounds.Center + newHalfSize);
		}

		// Transform bounds with matrix
		public static Bounds operator *(Matrix4x4 mat, Bounds bounds)
		{
			// Transform bounds with matrix, then calculate new bounds from those transformed values

			// Create vectors for each half size components
			Vector3 a = mat * new Vector3(bounds.Min.x, bounds.Min.y, bounds.Min.z);
			Vector3 b = mat * new Vector3(bounds.Max.x, bounds.Min.y, bounds.Min.z);

			Vector3 c = mat * new Vector3(bounds.Min.x, bounds.Max.y, bounds.Min.z);
			Vector3 d = mat * new Vector3(bounds.Max.x, bounds.Max.y, bounds.Min.z);

			Vector3 e = mat * new Vector3(bounds.Min.x, bounds.Min.y, bounds.Max.z);
			Vector3 f = mat * new Vector3(bounds.Max.x, bounds.Min.y, bounds.Max.z);

			Vector3 g = mat * new Vector3(bounds.Min.x, bounds.Max.y, bounds.Max.z);
			Vector3 h = mat * new Vector3(bounds.Max.x, bounds.Max.y, bounds.Max.z);


			// Get min/max of the transformed component vectors and their inverses
			Vector3 min = new Vector3(Double.MaxValue, Double.MaxValue, Double.MaxValue);
			Vector3 max = new Vector3(-Double.MaxValue, -Double.MaxValue, -Double.MaxValue);

			min = Vector3.Min(min, a);
			min = Vector3.Min(min, b);
			min = Vector3.Min(min, c);
			min = Vector3.Min(min, d);
			min = Vector3.Min(min, e);
			min = Vector3.Min(min, f);
			min = Vector3.Min(min, g);
			min = Vector3.Min(min, h);

			max = Vector3.Max(max, a);
			max = Vector3.Max(max, b);
			max = Vector3.Max(max, c);
			max = Vector3.Max(max, d);
			max = Vector3.Max(max, e);
			max = Vector3.Max(max, f);
			max = Vector3.Max(max, g);
			max = Vector3.Max(max, h);

			// New world space bounds
			return new Bounds(min, max);
		}

		public Vector3 Min;
		public Vector3 Max;
	}
}
