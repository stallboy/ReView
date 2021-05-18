using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	/// <summary>
	/// Utility class to do some simple quaternion calculations.
	/// </summary>
	[Serializable]
	public class Quaternion
	{
		// Initialize to 'zero' quaternion
		public Quaternion()
		{
			x = 0;
			y = 0;
			z = 0;
			w = 1;
		}

		// Initialize to x, y, z, w
		public Quaternion(double inX, double inY, double inZ, double inW)
		{
			x = inX;
			y = inY;
			z = inZ;
			w = inW;
		}

		// Initialize with axis and angle (in radians), does not normalize
		public Quaternion(Vector3 axis, double angle)
		{
			double cos_half_angle = Math.Cos(angle / 2.0);
			double sin_half_angle = Math.Sin(angle / 2.0);
			w = cos_half_angle;
			x = sin_half_angle * axis.x;
			y = sin_half_angle * axis.y;
			z = sin_half_angle * axis.z;
		}

		// Normalize quaternion, if zero length remains that way
		public Quaternion Normalize()
		{
			double l = Length();
			if (l != 0)
			{
				x /= l;
				y /= l;
				z /= l;
				w /= l;
			}
			return this;
		}

		// Get length of quaternion
		public double Length()
		{
			return Math.Sqrt(x * x + y * y + z * z + w * w);
		}

		// Get square length of quaternion
		public double SquareLength()
		{
			return x * x + y * y + z * z + w * w;
		}

		// Check equality
		public static bool operator ==(Quaternion a, Quaternion b)
		{
			// Both null -> equal
			if (((Object)a) == null && ((Object)b) == null)
				return true;

			// One of them null -> not equal
			if (((Object)a) == null || ((Object)b) == null)
				return false;

			// Both of then not null -> check values
			return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
		}

		// Check inequality
		public static bool operator !=(Quaternion a, Quaternion b)
		{
			return !(a == b);
		}

		// Scale quaternion
		public static Quaternion operator *(Quaternion a, double b)
		{
			return new Quaternion(a.x * b, a.y * b, a.z * b, a.w * b);
		}

		// Inverse scale quaternion
		public static Quaternion operator /(Quaternion a, double b)
		{
			return new Quaternion(a.x / b, a.y / b, a.z / b, a.w / b);
		}
		
		// 'Add' two quaternions
		public static Quaternion operator *(Quaternion a, Quaternion b)
		{
			Quaternion result = new Quaternion();
			result.x = a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y;
			result.y = a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x;
			result.z = a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w;
			result.w = a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z;
			return result;
		}

		// Conjugate quaternion
		public Quaternion Conjugate()
		{
			return new Quaternion(-x, -y, -z, w);
		}

		// Invert quaternion
		public Quaternion Inverse()
		{
			double sqLength = SquareLength();
			if (sqLength == 0)
				return new Quaternion(0, 0, 0, 0);
			return Conjugate() / sqLength;
		}

		public override bool Equals(object obj)
		{
			Quaternion other = obj as Quaternion;
			return (this == other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public double x, y, z, w;
	}
}
