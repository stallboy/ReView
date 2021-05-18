using ReView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _DelaunayTest
{
	public class Delaunay
	{
		/// <summary>
		/// Delaunay triangulate point cloud with given convex hull.
		/// Returns null if less than 3 points provided.
		/// Using Tenamura-Merriam algorithm which does 'advancing front' starting with convexHull edges.
		/// UNOPTIMAL IMPLEMENTATION! This will not deal with regular grids.
		/// </summary>
		/// <param name="points">Point cloud, list of Vector2 elements.</param>
		/// <param name="points">Edge list, convex hull for point cloud.</param>
		/// <returns>List of Edges defining the convex hull. Edges are not sorted but will have normals pointing away from the convex hull.</returns>
		public static List<Triangle> Triangulate(List<Vector2> points, List<Edge> convexHull)
		{
			if (points.Count < 3)
			{
				// Not enough points to create triangulation -> return null
				return null;
			}

#region Debug_Region_1
			Vector3 LINE_ZOFFS = new Vector3(0, 0, 0.002);
			int triCount = 0;

			ReViewFeedManager debug = ReViewFeedManager.Instance;
			long mesh_id = debug.AddMesh(debug.DebugTimer, -1, new Matrix4x4(), new Vector3(0, 0, 0.001), true);
#endregion

			// Final list of convex hull edges
			List<Triangle> triangles = new List<Triangle>();

			List<Edge> edgesToProcess = new List<Edge>();
			foreach (Edge edge in convexHull)
			{
				edgesToProcess.Add(edge.GetFlipped());
			}

			while (edgesToProcess.Count > 0)
			{
				Edge edgeToProcess = edgesToProcess[0];
				edgesToProcess.RemoveAt(0);

				int circumCircleFailCount = 0;
				bool triangleAdded = false;

				// Try to find third vertex for the edgeToProcess to form a triangle

				List<int> testedCandidates = new List<int>();
				for (int i = 0; i < points.Count; i++)
				{
					if (edgeToProcess.Contains(i))
					{
						// This point is part of the edge already -> Skip
						continue;
					}

					if (!edgeToProcess.PointInFront(points, points[i]))
					{
						// This point is not in front -> Skip
						continue;
					}

					if (testedCandidates.Contains(i))
					{
						// Already processed -> Skip
						continue;
					}
					testedCandidates.Add(i);

					Triangle t = new Triangle(edgeToProcess.A, edgeToProcess.B, i);

					if (GeometryMath.IsTriangleDegenerate(points[t.A], points[t.B], points[t.C]))
					{
						// Degenerate triangle (no surface area so points are all in a line) -> Ignore this and continue
						continue;
					}

					// Check circumcircle validity for this triangle
					bool pointsInCircumCircle = false;
					
					for (int j = 0; j < points.Count; j++)
					{
						if (i == j || edgeToProcess.Contains(j))
						{
							continue;
						}

						Vector2 otherPoint = points[j];
						// Some other point -> Check if in circumcircle
						if (t.PointInCircumcircle(points, otherPoint))
						{
#region Debug_Region_2
							// Add debug drawing for failed circumcircle tests
							Vector2 center;
							double radius;
							if (GeometryMath.GetCircumcircle(points[edgeToProcess.A], points[edgeToProcess.B], points[i], out center, out radius))
							{
								Color32 debugColor = Color32.FromHSVA((double)circumCircleFailCount / 16.0 % 1.0, 0.9, 1.0, 1.0);
								// Circumcircle
								debug.AddCircle(debug.DebugTimer, debug.DebugTimerStep, (Vector3)center + LINE_ZOFFS, radius, new Vector3(0, 0, 1), 64, debugColor);
								// Point that was inside (fails the test)
								debug.AddBox(debug.DebugTimer, debug.DebugTimerStep, new Matrix4x4(new Quaternion(new Vector3(0, 0, 1), Math.PI * 0.25)), new Vector3(points[j].x, points[j].y, 0.0), new Vector3(0.015, 0.015, 0.015), debugColor);
								// Lines to show triangle tested
								debug.AddLine(debug.DebugTimer, debug.DebugTimerStep, (Vector3)points[edgeToProcess.A] + LINE_ZOFFS, (Vector3)points[edgeToProcess.B] + LINE_ZOFFS, new Color32(0, 0, 0, 255));
								debug.AddLine(debug.DebugTimer, debug.DebugTimerStep, (Vector3)points[edgeToProcess.B] + LINE_ZOFFS, (Vector3)points[i] + LINE_ZOFFS, debugColor);
								debug.AddLine(debug.DebugTimer, debug.DebugTimerStep, (Vector3)points[i] + LINE_ZOFFS, (Vector3)points[edgeToProcess.A] + LINE_ZOFFS, debugColor);

								debug.AdvanceDebugTimer();
							}
#endregion

							circumCircleFailCount++;

							if (!testedCandidates.Contains(j))
							{
								i = j - 1;
							}
							pointsInCircumCircle = true;
									
							break;
						}
					}

					if (pointsInCircumCircle)
					{
						// Points in circumcircle -> Skip
						continue;
					}

					// Update edgesToProcess list by adding two new edges if they are not one of the convex hull edges
					Edge newEdgeA = new Edge(edgeToProcess.A, i);
					Edge newEdgeB = new Edge(i, edgeToProcess.B);

					// Check if new edges would be the same as unprocessed convex-hull edges
					for (int j = 0; j < edgesToProcess.Count; j++)
					{
						Edge convexEdge = edgesToProcess[j];
						if (convexEdge == newEdgeA)
						{
							// Don't add newEdgeA and remove convexEdge
							newEdgeA = null;
							edgesToProcess.RemoveAt(j);
							j--;
						}
						else if (convexEdge == newEdgeB)
						{
							// Don't add newEdgeA and remove convexEdge
							newEdgeB = null;
							edgesToProcess.RemoveAt(j);
							j--;
						}
					}

					// Check if new edges would be shared with existing triangle edges
					for (int j = 0; j < triangles.Count; j++)
					{
						if (triangles[j].Contains(newEdgeA))
						{
							newEdgeA = null;
						}
						if (triangles[j].Contains(newEdgeB))
						{
							newEdgeB = null;
						}
						if (newEdgeA == null && newEdgeB == null)
						{
							break;
						}
					}

					if (newEdgeA != null)
					{
						edgesToProcess.Add(newEdgeA);
					}
					if (newEdgeB != null)
					{
						edgesToProcess.Add(newEdgeB);
					}

					// Valid triangle -> Add to delaunay set
					triangles.Add(t);
#region Debug_Region_3
					// Debug graphics for triangle to be added
					Vector2 triCenter;
					double triRadius;
					// Add circumcircle
					GeometryMath.GetCircumcircle(points[t.A], points[t.B], points[t.C], out triCenter, out triRadius);
					debug.AddCircle(debug.DebugTimer, debug.DebugTimerStep, (Vector3)triCenter + LINE_ZOFFS, triRadius, new Vector3(0, 0, 1), 64, new Color32(255, 255, 0, 255));

					// Add triangle lines
					debug.AddLine(debug.DebugTimer, -1, (Vector3)points[t.A] + LINE_ZOFFS, (Vector3)points[t.B] + LINE_ZOFFS, new Color32(0, 0, 0, 255));
					debug.AddLine(debug.DebugTimer, -1, (Vector3)points[t.B] + LINE_ZOFFS, (Vector3)points[t.C] + LINE_ZOFFS, new Color32(0, 0, 0, 255));
					debug.AddLine(debug.DebugTimer, -1, (Vector3)points[t.C] + LINE_ZOFFS, (Vector3)points[t.A] + LINE_ZOFFS, new Color32(0, 0, 0, 255));

					// Add triangle
					debug.AddTriangle(mesh_id, debug.DebugTimer, points[t.A], points[t.B], points[t.C], new Color32(32, 255, 32, 128));

					// Progress for console window
					triCount++;
					if (triCount % 100 == 0)
					{
						Console.WriteLine("TriCount {0}", triCount);
					}

					debug.AdvanceDebugTimer();
#endregion

					triangleAdded = true;

					// Processed 'edgeToProcess' -> Proceed to next in the list
					break;
				}
#region Debug_Region_4
				// Debug graphics for the case when no triangle could be added for given edge
				if (!triangleAdded)
				{
					long line_id = debug.AddLine(debug.DebugTimer, debug.DebugTimerStep, (Vector3)points[edgeToProcess.A] + LINE_ZOFFS * 2, (Vector3)points[edgeToProcess.B] + LINE_ZOFFS * 2, new Color32(255, 0, 0, 255));
					debug.AddAnnotation(line_id, debug.DebugTimer, debug.DebugTimerStep, "No valid triangle could be generated!", new Color32(255, 255, 255, 255));
					debug.AdvanceDebugTimer();
				}
#endregion
			}

			Console.WriteLine("Done triangles");

			return triangles;
		}
	}
}
