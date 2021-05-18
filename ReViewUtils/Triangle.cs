using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public class Triangle
	{
		public Triangle(int a, int b, int c)
		{
			A = a;
			B = b;
			C = c;
		}

		public int A
		{
			get;
			set;
		}

		public int B
		{
			get;
			set;
		}

		public int C
		{
			get;
			set;
		}

		public Vector3 GetNormal(List<Vector3> points)
		{
			Vector3 a2b = points[B] - points[A];
			Vector3 a2c = points[C] - points[A];
			return a2b.Cross(a2c).GetNormalized();
		}

		public bool GetBarycentricCoordinates(List<Vector2> points, Vector2 point, out double t, out double u, out double v)
		{
			Vector2 a = points[A];
			Vector2 b = points[B];
			Vector2 c = points[C];

			return GeometryMath.GetBarycentricCoordinates(point, a, b, c, out t, out u, out v);
		}

		public bool PointInFront(List<Vector3> points, Vector3 point)
		{
			Vector3 a2point = point - points[A];
			return a2point.Dot(GetNormal(points)) >= 0.0;
		}

		public bool Contains(List<Vector2> points, Vector2 point)
		{
			double t, u, v;
			if (GetBarycentricCoordinates(points, point, out t, out u, out v))
			{
				return t >= 0 && t <= 1 && u >= 0 && u <= 1 && v >= 0 && v <= 1;
			}
			return false;
		}

		public bool Inside(List<Vector2> points, Vector2 point)
		{
			double t, u, v;
			if (GetBarycentricCoordinates(points, point, out t, out u, out v))
			{
				return t > 0 && t < 1 && u > 0 && u < 1 && v > 0 && v < 1;
			}
			return false;
		}

		public bool PointInCircumcircle(List<Vector2> points, Vector2 point)
		{
			Vector2 a = points[A];
			Vector2 b = points[B];
			Vector2 c = points[C];

			return GeometryMath.PointInCircumcircle(point, a, b, c);
		}

		public bool Contains(Edge edge)
		{
			if (edge == null)
			{
				return false;
			}
			return (edge.A == A || edge.A == B || edge.A == C) && (edge.B == A || edge.B == B || edge.B == C);
		}

		public override bool Equals(object obj)
		{
			Triangle other = obj as Triangle;
			return (this == other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(Triangle a, Triangle b)
		{
			// Both null -> equal
			if (((Object)a) == null && ((Object)b) == null)
				return true;

			// One of them null -> not equal
			if (((Object)a) == null || ((Object)b) == null)
				return false;

			// Both of then not null -> check values
			if (a.A != b.A && a.A != b.B && a.A != b.C)
				return false;
			if (a.B != b.A && a.B != b.B && a.B != b.C)
				return false;
			if (a.C != b.A && a.C != b.B && a.C != b.C)
				return false;
			return true;
		}

		public static bool operator !=(Triangle a, Triangle b)
		{
			return !(a == b);
		}
	}
}
