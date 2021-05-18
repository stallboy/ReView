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
	public class DebugRenderCylinder : DebugRenderPrimitive
	{
		private static Dictionary<int, MeshGeometry3D> cachedMeshes = new Dictionary<int, MeshGeometry3D>();
		private static Dictionary<int, MeshGeometry3D> cachedMeshesWithCaps = new Dictionary<int, MeshGeometry3D>();

		public DebugRenderCylinder(Matrix4x4 transform, Vector3 pivot, double topRadius, double bottomRadiusScale, double height, int segments, Color32 color, bool createCaps) : base(transform, pivot, new Vector3(topRadius, topRadius, height))
		{
			double scale = Math.Max(1.0, bottomRadiusScale);
			LocalBounds = new Bounds(new Vector3(-0.5, -0.5, -0.5) * scale, new Vector3(0.5, 0.5, 0.5) * scale);
			CreateCaps = createCaps;
			Segments = segments;
			BottomRadiusScale = bottomRadiusScale;
			PrimitiveColor = color;
		}

		public void Dispose()
		{
			// Don't delete VBOs unless all DebugRenderBox objects have been deleted
			if (CreateCaps)
			{
				if (RefCountMap.ContainsKey(Hash))
				{
					RefCountMap[Hash]--;
					if (RefCountMap[Hash] == 0)
					{
						RefCountMap.Remove(Hash);
						VertexMap = null;
						IndexMap = null;
					}
				}
			}
		}

		public int Segments
		{
			get;
			set;
		}

		public double BottomRadiusScale
		{
			get;
			set;
		}

		public bool CreateCaps
		{
			get;
			set;
		}

		private int Hash
		{
			get
			{
				return BottomRadiusScale.GetHashCode() + Segments.GetHashCode() + CreateCaps.GetHashCode();
			}
		}

		private static Dictionary<int, int> RefCountMap = new Dictionary<int, int>();
		private static Dictionary<int, VertexNormalColor[]> VertexMap = new Dictionary<int, VertexNormalColor[]>();
		private static Dictionary<int, uint[]> IndexMap = new Dictionary<int, uint[]>();

		private VertexNormalColor[] VertexList
		{
			get
			{
				return VertexMap.ContainsKey(Hash) ? VertexMap[Hash] : null;
			}
			set
			{
				if (VertexMap.ContainsKey(Hash))
				{
					VertexMap[Hash] = value;
				}
				else
				{
					VertexMap.Add(Hash, value);
				}
			}
		}

		private uint[] IndexList
		{
			get
			{
				return IndexMap.ContainsKey(Hash) ? IndexMap[Hash] : null;
			}
			set
			{
				if (IndexMap.ContainsKey(Hash))
				{
					IndexMap[Hash] = value;
				}
				else
				{
					IndexMap.Add(Hash, value);
				}
			}
		}

		private void CreateMesh()
		{
			double height = 0.5;
			double TopRadius = 0.5;
			double BottomRadius = TopRadius * BottomRadiusScale;

			Segments = SMath.Clamp(Segments, 3, 256);

			VertexList = new VertexNormalColor[Segments * (CreateCaps ? 4 : 2)];
			IndexList = new uint[Segments * 6 + (CreateCaps ? (Segments - 1) * 6 : 0)];

			// Create vertices
			for (int i = 0; i < Segments; i++)
			{
				double angle = ((double)i / (double)Segments) * Math.PI * 2.0;
				Vector2 planarCoordinate = Vector2.FromAngle(angle, 1.0);
				// Top
				VertexList[i + Segments * 0].Position = new Vector3(planarCoordinate.X * TopRadius, planarCoordinate.Y * TopRadius, height);
				// Bottom
				VertexList[i + Segments * 1].Position = new Vector3(planarCoordinate.X * BottomRadius, planarCoordinate.Y * BottomRadius, -height);

				if (CreateCaps)
				{
					// Top (for cap)
					VertexList[i + Segments * 2].Position = new Vector3(planarCoordinate.X * TopRadius, planarCoordinate.Y * TopRadius, height);
					// Bottom (for cap)
					VertexList[i + Segments * 3].Position = new Vector3(planarCoordinate.X * BottomRadius, planarCoordinate.Y * BottomRadius, -height);
				}
			}

			// Create normals
			Vector3 topNormal = CreateCaps ? GeometryMath.GetTriangleNormal(VertexList[Segments * 2].Position, VertexList[Segments * 2 + 2].Position, VertexList[Segments * 2 + 1].Position) : new Vector3(0, 0, 0);
			Vector3 bottomNormal = CreateCaps ? GeometryMath.GetTriangleNormal(VertexList[Segments * 3].Position, VertexList[Segments * 3 + 1].Position, VertexList[Segments * 3 + 2].Position) : new Vector3(0, 0, 0);

			for (int i = 0; i < Segments; i++)
			{
				Vector3 manifoldNormal = GeometryMath.GetTriangleNormal(VertexList[i].Position, VertexList[(i + 1) % Segments].Position, VertexList[i + Segments].Position);
				VertexList[Segments * 0 + i].Normal = manifoldNormal;
				VertexList[Segments * 1 + i].Normal = manifoldNormal;
				if (CreateCaps)
				{
					VertexList[Segments * 2 + i].Normal = topNormal;
					VertexList[Segments * 3 + i].Normal = bottomNormal;
				}
			}

			// Create triangles
			int index = 0;
			for (int i = 0; i < Segments; i++)
			{
				// Manifold		
				IndexList[index++] = (uint)i;
				IndexList[index++] = (uint)((i + 1) % Segments);
				IndexList[index++] = (uint)(i + Segments);

				IndexList[index++] = (uint)((i + 1) % Segments);
				IndexList[index++] = (uint)((i + 1) % Segments + Segments);
				IndexList[index++] = (uint)(i + Segments);

				if (CreateCaps && i < Segments - 1)
				{
					// Top cap
					IndexList[index++] = (uint)(Segments * 2);
					IndexList[index++] = (uint)(Segments * 2 + i + 1);
					IndexList[index++] = (uint)(Segments * 2 + i);

					// Bottom cap
					IndexList[index++] = (uint)(Segments * 3);
					IndexList[index++] = (uint)(Segments * 3 + i);
					IndexList[index++] = (uint)(Segments * 3 + i + 1);
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
			return new CylinderSelectionInfo(this, triangleIndex, isSelectedTriangleOpaque);
		}
	}

	public class CylinderSelectionInfo : PrimitiveSelectionInfo
	{
		public CylinderSelectionInfo(DebugRenderPrimitive selectedPrimitive, int triangleIndex, bool isSelectedTriangleOpaque)
			: base(selectedPrimitive, triangleIndex, isSelectedTriangleOpaque)
		{
			Color = Primitive.GetColor(TriangleIndex, IsSelectedTriangleOpaque);
		}

		[CategoryAttribute("Cylinder Info"), ReadOnlyAttribute(true), DescriptionAttribute("Position")]
		public Vector3 Position
		{
			get
			{
				return Primitive.GetTransform().Translation;
			}
		}

		[CategoryAttribute("Cylinder Info"), ReadOnlyAttribute(true), DescriptionAttribute("Segments")]
		public int Segments
		{
			get
			{
				return (Primitive as DebugRenderCylinder).Segments;
			}
		}

		[CategoryAttribute("Cylinder Info"), ReadOnlyAttribute(true), DescriptionAttribute("Has Caps")]
		public bool HasCaps
		{
			get
			{
				return (Primitive as DebugRenderCylinder).CreateCaps;
			}
		}

		[CategoryAttribute("Cylinder Info"), ReadOnlyAttribute(true), DescriptionAttribute("TopBottomRadiusRatio")]
		public string TopBottomRadiusRatio
		{
			get
			{
				return String.Format("{0:0.000}", (Primitive as DebugRenderCylinder).BottomRadiusScale);
			}
		}

		[CategoryAttribute("Cylinder Info"), ReadOnlyAttribute(true), DescriptionAttribute("HalfSize")]
		public Vector3 HalfSize
		{
			get
			{
				return Primitive.LocalBounds.HalfSize;
			}
		}

		[CategoryAttribute("Cylinder Info"), ReadOnlyAttribute(true), DescriptionAttribute("Color")]
		public Color32 Color
		{
			get;
			private set;
		}
	}
}
