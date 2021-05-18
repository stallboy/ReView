using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public class Edge
	{
		public Edge(int a, int b)
		{
			A = a;
			B = b;
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

		public Edge GetFlipped()
		{
			return new Edge(B, A);
		}

		public Vector2 GetNormal(List<Vector2> points)
		{
			Vector2 edgeVector = points[B] - points[A];
			return edgeVector.Cross().Normalize();
		}

		public bool PointInFront(List<Vector2> points, Vector2 point)
		{
			Vector2 a2point = point - points[A];
			double dot = a2point.Dot(GetNormal(points));
			if (dot == 0.0)
			{
				// On the line, check if in segment
				double distToA = point.DistanceSquared(points[A]);
				double distToB = point.DistanceSquared(points[B]);
				double lineLength = points[A].DistanceSquared(points[B]);
				// If within segment -> Accept "in front"
				return (distToA <= lineLength && distToB <= lineLength);
			}
			return dot > 0.0;
		}

		public bool PointInLine(List<Vector2> points, Vector2 point)
		{
			Vector2 a2point = point - points[A];
			return (a2point.Dot(GetNormal(points)) == 0.0) ;
		}

		public double DistanceToPoint(List<Vector2> points, Vector2 point)
		{
			Vector2 a = points[A];
			Vector2 b = points[B];
			Vector2 a2point = point - a;
			Vector2 b2point = point - b;
			Vector2 a2b = (b - a);

			// Calculate the normalized position on line segment (0..1)
			double t = a2point.Dot(a2b) / a2b.LengthSquared();

			if (t < 0.0)
			{
				// Projection would be past point A, return distance to point A
				return a2point.Length();
			}

			if (t > 1.0)
			{
				// Projection would be past point B, return distance to point B
				return b2point.Length();
			}

			// Calculate projection on line segment and then return distance to that point
			Vector2 projection = a + a2b * t;
			return (projection - point).Length();
		}

		public bool Contains(int index)
		{
			return (index == A) || (index == B);
		}

		public override bool Equals(object obj)
		{
			Edge other = obj as Edge;
			return (this == other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(Edge a, Edge b)
		{
			// Both null -> equal
			if (((Object)a) == null && ((Object)b) == null)
				return true;

			// One of them null -> not equal
			if (((Object)a) == null || ((Object)b) == null)
				return false;

			// Both of then not null -> check values
			return (a.A == b.A && a.B == b.B) || (a.A == b.B && a.B == b.A);
		}

		public static bool operator !=(Edge a, Edge b)
		{
			return !(a == b);
		}
	}
}
