using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	/// <summary>
	/// Utility class to do some simple 2D vector calculations.
	/// </summary>
	[Serializable]
	public class Vector2
	{
		// Initialize to zero
		public Vector2()
		{
			x = 0;
			y = 0;
		}

		// Initialize to 'other'
		public Vector2(Vector2 other)
		{
			x = other.x;
			y = other.y;
		}

		// Initialize with x, y
		public Vector2(double inx, double iny)
		{
			x = inx;
			y = iny;
		}

		// Initialize to a point 'magnitude' units away to 'angle' direction from 'start'.
		// 0 angle is to {1, 0}, angles go clock-wise and given in radians.
		public Vector2(Vector2 start, double angle, double magnitude)
		{
			x = (double)(start.x + Math.Cos(angle) * magnitude);
			y = (double)(start.y - Math.Sin(angle) * magnitude);
		}

		public static Vector2 FromAngle(double angle, double magnitude)
		{
			return new Vector2(Math.Cos(angle) * magnitude, -Math.Sin(angle) * magnitude);
		}

		// Give vector with random x, y components that are between 0..inMaxX, 0..inMaxY
		public static Vector2 Random(double inMaxX, double inMaxY)
		{
			return new Vector2(SRandom.Double(inMaxX), SRandom.Double(inMaxY));
		}

		// Give random unit vector
		public static Vector2 RandomUnitVector()
		{
			Vector2 randomVector = new Vector2(SRandom.Double(1.0) - 0.5, SRandom.Double(1.0) - 0.5);
			if (randomVector.IsZero())
				randomVector.x = 1.0;
			return randomVector.Normalize();
		}

		// Get angle of this vector in radians (0..2PI). 0 is {1, 0} goes clock-wise.
		public double GetAngle()
		{
			double dot = Normalize().Dot(new Vector2(1.0f, 0.0f));
			double angle = (double)Math.Acos(Math.Max(-1.0f, Math.Min(1.0f, dot)));
			return y < 0.0f ? (double)(Math.PI * 2.0f - angle) : angle;
		}

		// Is this vector zero
		public bool IsZero()
		{
			return x == 0.0f && y == 0.0f;
		}

		// Get angle between this and 'other' vector in radians (0..2PI). 0 is {1, 0} goes clock-wise.
		public double AngleTo(Vector2 other)
		{
			Vector2 direction = other - this;
			return direction.Normalize().GetAngle();
		}

		public override bool Equals(object obj)
		{
			Vector2 other = obj as Vector2;
			return (this == other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Distance to 'other'
		public double Distance(Vector2 other)
		{
			return (other - this).Length();
		}

		public double DistanceSquared(Vector2 other)
		{
			return (other - this).LengthSquared();
		}

		// Zero this vector
		public void Zero()
		{
			x = 0.0f;
			y = 0.0f;
		}

		// Set components of this vector
		public void Set(double inx, double iny)
		{
			x = inx;
			y = iny;
		}

		// Get dot product between this and 'other'
		public double Dot(Vector2 other)
		{
			return x * other.x + y * other.y;
		}

		// Get cross product (x, y => y, -x)
		public Vector2 Cross()
		{
			return new Vector2(y, -x);
		}

		// Get cross product with another Vector2
		public double Cross(Vector2 other)
		{
			return x * other.y - y * other.x;
		}

		// Length of this vector
		public double Length()
		{
			return (double)Math.Sqrt(x * x + y * y);
		}

		// Square length of this vector
		public double LengthSquared()
		{
			return (double)(x * x + y * y);
		}

		// Normalize this vector, if zero vector then remains that way
		public Vector2 Normalize()
		{
			double l = Length();
			if (l == 0.0f)
			{
				return new Vector2(0.0f, 0.0f);
			}
			return new Vector2(x / l, y / l);
		}

		// Add vectors
		public static Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x + b.x, a.y + b.y);
		}

		// Subtract vectors
		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x - b.x, a.y - b.y);
		}

		// Dot
		public static double operator *(Vector2 a, Vector2 b)
		{
			return a.Dot(b);
		}

		// Scale vector
		public static Vector2 operator *(Vector2 a, double scalar)
		{
			return new Vector2(a.x * scalar, a.y * scalar);
		}

		// Inverse-scale vector
		public static Vector2 operator /(Vector2 a, double scalar)
		{
			if (scalar != 0.0f)
				return new Vector2(a.x / scalar, a.y / scalar);
			else
				return new Vector2(0.0f, 0.0f);
		}

		// Check equality
		public static bool operator ==(Vector2 a, Vector2 b)
		{
			// Both null -> equal
			if (((Object)a) == null && ((Object)b) == null)
				return true;

			// One of them null -> not equal
			if (((Object)a) == null || ((Object)b) == null)
				return false;

			// Both of then not null -> check values
			return a.x == b.x && a.y == b.y;
		}

		// Check inequality
		public static bool operator !=(Vector2 a, Vector2 b)
		{
			return !(a == b);
		}

		// Invert vector
		public static Vector2 operator -(Vector2 a)
		{
			return new Vector2(-a.x, -a.y);
		}

		// Get vector where all components are the minimum from given vectors (a, b)
		public static Vector2 Min(Vector2 a, Vector2 b)
		{
			return new Vector2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
		}

		// Get vector where all components are the maximum from given vectors (a, b)
		public static Vector2 Max(Vector2 a, Vector2 b)
		{
			return new Vector2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
		}

		// Clamp components of 'a' to 'min'..'max'
		public static Vector2 Clamp(Vector2 a, Vector2 min, Vector2 max)
		{
			return Min(Max(a, min), max);
		}

		// Set vector length, if zero vector remains that way
		public void SetLength(double magnitude)
		{
			double l = Length();
			if (l != 0.0f)
			{
				x = x / l * magnitude;
				y = y / l * magnitude;
			}
		}

		// Snap components to 'scale'. If '1' is given then components are snapped to integer intervals.
		public Vector2 Snap(double scale)
		{
			return new Vector2((double)Math.Round(x / scale) * scale, (double)Math.Round(y / scale) * scale);
		}

		// Get x component as float
		public float X
		{
			get
			{
				return (float)x;
			}
		}

		// Get y component as float
		public float Y
		{
			get
			{
				return (float)y;
			}
		}

		// Convert Vector3 into Vector2
		public static implicit operator Vector2(Vector3 inVector3)
		{
			return new Vector2(inVector3.x, inVector3.y);
		}

		public override string ToString()
		{
			return String.Format("X: {0:0.000} Y: {1:0.000}", x, y);
		}

		public double x;
		public double y;
	}
}
