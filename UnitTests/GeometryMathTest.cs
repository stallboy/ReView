using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReView;

namespace UnitTests
{
	[TestClass]
	public class GeometryMathTest
	{
		[TestMethod]
		public void TestRayTriangleIntersect()
		{
			Vector3 lineStart = new Vector3(0, 0, 0);
			Vector3 lineEnd = new Vector3(0, 0, 2);

			Vector3 a = new Vector3(0, -0.5, 1);
			Vector3 b = new Vector3(-0.5, 0.5, 1);
			Vector3 c = new Vector3(0.5, 0.5, 1);

			// Check that line hits triangle (should pass through center)
			Assert.IsTrue(GeometryMath.Intersect(lineStart, lineEnd, a, b, c), "Expected to hit triangle but did not (line should go through the center)");

			lineEnd = new Vector3(0, 0, 1);
			// Check that line hits triangle (should just hit the triangle at the end)
			Assert.IsTrue(GeometryMath.Intersect(lineStart, lineEnd, a, b, c), "Expected to hit triangle but did not (line should hit in the very end)");

			lineStart = new Vector3(0, 0, 1);
			lineEnd = new Vector3(0, 0, 2);
			// Check that line hits triangle (should just hit the triangle in the beginning)
			Assert.IsTrue(GeometryMath.Intersect(lineStart, lineEnd, a, b, c), "Expected to hit triangle but did not (line should hit in the very beginning)");

			lineStart = new Vector3(-0.4, 0, 0);
			lineEnd = new Vector3(-0.4, 0, 2);
			// Check that line does not hit triangle (should intersect plane but not triangle)
			Assert.IsFalse(GeometryMath.Intersect(lineStart, lineEnd, a, b, c), "Expected to miss triangle but hit anyway (line intersects plane but not triangle)");

			lineStart = new Vector3(0, 0, 1);
			lineEnd = new Vector3(1, 0, 1);
			// Check that line does not hit triangle (line is co-linear with triangle)
			Assert.IsFalse(GeometryMath.Intersect(lineStart, lineEnd, a, b, c), "Expected to miss triangle but hit anyway (line is co-linear with triangle)");

			lineStart = new Vector3(0, 0, 0);
			lineEnd = new Vector3(0, 0, 0.9);
			// Check that line does not hit triangle (not long enough)
			Assert.IsFalse(GeometryMath.Intersect(lineStart, lineEnd, a, b, c), "Expected to miss triangle but hit anyway (line not be long enough)");

			lineStart = new Vector3(0, 0, 1.1);
			lineEnd = new Vector3(0, 0, 2);
			// Check that line does not hit triangle (goes wrong way)
			Assert.IsFalse(GeometryMath.Intersect(lineStart, lineEnd, a, b, c), "Expected to miss triangle, but hit anyway (line goes away from triangle)");
		}

		[TestMethod]
		public void TestRayAABBIntersect()
		{
			Vector3 rayStart = new Vector3(0, 0, 0);
			Vector3 rayDir = new Vector3(0, 0, 1);

			double s, t;

			Bounds bounds = new Bounds(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));

			// Check that ray hits AABB (ray starts inside the bounds)
			Assert.IsTrue(GeometryMath.IntersectAABB(rayStart, rayDir, bounds.Min, bounds.Max, out s, out t), "Expected to hit AABB but did not (ray starts inside AABB)");

			rayStart = new Vector3(0, 0, -2);
			// Check that ray hits AABB (ray starts outside the bounds)
			Assert.IsTrue(GeometryMath.IntersectAABB(rayStart, rayDir, bounds.Min, bounds.Max, out s, out t), "Expected to hit AABB but did not (ray starts outside AABB)");

			rayDir = new Vector3(0, 0, -1);
			// Check that ray does not hit AABB (ray goes to other direction)
			Assert.IsFalse(GeometryMath.IntersectAABB(rayStart, rayDir, bounds.Min, bounds.Max, out s, out t), "Expected to miss AABB but did not (ray goes away from AABB)");

			rayStart = new Vector3(1, 0, -2);
			rayDir = new Vector3(1, 0, 1);
			// Check that ray does not hit AABB (ray is co-linear with one side of the AABB)
			Assert.IsFalse(GeometryMath.IntersectAABB(rayStart, rayDir, bounds.Min, bounds.Max, out s, out t), "Expected to miss AABB but did not (ray is co-linear with one side of the AABB)");
		}

		[TestMethod]
		public void TestPointInCircumcircle()
		{
			Vector2 a = new Vector2(0, -0.5);
			Vector2 b = new Vector2(-0.5, 0.5);
			Vector2 c = new Vector2(0.5, 0.5);

			Vector2 point = new Vector2(0, 0);

			// Check that point is in circumcircle
			Assert.IsTrue(GeometryMath.PointInCircumcircle(point, a, b, c), "Expected to be inside circumcircle but was not (point in middle of triangle)");

			point = new Vector2(0, 1);
			// Check that point is not in circumcircle
			Assert.IsFalse(GeometryMath.PointInCircumcircle(point, a, b, c), "Expected to be outside circumcircle (point outside one of the corners)");

			// Check that point is in circumcircle when triangle points are on the same line
			a = new Vector2(0, 0);
			b = new Vector2(1, 0);
			c = new Vector2(2, 0);
			Assert.IsTrue(GeometryMath.PointInCircumcircle(point, a, b, c), "Expected to be inside circumcircle (all triangle points on the same line)");

			// Check that point is in circumcircle when on triangle side length is 0
			a = new Vector2(0, 0);
			b = new Vector2(0, 0);
			c = new Vector2(0, 1);
			Assert.IsTrue(GeometryMath.PointInCircumcircle(point, a, b, c), "Expected to be inside circumcircle (one triangle side length is 0)");

			// Check that point is in circumcircle when is it just within radius
			a = new Vector2(0, -0.5);
			b = new Vector2(-0.5, 0.5);
			c = new Vector2(0.5, 0.5);
			point = new Vector2(0, -0.5);
			Assert.IsTrue(GeometryMath.PointInCircumcircle(point, a, b, c), "Expected to be inside circumcircle (point is the same as one of the triangle points)");
		}

		[TestMethod]
		public void TestBarycentricCoordinates()
		{
			Vector2 a = new Vector2(0, -0.5);
			Vector2 b = new Vector2(-0.5, 0.5);
			Vector2 c = new Vector2(0.5, 0.5);

			Vector2 point = new Vector2(0, 0);

			double t, u, v;

			// Check that barycentric coordinates are all between 0..1
			GeometryMath.GetBarycentricCoordinates(point, a, b, c, out t, out u, out v);
			Assert.IsTrue(t >= 0.0 && t <= 1.0 && u >= 0.0 && u <= 1.0 && v >= 0.0 && v <= 1.0, "Expected t, u, v to be within 0..1 (point in middle of triangle)");

			point = new Vector2(0, -0.5);
			// Check that barycentric coordinates are 0, 0, 1 as point is exactly where with one of the triangle points
			GeometryMath.GetBarycentricCoordinates(point, a, b, c, out t, out u, out v);
			Assert.IsTrue(t == 1.0 && u == 0.0 && v == 0.0, "Expected t = 0, u = 0, v = 1 (point in the same place as one of the triangle points)");

			point = new Vector2(-0.25, 0);
			// Check that one of the barycentric coordinates is 0 as point is on one of the triangle edges
			GeometryMath.GetBarycentricCoordinates(point, a, b, c, out t, out u, out v);
			Assert.IsTrue(v == 0, "Expected one of the (t, u, v) be 0 (point is in one of the triangle edges)");

			point = new Vector2(0, -0.51);
			// Check that barycentric coordinates are not between 0..1
			GeometryMath.GetBarycentricCoordinates(point, a, b, c, out t, out u, out v);
			Assert.IsFalse(t >= 0.0 && t <= 1.0 && u >= 0.0 && u <= 1.0 && v >= 0.0 && v <= 1.0, "Expected one of the (t, u, v) be outside 0..1 (point is outside of the triangle)");
		}

		[TestMethod]
		public void TestLineLineDistance()
		{
			Vector3 startA = new Vector3(0, 0, 0);
			Vector3 endA = new Vector3(1, 0, 0);

			Vector3 startB = new Vector3(0, 2, 0);
			Vector3 endB = new Vector3(1, 1, 0);

			double t;
			double s;
			double distance = GeometryMath.LineLineDistance(startA, endA, startB, endB, true, true, out t, out s);

			// Check that ray hits AABB (ray starts inside the bounds)
			Assert.IsTrue(distance == 1, String.Format("Expected distance to be 1 but got {0}, t {1} s {2} (lines cross but testing against segments)", distance, t, s));
		}
	}
}
