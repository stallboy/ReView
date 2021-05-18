using ReView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace ReViewDebugRenderView.Primitives
{
	/// <summary>
	/// Span of triangles by defining time and start - end index range
	/// </summary>
	public class DebugRenderTriangleIndexMarker : IComparable<DebugRenderTriangleIndexMarker>
	{
		public DebugRenderTriangleIndexMarker(int inIndex, int inTime)
		{
			time = inTime;
			index = inIndex;
		}

		public int CompareTo(DebugRenderTriangleIndexMarker other)
		{
			return time < other.time ? -1 : time == other.time ? 0 : 1;
		}

		public int index;
		public int time;
	}

	/// <summary>
	/// TriMesh
	/// Collection of triangles that belong to different smoothing groups.
	/// Each triangle that belongs to the same smoothing group will try to share vertices and normals are calculated as averages from all triangles that share the vertex.
	/// Having hard edges or "flat shading" you can have each triangle belonging to a separate smoothing group
	/// </summary>
	public class DebugRenderTriMesh : DebugRenderPrimitive
	{
		public DebugRenderTriMesh(Matrix4x4 transform, Vector3 pivot) : base(transform, pivot, new Vector3(1, 1, 1))
		{
			LocalBounds = new Bounds();

			MeshGeometry3D mesh = new MeshGeometry3D();

			opaqueTriangleMarkers = new List<DebugRenderTriangleIndexMarker>();
			transparentTriangleMarkers = new List<DebugRenderTriangleIndexMarker>();

			opaqueVertexList = new ArrayList<VertexNormalColor>(128 * 3);
			transparentVertexList = new ArrayList<VertexNormalColor>(128 * 3);
		}

		public void AddTriangle(int time, Vector3 a, Vector3 b, Vector3 c, Color32 color)
		{
			LocalBounds.Encapsulate(a);
			LocalBounds.Encapsulate(b);
			LocalBounds.Encapsulate(c);

			Vector3 newTriangleNormal = GeometryMath.GetTriangleNormal(a, b, c);

			ArrayList<VertexNormalColor> vertexList = color.A < 255 ? transparentVertexList : opaqueVertexList;
			List<DebugRenderTriangleIndexMarker> triangleMarkers = color.A < 255 ? transparentTriangleMarkers : opaqueTriangleMarkers;

			vertexList.Add(new VertexNormalColor() { Position = a, Color = color, Normal = newTriangleNormal });
			vertexList.Add(new VertexNormalColor() { Position = b, Color = color, Normal = newTriangleNormal });
			vertexList.Add(new VertexNormalColor() { Position = c, Color = color, Normal = newTriangleNormal });

			// Find or create new index marker and add triangle
			DebugRenderTriangleIndexMarker searchItem = new DebugRenderTriangleIndexMarker(0, time);
			int index = triangleMarkers.BinarySearch(searchItem);
			if (index < 0)
			{
				index = ~index;
			}

			while (index > 0 && index < triangleMarkers.Count && triangleMarkers[index].time > time)
			{
				// Not exact match but the next one -> Decrease index by one to have only the range that counts up for the requested time
				index--;
			}

			if (index < triangleMarkers.Count && triangleMarkers[index].time == time)
			{
				// Has existing one for this time -> Update end index
				triangleMarkers[index].index = vertexList.Count - 1;
			}
			else
			{
				// Insert new
				DebugRenderTriangleIndexMarker range = new DebugRenderTriangleIndexMarker(vertexList.Count - 1, time);
				triangleMarkers.Insert(index, range);
			}

			if (!InfiniteLength && EndTime < time)
			{
				EndTime = time;
			}
		}

		private int GetElementCount(int time, bool transparent)
		{
			List<DebugRenderTriangleIndexMarker> triangleMarkers = transparent ? transparentTriangleMarkers : opaqueTriangleMarkers;

			int indexElementCount = 0;
			DebugRenderTriangleIndexMarker searchItem = new DebugRenderTriangleIndexMarker(0, time);
			int index = triangleMarkers.BinarySearch(searchItem);
			if (index < 0)
			{
				index = ~index;
			}

			index = triangleMarkers.Count > 0 ? SMath.Clamp(index, 0, triangleMarkers.Count - 1) : index;

			if (index < triangleMarkers.Count)
			{
				while (index > 0 && triangleMarkers[index].time > time)
				{
					// Not exact match but the next one -> Decrease index by one to have only the range that counts up for the requested time
					index--;
				}

				if (index < triangleMarkers.Count && triangleMarkers[index].time <= time)
				{
					indexElementCount = triangleMarkers[index].index + 1;
				}
			}
			return indexElementCount;
		}

		public override void AddLines(RenderInfo info, ArrayList<VertexNormalColor> opaqueVertexData, ArrayList<VertexNormalColor> transparentVertexData)
		{
			if (info.showTriangleNormals)
			{
				Matrix4x4 mat = GetTransform();
				
				int opaqueElementCount = GetElementCount(info.time, false);
				for (int i = 0; i < opaqueElementCount; i += 3)
				{
					Vector3 a = mat * opaqueVertexList[i].Position;
					Vector3 b = mat * opaqueVertexList[i + 1].Position;
					Vector3 c = mat * opaqueVertexList[i + 2].Position;
					Vector3 normalPosition = (a + b + c) / 3;
					double triangleSize = (a.Distance(b) + a.Distance(c)) / 2;
					Color32 normalColor = info.normalColor != null ? info.normalColor : opaqueVertexList[i].Color;

					if (normalColor.A < 255)
					{
						transparentVertexData.Add(new VertexNormalColor() { Position = normalPosition, Color = normalColor });
						transparentVertexData.Add(new VertexNormalColor() { Position = normalPosition + opaqueVertexList[i].Normal * (info.normalLength * triangleSize), Color = normalColor });
					}
					else
					{
						opaqueVertexData.Add(new VertexNormalColor() { Position = normalPosition, Color = normalColor });
						opaqueVertexData.Add(new VertexNormalColor() { Position = normalPosition + opaqueVertexList[i].Normal * (info.normalLength * triangleSize), Color = normalColor });
					}
				}

				int transparentElementCount = GetElementCount(info.time, true);
				for (int i = 0; i < transparentElementCount; i += 3)
				{
					Vector3 a = mat * transparentVertexList[i].Position;
					Vector3 b = mat * transparentVertexList[i + 1].Position;
					Vector3 c = mat * transparentVertexList[i + 2].Position;
					Vector3 normalPosition = (a + b + c) / 3;
					double triangleSize = (a.Distance(b) + a.Distance(c)) / 2;
					Color32 normalColor = info.normalColor != null ? info.normalColor : transparentVertexList[i].Color;

					if (normalColor.A < 255)
					{
						transparentVertexData.Add(new VertexNormalColor() { Position = normalPosition, Color = normalColor });
						transparentVertexData.Add(new VertexNormalColor() { Position = normalPosition + transparentVertexList[i].Normal * (info.normalLength * triangleSize), Color = normalColor });
					}
					else
					{
						opaqueVertexData.Add(new VertexNormalColor() { Position = normalPosition, Color = normalColor });
						opaqueVertexData.Add(new VertexNormalColor() { Position = normalPosition + transparentVertexList[i].Normal * (info.normalLength * triangleSize), Color = normalColor });
					}
				}
			}
		}

		public override void AddTriangles(RenderInfo info, ArrayList<VertexNormalColor> opaqueVertexData, ArrayList<VertexNormalColor> transparentVertexData)
		{
			Matrix4x4 mat = GetTransform();

			int opaqueElementCount = GetElementCount(info.time, false);
			for (int i = 0; i < opaqueElementCount; i++)
			{
				opaqueVertexData.Add(mat * opaqueVertexList[i]);
			}

			int transparentElementCount = GetElementCount(info.time, true);
			for (int i = 0; i < transparentElementCount; i++)
			{
				transparentVertexData.Add(mat * transparentVertexList[i]);
			}
		}

		protected override bool RayCheckInternal(int time, Vector3 rayStart, Vector3 rayDirection, out int triangleIndex, out bool hitOpaque, out Vector3 intersection)
		{
			Matrix4x4 mat = GetTransform();

			hitOpaque = false;
			triangleIndex = -1;
			intersection = new Vector3(0, 0, 0);
			double minDistSq = Double.MaxValue;
			Vector3 triangleIntersection = new Vector3(0, 0, 0);

			int elementCount = GetElementCount(time, false);
			for (int i = 0; i < elementCount; i += 3)
			{
				Vector3 a = mat * opaqueVertexList[i].Position;
				Vector3 b = mat * opaqueVertexList[i + 1].Position;
				Vector3 c = mat * opaqueVertexList[i + 2].Position;
				if (GeometryMath.Intersect(rayStart, rayDirection, a, b, c, out triangleIntersection))
				{
					double distSq = rayStart.DistanceSquared(triangleIntersection);
					if (distSq < minDistSq)
					{
						minDistSq = distSq;
						intersection = triangleIntersection;
						triangleIndex = i;
						hitOpaque = true;
					}
				}
			}

			elementCount = GetElementCount(time, true);
			for (int i = 0; i < elementCount; i += 3)
			{
				Vector3 a = mat * transparentVertexList[i].Position;
				Vector3 b = mat * transparentVertexList[i + 1].Position;
				Vector3 c = mat * transparentVertexList[i + 2].Position;
				if (GeometryMath.Intersect(rayStart, rayDirection, a, b, c, out triangleIntersection))
				{
					double distSq = rayStart.DistanceSquared(triangleIntersection);
					if (distSq < minDistSq)
					{
						minDistSq = distSq;
						intersection = triangleIntersection;
						triangleIndex = i;
						hitOpaque = false;
					}
				}
			}

			return (triangleIndex != -1);
		}

		public override bool GetTriangle(int triangleIndex, bool isOpaque, out Vector3 a, out Vector3 b, out Vector3 c)
		{
			Matrix4x4 mat = GetTransform();

			a = b = c = new Vector3(0, 0, 0);
			if (isOpaque)
			{
				if (triangleIndex < 0 || triangleIndex + 2 >= opaqueVertexList.Count)
				{
					return false;
				}
				a = mat * opaqueVertexList[triangleIndex].Position;
				b = mat * opaqueVertexList[triangleIndex + 1].Position;
				c = mat * opaqueVertexList[triangleIndex + 2].Position;
			}
			else
			{
				if (triangleIndex < 0 || triangleIndex + 2 >= transparentVertexList.Count)
				{
					return false;
				}
				a = mat * transparentVertexList[triangleIndex].Position;
				b = mat * transparentVertexList[triangleIndex + 1].Position;
				c = mat * transparentVertexList[triangleIndex + 2].Position;
			}

			return true;
		}

		public override Color32 GetColor(int triangleIndex, bool isOpaque)
		{
			if (isOpaque)
			{
				if (triangleIndex >= 0 && triangleIndex + 2 < opaqueVertexList.Count)
				{
					return opaqueVertexList[triangleIndex].Color;
				}
			}
			else
			{
				if (triangleIndex >= 0 && triangleIndex + 2 < transparentVertexList.Count)
				{
					return transparentVertexList[triangleIndex].Color;
				}
			}

			return new Color32(0, 0, 0, 0);
		}

		public override PrimitiveSelectionInfo GetSelectionInfo(int triangleIndex, bool isSelectedTriangleOpaque)
		{
			return new TriMeshSelectionInfo(this, triangleIndex, isSelectedTriangleOpaque);
		}

		private ArrayList<VertexNormalColor> opaqueVertexList;
		private ArrayList<VertexNormalColor> transparentVertexList;

		private List<DebugRenderTriangleIndexMarker> opaqueTriangleMarkers;
		private List<DebugRenderTriangleIndexMarker> transparentTriangleMarkers;
	}

	public class TriMeshSelectionInfo : PrimitiveSelectionInfo
	{
		public TriMeshSelectionInfo(DebugRenderPrimitive selectedPrimitive, int triangleIndex, bool isSelectedTriangleOpaque)
			: base(selectedPrimitive, triangleIndex, isSelectedTriangleOpaque)
		{
			Vector3 a, b, c;
			if (Primitive.GetTriangle(TriangleIndex, IsSelectedTriangleOpaque, out a, out b, out c))
			{
				V0 = a;
				V1 = b;
				V2 = c;
			}
			else
			{
				V0 = V1 = V2 = new Vector3(0, 0, 0);
			}

			Color = Primitive.GetColor(TriangleIndex, IsSelectedTriangleOpaque);
		}

		[CategoryAttribute("Triangle Info"), ReadOnlyAttribute(true), DescriptionAttribute("V0")]
		public Vector3 V0
		{
			get;
			private set;
		}

		[CategoryAttribute("Triangle Info"), ReadOnlyAttribute(true), DescriptionAttribute("V1")]
		public Vector3 V1
		{
			get;
			private set;
		}

		[CategoryAttribute("Triangle Info"), ReadOnlyAttribute(true), DescriptionAttribute("V2")]
		public Vector3 V2
		{
			get;
			private set;
		}

		[CategoryAttribute("Triangle Info"), ReadOnlyAttribute(true), DescriptionAttribute("Surface area")]
		public string Area
		{
			get
			{
				return String.Format("{0:0.000}", GeometryMath.GetTriangleArea(V0, V1, V2));
			}
		}

		[CategoryAttribute("Triangle Info"), ReadOnlyAttribute(true), DescriptionAttribute("Perimeter")]
		public string Perimeter
		{
			get
			{
				return String.Format("{0:0.000}", GeometryMath.GetTrianglePerimeter(V0, V1, V2));
			}
		}

		[CategoryAttribute("Triangle Info"), ReadOnlyAttribute(true), DescriptionAttribute("Center")]
		public Vector3 Center
		{
			get
			{
				return (V0 + V1 + V2) / 3.0;
			}
		}

		[CategoryAttribute("Triangle Info"), ReadOnlyAttribute(true), DescriptionAttribute("Normal")]
		public Vector3 Normal
		{
			get
			{
				return GeometryMath.GetTriangleNormal(V0, V1, V2);
			}
		}

		[CategoryAttribute("Triangle Info"), ReadOnlyAttribute(true), DescriptionAttribute("Color")]
		public Color32 Color
		{
			get;
			set;
		}

		[CategoryAttribute("Triangle Info"), ReadOnlyAttribute(true), DescriptionAttribute("Degenerate")]
		public bool IsDegenerate
		{
			get
			{
				return GeometryMath.IsTriangleDegenerate(V0, V1, V2);
			}
		}
	}
}
