using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReView
{
	public class GeometryMath
	{
		// Get shortest distance between two lines.
		// You can specify whether line A or B is a segment or "infinite" length.
		// When line is restricted to a segment the closest point is clamped to the line.
		// http://geomalgorithms.com/a07-_distance.html#dist3D_Segment_to_Segment
		public static double LineLineDistance(Vector3 startA, Vector3 endA, Vector3 startB, Vector3 endB, bool isSegmentA, bool isSegmentB, out double t, out double s)
		{
			Vector3 u = endA - startA;
			Vector3 v = endB - startB;
			Vector3 w = startA - startB;

			double a = u.Dot(u);
			double b = u.Dot(v);
			double c = v.Dot(v);
			double d = u.Dot(w);
			double e = v.Dot(w);
			double D = a * c - b * b;

			if (D == 0.0)
			{
				t = 0.0;
				s = (b > c ? d / b : e / c);
			}
			else
			{
				t = (b * e - c * d) / D;
				s = (a * e - b * d) / D;
			}

			// Clamp s and t to test segment vs segment distance
			if (isSegmentA)
			{
				t = SMath.Clamp(t, 0.0, u.Length());
			}
			if (isSegmentB)
			{
				s = SMath.Clamp(s, 0.0, w.Length());
			}

			Vector3 intersectionA = startA + u * t;
			Vector3 intersectionB = startB + v * s;
			
			return (intersectionB - intersectionA).Length();
		}

		// Get intersection of two infinite lines
		// http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
		public static bool Intersect(Vector2 startA, Vector2 directionA, Vector2 startB, Vector2 directionB, out Vector2 intersection)
		{
			intersection = new Vector2();

			Vector2 a2b = startB - startA;
			double cross = directionA.Cross(directionB);
			if (cross != 0.0)
			{
				// Intersection
				intersection = startA + directionA * (a2b.Cross(directionB) / cross);
				return true;
			}

			// Co-linear -> No intersection
			return false;
		}

		// Ray - Triangle intersection check
		public static bool Intersect(Vector3 lineStart, Vector3 lineEnd, Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 intersection;
			if (Intersect(lineStart, lineEnd - lineStart, a, b, c, out intersection))
			{
				return lineStart.DistanceSquared(intersection) <= lineStart.DistanceSquared(lineEnd);
			}

			return false;
		}

		// Ray - Triangle intersection check
		// http://www.lighthouse3d.com/tutorials/maths/ray-triangle-intersection/
		public static bool Intersect(Vector3 rayStart, Vector3 rayDir, Vector3 a, Vector3 b, Vector3 c, out Vector3 intersection)
		{
			intersection = new Vector3(0, 0, 0);

			Vector3 a2b = b - a;
			Vector3 a2c = c - a;

			Vector3 h = rayDir.Cross(a2c);
			double dot = a2b.Dot(h);

			if (dot == 0.0)
			{
				// Co-linear -> No intersection
				return false;
			}

			Vector3 a2rayStart = rayStart - a;
			double u = a2rayStart.Dot(h) / dot;

			if (u < 0.0 || u > 1.0)
			{
				// Ray does not hit triangle
				return false;
			}

			Vector3 q = a2rayStart.Cross(a2b);
			double v = rayDir.Dot(q) / dot;

			if (v < 0.0 || u + v > 1.0)
			{
				// Ray does not hit triangle
				return false;
			}

			double t = a2c.Dot(q) / dot;
			if (t < 0.0)
			{
				// Ray hits triangle but intersection point is behind the ray
				return false;
			}

			intersection = rayStart + rayDir * t;

			// Hit
			return true;
		}

		// Check intersection of ray and AABB
		// http://www.scratchapixel.com/lessons/3d-basic-lessons/lesson-7-intersecting-simple-shapes/ray-box-intersection/
		// An Efficient and Robust Ray–Box Intersection Algorithm, Amy Williams et al. 2004.
		public static bool IntersectAABB(Vector3 rayStart, Vector3 rayDir, Vector3 min, Vector3 max, out double tmin, out double tmax)
		{
			tmin = tmax = 0.0;

			Vector3 inv = rayDir.Rcp();

			double txmin = ((rayDir.x < 0 ? max.x : min.x) - rayStart.x) * inv.x;
			double txmax = ((rayDir.x < 0 ? min.x : max.x) - rayStart.x) * inv.x;

			double tymin = ((rayDir.y < 0 ? max.y : min.y) - rayStart.y) * inv.y;
			double tymax = ((rayDir.y < 0 ? min.y : max.y) - rayStart.y) * inv.y;

			if ((txmin > tymax) || (tymin > txmax))
			{
				return false;
			}

			if (tymin > txmin)
				txmin = tymin;

			if (tymax < txmax)
				txmax = tymax;

			double tzmin = ((rayDir.z < 0 ? max.z : min.z) - rayStart.z) * inv.z;
			double tzmax = ((rayDir.z < 0 ? min.z : max.z) - rayStart.z) * inv.z;

			if ((txmin > tzmax) || (tzmin > txmax))
			{
				return false;
			}

			if (tzmin > txmin)
				txmin = tzmin;

			if (tzmax < txmax)
				txmax = tzmax;

			if (txmin < 0 && txmax < 0)
			{
				// Ray going away from AABB
				return false;
			}

			tmin = txmin;
			tmax = txmax;

			return true;
		}

		// OpenGL Project
		// https://www.opengl.org/wiki/GluProject_and_gluUnProject_code
		public static Vector3 Project(Vector3 position, Matrix4x4 transformMatrix, Viewport viewport)
		{
			Vector3 projected = new Vector3();
			Vector4 transformedPosition = transformMatrix * new Vector4(position);
			transformedPosition.x /= transformedPosition.w;
			transformedPosition.y /= transformedPosition.w;
			transformedPosition.z /= transformedPosition.w;
			projected.x = (viewport.x + viewport.width * (transformedPosition.x * 0.5 + 0.5));
			projected.y = (viewport.y + viewport.height * (transformedPosition.y * 0.5 + 0.5));
			projected.z = (transformedPosition.z + 1.0) / 2.0 * Math.Sign(transformedPosition.w);
			return projected;
		}

		// OpenGL Unproject
		// https://www.opengl.org/wiki/GluProject_and_gluUnProject_code
		public static Vector3 Unproject(Vector3 screenPosition, Matrix4x4 transformMatrix, Viewport viewport)
		{
			Matrix4x4 inverseTransform = transformMatrix.Invert();
			Vector4 translatedPosition = new Vector4(	(screenPosition.x - viewport.x) / viewport.width * 2.0 - 1.0,
														(screenPosition.y - viewport.y) / viewport.height * 2.0 - 1.0,
														screenPosition.z * 2.0 - 1.0,
														1.0);
			Vector4 transformed = inverseTransform * translatedPosition;
			if (transformed.w == 0.0)
			{
				return new Vector3(0, 0, 0);
			}

			return new Vector3(transformed.x / transformed.w, transformed.y / transformed.w, transformed.z / transformed.w);
		}

		public static double GetTriangleArea(Vector3 a, Vector3 b, Vector3 c)
		{
			double halfPerim = GetTrianglePerimeter(a, b, c) / 2.0;
			return Math.Sqrt(halfPerim * (halfPerim - (b - a).Length()) * (halfPerim - (b - c).Length()) * (halfPerim - (c - a).Length()));
		}

		public static double GetTrianglePerimeter(Vector3 a, Vector3 b, Vector3 c)
		{
			return (b - a).Length() + (c - a).Length() + (b - c).Length();
		}

		public static bool IsTriangleDegenerate(Vector3 a, Vector3 b, Vector3 c)
		{
			return a.DistanceSquared(b) == 0.0 || a.DistanceSquared(c) == 0.0 || b.DistanceSquared(c) == 0.0 || (b - a).Dot(c - a) == 1.0 || (b - a).Dot(c - a) == -1.0;
		}

		public static bool GetCircumcircle(Vector2 a, Vector2 b, Vector2 c, out Vector2 center, out double radius)
		{
			Vector2 edgeA = (b - a);
			Vector2 edgeB = (c - b);
			Vector2 edgeC = (a - c);

			Vector2 edgeCenterA = (a + b) * 0.5;
			Vector2 edgeCenterB = (b + c) * 0.5;

			if (GeometryMath.Intersect(edgeCenterA, edgeA.Cross(), edgeCenterB, edgeB.Cross(), out center))
			{
				double lengthA = edgeA.Length();
				double lengthB = edgeB.Length();
				double lengthC = edgeC.Length();

				double divider = Math.Sqrt((lengthA + lengthB + lengthC) * (lengthB + lengthC - lengthA) * (lengthA + lengthC - lengthB) * (lengthA + lengthB - lengthC));
				if (divider > 0)
				{
					radius = (lengthA * lengthB * lengthC) / divider;

					return true;
				}
			}

			radius = 0.0;

			// All triangle points are on a line -> Infinite radius of circumcircle, cannot calculate radius
			return false;
		}

		// Check point being inside the circumcircle of this triangle
		// You should check that not all points of the triangle lie on a line and that all sides have length greater than 0.
		// If all points lie on a line or one side has length of 0 this method will return 'true' as the circumcircle would have inifite radius.
		// http://www.mathopenref.com/trianglecircumcircle.html
		public static bool PointInCircumcircle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
		{
			Vector2 center;
			double radius;
			if (!GetCircumcircle(a, b, c, out center, out radius))
			{
				// If cannot get circumcircle (infinite radius) then return 'true' as all points would be in circumcircle
				return true;
			}
		
			return (point.Distance(center) <= radius);
		}

		// Get barycentric coordinates for given point using triangle ABC.
		public static bool GetBarycentricCoordinates(Vector2 point, Vector2 a, Vector2 b, Vector2 c, out double t, out double u, out double v)
		{
			double den = ((b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y));
			if (den == 0)
			{
				t = u = v = 0.0;
				return false;
			}
			t = ((b.Y - c.Y) * (point.X - c.X) + (c.X - b.X) * (point.Y - c.Y)) / den;
			u = ((c.Y - a.Y) * (point.X - c.X) + (a.X - c.X) * (point.Y - c.Y)) / den;
			v = 1 - t - u;

			return true;
		}

		public static Vector3 GetTriangleNormal(Vector3 a, Vector3 b, Vector3 c)
		{
			return (b - a).Cross(c - a).GetNormalized();
		}
	}
}
