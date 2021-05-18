using ReView;
using SharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ReViewDebugRenderView.Primitives
{
	public class DebugRenderLine : DebugRenderPrimitive
	{
		public DebugRenderLine(Vector3 inStart, Vector3 inEnd, Color32 inColor)
		{
			LocalBounds = new Bounds();
			LocalBounds.Encapsulate(inStart);
			LocalBounds.Encapsulate(inEnd);

			start = inStart;
			end = inEnd;
			PrimitiveColor = inColor;
		}

		public override void AddLines(RenderInfo info, ArrayList<VertexNormalColor> opaqueVertexData, ArrayList<VertexNormalColor> transparentVertexData)
		{
			if (PrimitiveColor.A < 255)
			{
				transparentVertexData.Add(new VertexNormalColor() { Position = start, Color = PrimitiveColor });
				transparentVertexData.Add(new VertexNormalColor() { Position = end, Color = PrimitiveColor });
			}
			else
			{
				opaqueVertexData.Add(new VertexNormalColor() { Position = start, Color = PrimitiveColor });
				opaqueVertexData.Add(new VertexNormalColor() { Position = end, Color = PrimitiveColor });
			}
			
			if (info.showLineNormals)
			{
				double lineLength = start.Distance(end);
				Vector3 center = (start + end) * 0.5;
				Vector3 normal = (end - start).GetNormalized().Cross(new Vector3(0, 0, 1)) * (info.normalLength * lineLength);
				Color32 normalColor = info.normalColor != null ? info.normalColor : PrimitiveColor;
				if (normalColor.A < 255)
				{
					transparentVertexData.Add(new VertexNormalColor() { Position = center, Color = normalColor });
					transparentVertexData.Add(new VertexNormalColor() { Position = center + normal, Color = normalColor });
				}
				else
				{
					opaqueVertexData.Add(new VertexNormalColor() { Position = center, Color = normalColor });
					opaqueVertexData.Add(new VertexNormalColor() { Position = center + normal, Color = normalColor });
				}
			}
		}

		private static double LINE_SELECTION_THRESHOLD = 0.01;

		protected override bool RayCheckInternal(int time, Vector3 rayStart, Vector3 rayDirection, out int triangleIndex, out bool hitOpaque, out Vector3 intersection)
		{
			hitOpaque = false;
			triangleIndex = -1;
			intersection = new Vector3(0, 0, 0);

			double t, s;
			double distance = GeometryMath.LineLineDistance(rayStart, rayStart + rayDirection, start, end, false, true, out t, out s);
			if (distance <= LINE_SELECTION_THRESHOLD)
			{
				intersection = rayStart + rayDirection * t;
				return true;
			}

			return false;
		}

		public override PrimitiveSelectionInfo GetSelectionInfo(int triangleIndex, bool isSelectedTriangleOpaque)
		{
			return new LineSelectionInfo(this, triangleIndex, isSelectedTriangleOpaque);
		}

		public Vector3 start;
		public Vector3 end;
	}

	public class LineSelectionInfo : PrimitiveSelectionInfo
	{
		public LineSelectionInfo(DebugRenderPrimitive selectedPrimitive, int triangleIndex, bool isSelectedTriangleOpaque)
			: base(selectedPrimitive, triangleIndex, isSelectedTriangleOpaque)
		{
			Color = Primitive.GetColor(TriangleIndex, IsSelectedTriangleOpaque);
			A = (selectedPrimitive as DebugRenderLine).start;
			B = (selectedPrimitive as DebugRenderLine).end;
		}

		[CategoryAttribute("Line Info"), ReadOnlyAttribute(true), DescriptionAttribute("Length")]
		public string Length
		{
			get
			{
				return String.Format("{0:0.000}", A.Distance(B));
			}
		}

		[CategoryAttribute("Line Info"), ReadOnlyAttribute(true), DescriptionAttribute("Start")]
		public Vector3 A
		{
			get;
			private set;
		}

		[CategoryAttribute("Line Info"), ReadOnlyAttribute(true), DescriptionAttribute("End")]
		public Vector3 B
		{
			get;
			private set;
		}

		[CategoryAttribute("Line Info"), ReadOnlyAttribute(true), DescriptionAttribute("Center")]
		public Vector3 Center
		{
			get
			{
				return (A + B) / 2.0;
			}
		}

		[CategoryAttribute("Line Info"), ReadOnlyAttribute(true), DescriptionAttribute("Normal (2D)")]
		public Vector3 Normal2D
		{
			get
			{
				return (B - A).Cross(new Vector3(0, 0, 1));
			}
		}

		[CategoryAttribute("Line Info"), ReadOnlyAttribute(true), DescriptionAttribute("Color")]
		public Color32 Color
		{
			get;
			private set;
		}

		[CategoryAttribute("Line Info"), ReadOnlyAttribute(true), DescriptionAttribute("Degenerate")]
		public bool IsDegenerate
		{
			get
			{
				return A.Distance(B) == 0.0;
			}
		}
	}
}
