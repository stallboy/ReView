using ReView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReViewDebugRenderView.Primitives
{
	public class DebugRenderCone : DebugRenderPrimitive, IDisposable
	{
		private static Dictionary<int, MeshGeometry3D> cachedMeshes = new Dictionary<int, MeshGeometry3D>();
		private static Dictionary<int, MeshGeometry3D> cachedMeshesWithCaps = new Dictionary<int, MeshGeometry3D>();

		public DebugRenderCone(Matrix4x4 transform, Vector3 pivot, double radius, double height, int segments, Color32 color, bool createCaps) : base(transform, pivot, new Vector3(radius, radius, height))
		{
			LocalBounds = new Bounds(new Vector3(-0.5, -0.5, -0.5), new Vector3(0.5, 0.5, 0.5));
			CreateCaps = createCaps;
			Segments = segments;
			PrimitiveColor = color;
		}

		public int Segments
		{
			get;
			set;
		}

		public bool CreateCaps
		{
			get;
			set;
		}

		public void Dispose()
		{
			if (RefCountMap.ContainsKey(Segments))
			{
				RefCountMap[Segments]--;
				if (RefCountMap[Segments] == 0)
				{
					RefCountMap.Remove(Segments);
					VertexMap = null;
					IndexMap = null;
				}
			}
		}

		private static Dictionary<int, int> RefCountMap = new Dictionary<int, int>();
		private static Dictionary<int, VertexNormalColor[]> VertexMap = new Dictionary<int, VertexNormalColor[]>();
		private static Dictionary<int, uint[]> IndexMap = new Dictionary<int, uint[]>();

		private int Hash
		{
			get
			{
				return Segments.GetHashCode();
			}
		}

		private VertexNormalColor[] VertexList
		{
			get
			{
				return VertexMap.ContainsKey(Segments) ? VertexMap[Segments] : null;
			}
			set
			{
				if (VertexMap.ContainsKey(Segments))
				{
					VertexMap[Segments] = value;
				}
				else
				{
					VertexMap.Add(Segments, value);
				}
			}
		}

		private uint[] IndexList
		{
			get
			{
				return IndexMap.ContainsKey(Segments) ? IndexMap[Segments] : null;
			}
			set
			{
				if (IndexMap.ContainsKey(Segments))
				{
					IndexMap[Segments] = value;
				}
				else
				{
					IndexMap.Add(Segments, value);
				}
			}
		}

		private void CreateMesh()
		{
			double height = 0.5;
			double radius = 0.5;

			Segments = SMath.Clamp(Segments, 3, 256);

			VertexList = new VertexNormalColor[Segments * (CreateCaps ? 2 : 1) + 1];
			IndexList = new uint[Segments * 3 + (CreateCaps ? (Segments - 2) * 3 : 0)];

			// Create vertices
			VertexList[0].Position = new Vector3(0, 0, height);
			for (int i = 0; i < Segments; i++)
			{
				double angle = ((double)i / (double)Segments) * Math.PI * 2.0;
				Vector2 planarCoordinate = Vector2.FromAngle(angle, radius);

				VertexList[i + 1].Position = new Vector3(planarCoordinate.X, planarCoordinate.Y, -height);

				if (CreateCaps)
				{
					// Top (for cap)
					VertexList[i + Segments + 1].Position = new Vector3(planarCoordinate.X, planarCoordinate.Y, -height);
				}
			}

			// Create normals
			Vector3 capNormal = CreateCaps ? GeometryMath.GetTriangleNormal(VertexList[1].Position, VertexList[2].Position, VertexList[3].Position) : new Vector3(0, 0, 0);

			for (int i = 0; i < Segments; i++)
			{
				Vector3 manifoldNormal = GeometryMath.GetTriangleNormal(VertexList[0].Position, VertexList[(i + 1) % Segments + 1].Position, VertexList[i + 1].Position);
				VertexList[i + 1].Normal = manifoldNormal;
				if (CreateCaps)
				{
					VertexList[Segments + i + 1].Normal = capNormal;
				}
			}

			// Create triangles
			int index = 0;
			for (int i = 0; i < Segments; i++)
			{
				// Manifold					
				IndexList[index++] = 0;
				IndexList[index++] = (uint)((i + 1) % Segments + 1);
				IndexList[index++] = (uint)(i + 1);

				if (CreateCaps && i < Segments - 2)
				{
					// Cap
					IndexList[index++] = (uint)(Segments + 1);
					IndexList[index++] = (uint)(Segments + i + 2);
					IndexList[index++] = (uint)(Segments + i + 3);
				}
			}
		}

		public override void AddTriangles(RenderInfo info, ArrayList<VertexNormalColor> opaqueVertexData, ArrayList<VertexNormalColor> transparentVertexData, ArrayList<uint> opaqueIndexData, ArrayList<uint> transparentIndexData)
		{
			if (VertexList == null || IndexList == null)
			{
				CreateMesh();
			}

			Matrix4x4 mat = GetTransform();

			if (PrimitiveColor.A < 255)
			{
				int baseIndex = transparentVertexData.Count;

				for (int i = 0; i < VertexList.Length; i++)
				{
					VertexNormalColor newVertex = mat * VertexList[i];
					newVertex.Color = PrimitiveColor;
					transparentVertexData.Add(newVertex);
				}

				for (int i = 0; i < IndexList.Length; i++)
				{
					transparentIndexData.Add((uint)(IndexList[i] + baseIndex));
				}
			}
			else
			{
				int baseIndex = opaqueVertexData.Count;

				for (int i = 0; i < VertexList.Length; i++)
				{
					VertexNormalColor newVertex = mat * VertexList[i];
					newVertex.Color = PrimitiveColor;
					opaqueVertexData.Add(newVertex);
				}

				for (int i = 0; i < IndexList.Length; i++)
				{
					opaqueIndexData.Add((uint)(IndexList[i] + baseIndex));
				}
			}
		}

		public override PrimitiveSelectionInfo GetSelectionInfo(int triangleIndex, bool isSelectedTriangleOpaque)
		{
			return new ConeSelectionInfo(this, triangleIndex, isSelectedTriangleOpaque);
		}
	}

	public class ConeSelectionInfo : PrimitiveSelectionInfo
	{
		public ConeSelectionInfo(DebugRenderPrimitive selectedPrimitive, int triangleIndex, bool isSelectedTriangleOpaque)
			: base(selectedPrimitive, triangleIndex, isSelectedTriangleOpaque)
		{
			Color = Primitive.GetColor(TriangleIndex, IsSelectedTriangleOpaque);
		}

		[CategoryAttribute("Cone Info"), ReadOnlyAttribute(true), DescriptionAttribute("Position")]
		public Vector3 Position
		{
			get
			{
				return Primitive.GetTransform().Translation;
			}
		}

		[CategoryAttribute("Cone Info"), ReadOnlyAttribute(true), DescriptionAttribute("Segments")]
		public int Segments
		{
			get
			{
				return (Primitive as DebugRenderCone).Segments;
			}
		}

		[CategoryAttribute("Cone Info"), ReadOnlyAttribute(true), DescriptionAttribute("Has Caps")]
		public bool HasCaps
		{
			get
			{
				return (Primitive as DebugRenderCone).CreateCaps;
			}
		}

		[CategoryAttribute("Cone Info"), ReadOnlyAttribute(true), DescriptionAttribute("HalfSize")]
		public Vector3 HalfSize
		{
			get
			{
				return Primitive.LocalBounds.HalfSize;
			}
		}

		[CategoryAttribute("Cone Info"), ReadOnlyAttribute(true), DescriptionAttribute("Color")]
		public Color32 Color
		{
			get;
			private set;
		}
	}
}
