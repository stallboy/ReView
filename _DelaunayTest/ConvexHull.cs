using ReView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _DelaunayTest
{
	public class ConvexHull
	{
		/// <summary>
		/// Get convex hull for 2D point cloud.
		/// Returns null if less than 2 points provided.
		/// Returns two edges with surface area of 0 if exactly two points provided.
		/// </summary>
		/// <param name="points">Point cloud, list of Vector2 elements.</param>
		/// <returns>List of Edges defining the convex hull. Edges are not sorted but will have normals pointing away from the convex hull.</returns>
		public static List<Edge> GetHull(List<Vector2> points)
		{
			ReViewFeedManager debug = ReViewFeedManager.Instance;

			if (points.Count < 2)
			{
				// Not enough points to create a convex hull -> return null
				return null;
			}

			// Final list of convex hull edges
			List<Edge> edges = new List<Edge>();

			List<int> pointIndicesToProcess = new List<int>();

			// Find min and max vertex from X axis
			int minIndex = -1;
			int maxIndex = -1;
			double maxX = -Double.MaxValue;
			double minX = Double.MaxValue;
			for (int i = 0; i < points.Count; i++)
			{
				pointIndicesToProcess.Add(i);
				if (points[i].X > maxX)
				{
					maxX = points[i].X;
					maxIndex = i;
				}
				if (points[i].X < minX)
				{
					minX = points[i].X;
					minIndex = i;
				}
			}

			// Random points -> Introduces a bug
			/*
			minIndex = SRandom.Index(points.Count);
			maxIndex = SRandom.Index(points.Count);
			if (maxIndex == minIndex)
			{
				maxIndex = (maxIndex + 1) % points.Count;
			}
			*/

			// Add initial two edges to processing list
			List<Edge> edgesToProcess = new List<Edge>();
			edgesToProcess.Add(new Edge(minIndex, maxIndex));
			edgesToProcess.Add(new Edge(maxIndex, minIndex));
			pointIndicesToProcess.Remove(minIndex);
			pointIndicesToProcess.Remove(maxIndex);

#region Debug_Region_1
			//
			// REVIEW DEBUG RENDERING
			// Add boxes for vertices and for initial lines in the processing list
			//
			for (int i = 0; i < points.Count; i++)
			{
				debug.AddBox(0, -1, new Matrix4x4(), new Vector3(points[i].x, points[i].y, 0.0), new Vector3(0.005, 0.005, 0.005), new Color32(255, 255, 0, 255));
			}
			debug.AdvanceDebugTimer();
			debug.MapID(edgesToProcess[0], debug.AddLine(debug.DebugTimer, -1, points[minIndex], points[maxIndex], new Color32(200, 255, 0, 255)));
			debug.MapID(edgesToProcess[1], debug.AddLine(debug.DebugTimer, -1, points[maxIndex], points[minIndex], new Color32(200, 255, 0, 255)));
			debug.AdvanceDebugTimer();
#endregion

			// Iterate until no more edges to process
			while (edgesToProcess.Count > 0)
			{
				// Remove edge from processing list (it will either end up in the final edges list or get ignored)
				Edge edgeToProcess = edgesToProcess[0];
				edgesToProcess.RemoveAt(0);

#region Debug_Region_2
				//
				// REVIEW DEBUG RENDERING
				// Remove edge being processed and add it with different color for this frame
				//
				debug.RemovePrimitive(debug.FindID(edgeToProcess), debug.DebugTimer);
				debug.AddLine(debug.DebugTimer, debug.DebugTimerStep, points[edgeToProcess.A], points[edgeToProcess.B], new Color32(0, 0, 0, 255));
#endregion

				double furthestDistance = -1.0;
				int candidateIndex = -1;
				int pointsInFrontCount = 0;

				for (int i = 0; i < pointIndicesToProcess.Count; i++)
				{
					int pointIndex = pointIndicesToProcess[i];
					Vector2 point = points[pointIndex];

					if (!edgeToProcess.Contains(pointIndex) && edgeToProcess.PointInFront(points, point))
					{
#region Debug_Region_3
						//
						// REVIEW DEBUG RENDERING
						// Color vertex being checked to be "in front"
						//
						debug.AddBox(debug.DebugTimer, debug.DebugTimerStep, new Matrix4x4(), new Vector3(point.x, point.y, 0.0), new Vector3(0.0075, 0.0075, 0.0075), new Color32(0, 255, 0, 255));
#endregion

						// This point is not part of edge and it is on the "front" side of the edge -> Check distance and mark as candidate
						double distanceToEdge = edgeToProcess.DistanceToPoint(points, point);
						if (distanceToEdge > furthestDistance)
						{
							furthestDistance = distanceToEdge;
							candidateIndex = pointIndex;
						}

						pointsInFrontCount++;
					}
#region Debug_Region_4
					else
					{
						//
						// REVIEW DEBUG RENDERING
						// Color vertex being checked to not be "in front"
						//
						debug.AddBox(debug.DebugTimer, debug.DebugTimerStep, new Matrix4x4(), new Vector3(point.x, point.y, 0.0), new Vector3(0.0075, 0.0075, 0.0075), new Color32(255, 0, 0, 255));
					}
#endregion
				}
#region Debug_Region_5
				debug.AdvanceDebugTimer();
#endregion
				if (candidateIndex >= 0)
				{
#region Debug_Region_6
					//
					// REVIEW DEBUG RENDERING
					// Color vertex being selected as the furthest and "in front"
					//
					debug.AddBox(debug.DebugTimer, debug.DebugTimerStep, new Matrix4x4(), points[candidateIndex], new Vector3(0.0075, 0.0075, 0.0075), new Color32(0, 0, 0, 255));
#endregion
					if (!edgeToProcess.PointInLine(points, points[candidateIndex]))
					{
						// Remove all points inside the new triangle
						Triangle t = new Triangle(edgeToProcess.A, edgeToProcess.B, candidateIndex);
						for (int i = 0; i < pointIndicesToProcess.Count; i++)
						{
							int pointIndex = pointIndicesToProcess[i];
							if (t.Inside(points, points[pointIndex]))
							{
								pointIndicesToProcess.RemoveAt(i);
								i--;
								pointsInFrontCount--;
							}
						}
					}

					if (pointsInFrontCount == 0)
					{
						// New lines are part of the convex hull -> Don't add edgesToProcess
						edges.Add(new Edge(edgeToProcess.A, candidateIndex));
						edges.Add(new Edge(candidateIndex, edgeToProcess.B));
#region Debug_Region_7
						//
						// REVIEW DEBUG RENDERING
						// Show convex hull lines added
						//
						long line_id_1 = debug.AddLine(debug.DebugTimer, -1, points[edgeToProcess.A], points[candidateIndex], new Color32(255, 255, 255, 255));
						long line_id_2 = debug.AddLine(debug.DebugTimer, -1, points[candidateIndex], points[edgeToProcess.B], new Color32(255, 255, 255, 255));

						debug.AddAnnotation(line_id_1, debug.DebugTimer, -1, "Hull Edge #" + (edges.Count - 1), new Color32(255, 255, 255, 255));
						debug.AddAnnotation(line_id_2, debug.DebugTimer, -1, "Hull Edge #" + edges.Count, new Color32(255, 255, 255, 255));
#endregion
					}
					else
					{
						// Found point, add two new edges to process
						edgesToProcess.Add(new Edge(edgeToProcess.A, candidateIndex));
						edgesToProcess.Add(new Edge(candidateIndex, edgeToProcess.B));
#region Debug_Region_8
						//
						// REVIEW DEBUG RENDERING
						// Show new lines added to be processed
						//
						debug.MapID(edgesToProcess[edgesToProcess.Count - 2], debug.AddLine(debug.DebugTimer, -1, points[edgeToProcess.A], points[candidateIndex], new Color32(200, 255, 0, 255)));
						debug.MapID(edgesToProcess[edgesToProcess.Count - 1], debug.AddLine(debug.DebugTimer, -1, points[candidateIndex], points[edgeToProcess.B], new Color32(200, 255, 0, 255)));
#endregion
					}
				}
				else
				{
					// No candidates found -> This is part of convex hull
					edges.Add(new Edge(edgeToProcess.A, edgeToProcess.B));
#region Debug_Region_9
					long line_id = debug.AddLine(debug.DebugTimer, -1, points[edgeToProcess.A], points[edgeToProcess.B], new Color32(255, 255, 255, 255));
					debug.AddAnnotation(line_id, debug.DebugTimer, -1, "Hull Edge #" + edges.Count, new Color32(255, 255, 255, 255));
#endregion
				}
#region Debug_Region_10
				debug.AdvanceDebugTimer();
#endregion
			}
#region Debug_Region_11
			debug.RemoveAllAnnotations(debug.DebugTimer);
#endregion
			return edges;
		}
	}
}
