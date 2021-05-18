using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	/// <summary>
	/// Utility class to do some simple 3D vector calculations
	/// </summary>
	[Serializable]
	public class Vector4
	{
		// Initialize to zero
		public Vector4()
		{
			x = 0;
			y = 0;
			z = 0;
			w = 0;
		}

		// Initialize to 'other'
		public Vector4(Vector4 other)
		{
			x = other.x;
			y = other.y;
			z = other.z;
			w = other.w;
		}

		//  Initialize with x, y, z, w
		public Vector4(double inX, double inY, double inZ, double inW)
		{
			x = inX;
			y = inY;
			z = inZ;
			w = inW;
		}

		// Is this vector zero
		public bool IsZero()
		{
			return x == 0 && y == 0 && z == 0 && w == 0;
		}

		public override bool Equals(object obj)
		{
			Vector4 other = obj as Vector4;
			return (this == other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Zero this vector
		public void Zero()
		{
			x = 0;
			y = 0;
			z = 0;
			w = 0;
		}

		// Convert Vector2 into Vector3, set z to 0
		public static implicit operator Vector4(Vector3 inVector3)
		{
			return new Vector4(inVector3.x, inVector3.y, inVector3.z, 1.0);
		}

		public override string ToString()
		{
			return String.Format("X: {0:0.000} Y: {1:0.000} Z: {2:0.000} W: {3:0.000}", x, y, z, w);
		}

		public double x;
		public double y;
		public double z;
		public double w;
	}
}
