using ReView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _DelaunayTest
{
	public class Test
	{
		public Test()
		{
			ReViewFeedManager.Instance.Connect("localhost", 5000);

			Init(300, false);
		}

		public void Init(int numPointsToCreate, bool regularGrid)
		{
			Points = new List<Vector2>();

			if (regularGrid)
			{
				int dimension = (int)Math.Sqrt(numPointsToCreate);
				for (int y = 0; y < dimension; y++)
				{
					for (int x = 0; x < dimension; x++)
					{
						Points.Add(new Vector2(x / (double)dimension, y / (double)dimension));
					}
				}
			}
			else
			{
				// Create random points
				for (int i = 0; i < numPointsToCreate; i++)
				{
					Points.Add(Vector2.Random(0.8, 0.8) + new Vector2(-0.4, -0.4));
				}
			}
			
			// Create convex hull for points
			List<Edge> hullEdges = ConvexHull.GetHull(Points);

			// Create Delaunay triangulation from hull and points
			List<Triangle> triangles = Delaunay.Triangulate(Points, hullEdges);
		}

		private List<Vector2> Points
		{
			get;
			set;
		}
	}
}
