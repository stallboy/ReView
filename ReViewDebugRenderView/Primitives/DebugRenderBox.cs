using ReView;
using SharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ReViewDebugRenderView.Primitives
{
	public class DebugRenderBox : DebugRenderPrimitive, IDisposable
	{
		// A-------B
		// |\      |\
		// | \     | \
		// |  E----|--F        Y+
		// C-------D  |    Z+  |   
		//  \ |     \ |     \  | 
		//   \|      \|      \ |
		//    G-------H       \------X+
		//
		// (A, B, D), (A, D, C), (E, H, F), (E, G, H), (A, E, B), (B, E, F), (C, D, G), (D, H, G), (D, B, F), (H, D, F), (C, G, E), (C, E, A)
		public DebugRenderBox(Matrix4x4 transform, Vector3 pivot, Vector3 halfSize, Color32 color) : base(transform, pivot, halfSize)
		{
			LocalBounds = new Bounds(new Vector3(-0.5, -0.5, -0.5), new Vector3(0.5, 0.5, 0.5));
			PrimitiveColor = color;
			RefCount++;
		}

		public void Dispose()
		{
			RefCount--;
			if (RefCount == 0)
			{
				vertexList = null;
			}
		}

		protected static void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color32 color)
		{
			Vector3 normal = (b - a).Cross(c - a).GetNormalized();

			vertexList.Add(new VertexNormalColor()
			{
				x = (float)a.x,
				y = (float)a.y,
				z = (float)a.z,
				nx = (float)normal.x,
				ny = (float)normal.y,
				nz = (float)normal.z,
				r = (float)color.R_Double,
				g = (float)color.G_Double,
				b = (float)color.B_Double,
				a = (float)color.A_Double
			});

			vertexList.Add(new VertexNormalColor()
			{
				x = (float)b.x,
				y = (float)b.y,
				z = (float)b.z,
				nx = (float)normal.x,
				ny = (float)normal.y,
				nz = (float)normal.z,
				r = (float)color.R_Double,
				g = (float)color.G_Double,
				b = (float)color.B_Double,
				a = (float)color.A_Double
			});

			vertexList.Add(new VertexNormalColor()
			{
				x = (float)c.x,
				y = (float)c.y,
				z = (float)c.z,
				nx = (float)normal.x,
				ny = (float)normal.y,
				nz = (float)normal.z,
				r = (float)color.R_Double,
				g = (float)color.G_Double,
				b = (float)color.B_Double,
				a = (float)color.A_Double
			});
		}

		private void CreateMesh()
		{
			Vector3D halfSize = new Vector3D(0.5, 0.5, 0.5);
			Vector3 a = new Vector3(-halfSize.X, halfSize.Y, halfSize.Z);
			Vector3 b = new Vector3(halfSize.X, halfSize.Y, halfSize.Z);
			Vector3 c = new Vector3(-halfSize.X, -halfSize.Y, halfSize.Z);
			Vector3 d = new Vector3(halfSize.X, -halfSize.Y, halfSize.Z);
			Vector3 e = new Vector3(-halfSize.X, halfSize.Y, -halfSize.Z);
			Vector3 f = new Vector3(halfSize.X, halfSize.Y, -halfSize.Z);
			Vector3 g = new Vector3(-halfSize.X, -halfSize.Y, -halfSize.Z);
			Vector3 h = new Vector3(halfSize.X, -halfSize.Y, -halfSize.Z);

			vertexList = new ArrayList<VertexNormalColor>(36);

			Color32 white = new Color32(255, 255, 255, 255);
			AddTriangle(a, d, b, white);
			AddTriangle(a, c, d, white); 
			AddTriangle(e, f, h, white);
			AddTriangle(e, h, g, white);
			AddTriangle(a, b, e, white);
			AddTriangle(b, f, e, white);
			AddTriangle(c, g, d, white);
			AddTriangle(d, g, h, white);
			AddTriangle(d, f, b, white);
			AddTriangle(h, f, d, white);
			AddTriangle(c, e, g, white);
			AddTriangle(c, a, e, white);
		}

		public override void AddTriangles(RenderInfo info, ArrayList<VertexNormalColor> opaqueVertexData, ArrayList<VertexNormalColor> transparentVertexData)
		{
			if (vertexList == null)
			{
				CreateMesh();
			}

			Matrix4x4 mat = GetTransform();

			if (PrimitiveColor.A < 255)
			{
				for (int i = 0; i < vertexList.Count; i++)
				{
					VertexNormalColor newVertex = mat * vertexList[i];
					newVertex.Color = PrimitiveColor;
					transparentVertexData.Add(newVertex);
				}
			}
			else
			{
				for (int i = 0; i < vertexList.Count; i++)
				{
					VertexNormalColor newVertex = mat * vertexList[i];
					newVertex.Color = PrimitiveColor;
					opaqueVertexData.Add(newVertex);
				}
			}
		}

		public override PrimitiveSelectionInfo GetSelectionInfo(int triangleIndex, bool isSelectedTriangleOpaque)
		{
			return new BoxSelectionInfo(this, triangleIndex, isSelectedTriangleOpaque);
		}

		private static ArrayList<VertexNormalColor> vertexList = null;

		private static int RefCount = 0;
	}

	public class BoxSelectionInfo : PrimitiveSelectionInfo
	{
		public BoxSelectionInfo(DebugRenderPrimitive selectedPrimitive, int triangleIndex, bool isSelectedTriangleOpaque)
			: base(selectedPrimitive, triangleIndex, isSelectedTriangleOpaque)
		{
			Color = Primitive.GetColor(TriangleIndex, IsSelectedTriangleOpaque);
		}

		[CategoryAttribute("Box Info"), ReadOnlyAttribute(true), DescriptionAttribute("Position")]
		public Vector3 Position
		{
			get
			{
				return Primitive.GetTransform().Translation;
			}
		}

		[CategoryAttribute("Box Info"), ReadOnlyAttribute(true), DescriptionAttribute("Local Center")]
		public Vector3 LocalCenter
		{
			get
			{
				return Primitive.LocalBounds.Center;
			}
		}

		[CategoryAttribute("Box Info"), ReadOnlyAttribute(true), DescriptionAttribute("HalfSize")]
		public Vector3 HalfSize
		{
			get
			{
				return Primitive.LocalBounds.HalfSize;
			}
		}

		[CategoryAttribute("Box Info"), ReadOnlyAttribute(true), DescriptionAttribute("Color")]
		public Color32 Color
		{
			get;
			private set;
		}
	}
}
