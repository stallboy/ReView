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
	public class Vector3
	{
		// Initialize to zero
		public Vector3()
		{
			x = 0;
			y = 0;
			z = 0;
		}

		// Initialize to 'other'
		public Vector3(Vector3 other)
		{
			x = other.x;
			y = other.y;
			z = other.z;
		}

		//  Initialize with x, y, z
		public Vector3(double inX, double inY, double inZ)
		{
			x = inX;
			y = inY;
			z = inZ;
		}

		public Vector3 Rcp()
		{
			return new Vector3(1.0 / x, 1.0 / y, 1.0 / z);
		}

		public Vector3 Sign()
		{
			return new Vector3(Math.Sign(x), Math.Sign(y), Math.Sign(z));
		}

		public static Vector3 Lerp(Vector3 a, Vector3 b, double t)
		{
			return a + (b - a) * t;
		}

		// Get distance to 'other'
		public double Distance(Vector3 other)
		{
			return (this - other).Length();
		}

		// Get square distance to 'other'
		public double DistanceSquared(Vector3 other)
		{
			return (this - other).LengthSquared();
		}

		// Give random unit vector
		public static Vector3 RandomUnitVector()
		{
			Vector3 randomVector = new Vector3(SRandom.Double(1.0) - 1.0, SRandom.Double(1.0) - 1.0, SRandom.Double(1.0) - 1.0);
			if (randomVector.IsZero())
				randomVector.x = 1.0;
			return randomVector.GetNormalized();
		}

		public static Vector3 Random(double inMaxX, double inMaxY, double inMaxZ)
		{
			return new Vector3(SRandom.Double(inMaxX), SRandom.Double(inMaxY), SRandom.Double(inMaxZ));
		}

		// Is this vector zero
		public bool IsZero()
		{
			return x == 0 && y == 0 && z == 0;
		}

		// Get angle between this and 'other' vector in radians (0..2PI). 0 is {1, 0} goes clock-wise.
		public double AngleTo(Vector3 other)
		{
			return Math.Acos(SMath.Clamp(Dot(other), -1, 1));
		}

		public override bool Equals(object obj)
		{
			Vector3 other = obj as Vector3;
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
		}

		// Set components of this vector
		public void Set(double inX, double inY, double inZ)
		{
			x = inX;
			y = inY;
			z = inZ;
		}

		// Get dot product between this and 'other'
		public double Dot(Vector3 other)
		{
			return x * other.x + y * other.y + z * other.z;
		}

		// Get cross product between this and 'other'
		public Vector3 Cross(Vector3 other)
		{
			return new Vector3(y * other.z - z * other.y, z * other.x - x * other.z, x * other.y - y * other.x);
		}

		// Length of this vector
		public double Length()
		{
			return (double)Math.Sqrt(x * x + y * y + z * z);
		}

		// Returns value of the component with maximum value
		public double MaxComponent()
		{
			return Math.Max(x, Math.Max(y, z));
		}

		// Returns value of the component with minimum value
		public double MinComponent()
		{
			return Math.Min(x, Math.Min(y, z));
		}

		// Square length of this vector
		public double LengthSquared()
		{
			return (double)(x * x + y * y + z * z);
		}

		// Normalize this vector, if zero vector then remains that way
		public Vector3 GetNormalized()
		{
			double l = Length();
			if (l == 0)
			{
				return new Vector3();
			}
			return new Vector3(x / l, y / l, z / l);
		}

		// Add vectors
		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		// Subtract vectors
		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		// Dot
		public static double operator *(Vector3 a, Vector3 b)
		{
			return a.Dot(b);
		}

		public Vector3 Scale(Vector3 other)
		{
			return new Vector3(x * other.x, y * other.y, z * other.z);
		}

		// Scale vector
		public static Vector3 operator *(Vector3 a, double scalar)
		{
			return new Vector3(a.x * scalar, a.y * scalar, a.z * scalar);
		}

		// Inverse-scale vector
		public static Vector3 operator /(Vector3 a, double scalar)
		{
			if (scalar != 0)
				return new Vector3(a.x / scalar, a.y / scalar, a.z / scalar);
			else
				return new Vector3();
		}

		// Check equality
		public static bool operator ==(Vector3 a, Vector3 b)
		{
			// Both null -> equal
			if (((Object)a) == null && ((Object)b) == null)
				return true;

			// One of them null -> not equal
			if (((Object)a) == null || ((Object)b) == null)
				return false;

			// Both of then not null -> check values
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		// Check inequality
		public static bool operator !=(Vector3 a, Vector3 b)
		{
			return !(a == b);
		}

		// Invert vector
		public static Vector3 operator -(Vector3 a)
		{
			return new Vector3(-a.x, -a.y, -a.z);
		}

		// Get vector where all components are the minimum from given vectors (a, b)
		public static Vector3 Min(Vector3 a, Vector3 b)
		{
			return new Vector3(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
		}

		// Get vector where all components are the maximum from given vectors (a, b)
		public static Vector3 Max(Vector3 a, Vector3 b)
		{
			return new Vector3(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
		}

		// Clamp components of 'a' to 'min'..'max'
		public static Vector3 Clamp(Vector3 a, Vector3 min, Vector3 max)
		{
			return Min(Max(a, min), max);
		}

		// Set vector length, if zero vector remains that way
		public void SetLength(double magnitude)
		{
			double l = Length();
			if (l != 0)
			{
				x = x / l * magnitude;
				y = y / l * magnitude;
				z = z / l * magnitude;
			}
		}

		// Snap components to 'scale'. If '1' is given then components are snapped to integer intervals
		public Vector3 Snap(double scale)
		{
			return new Vector3(Math.Round(x / scale) * scale, Math.Round(y / scale) * scale, Math.Round(z / scale) * scale);
		}

		// Convert Vector2 into Vector3, set z to 0
		public static implicit operator Vector3(Vector2 inVector2)
		{
			return new Vector3(inVector2.x, inVector2.y, 0.0);
		}

		public override string ToString()
		{
			return String.Format("X: {0:0.000} Y: {1:0.000} Z: {2:0.000}", x, y, z);
		}

		public double x;
		public double y;
		public double z;
	}
}
