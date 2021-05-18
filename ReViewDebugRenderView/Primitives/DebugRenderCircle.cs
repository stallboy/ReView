using ReView;
using SharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ReViewDebugRenderView.Primitives
{
	public class DebugRenderCircle : DebugRenderPrimitive
	{
		public DebugRenderCircle(Vector3 inCenter, double inRadius, Vector3 inUp, int inSegments, Color32 inColor)
		{
			LocalBounds = new Bounds();

			Vector3 sideVector = Math.Abs(inUp.Dot(new Vector3(0, 0, 1))) < 1.0 ? new Vector3(0, 0, 1) : new Vector3(1, 0, 0);
			handDir = inUp.Cross(sideVector).GetNormalized();

			Matrix4x4 rotation = new Matrix4x4(new Quaternion(inUp, Math.PI * 2.0 / inSegments));
			Vector3 currentDir = handDir;
			for (int i = 0; i <= inSegments; i++)
			{
				LocalBounds.Encapsulate(currentDir * inRadius + inCenter);
				currentDir = rotation * currentDir;
			}

			center = inCenter;
			segments = inSegments;
			up = inUp;

			radius = inRadius;
			PrimitiveColor = inColor;
		}

		public override void AddLines(RenderInfo info, ArrayList<VertexNormalColor> opaqueVertexData, ArrayList<VertexNormalColor> transparentVertexData)
		{
			ArrayList<VertexNormalColor> vertexData = (PrimitiveColor.A < 255 ? transparentVertexData : opaqueVertexData);

			Matrix4x4 rotation = new Matrix4x4(new Quaternion(up, Math.PI * 2.0 / segments));
			Vector3 currentDir = handDir;
			Vector3 oldDir = currentDir;
			for (int i = 1; i <= segments; i++)
			{
				currentDir = rotation * currentDir;

				vertexData.Add(new VertexNormalColor() { Position = oldDir * radius + center, Color = PrimitiveColor });
				vertexData.Add(new VertexNormalColor() { Position = currentDir * radius + center, Color = PrimitiveColor });

				oldDir = currentDir;
			}
		}

		protected override bool RayCheckInternal(int time, Vector3 rayStart, Vector3 rayDirection, out int triangleIndex, out bool hitOpaque, out Vector3 intersection)
		{
			// Cannot select circles at the moment (TBD)
			hitOpaque = false;
			triangleIndex = -1;
			intersection = new Vector3(0, 0, 0);

			return false;
		}

		public override PrimitiveSelectionInfo GetSelectionInfo(int triangleIndex, bool isSelectedTriangleOpaque)
		{
			return new CircleSelectionInfo(this, triangleIndex, isSelectedTriangleOpaque);
		}

		public Vector3 handDir;
		public Vector3 center;
		public Vector3 up;
		public double radius;
		public int segments;
	}

	public class CircleSelectionInfo : PrimitiveSelectionInfo
	{
		public CircleSelectionInfo(DebugRenderPrimitive selectedPrimitive, int triangleIndex, bool isSelectedTriangleOpaque)
			: base(selectedPrimitive, triangleIndex, isSelectedTriangleOpaque)
		{
			Color = Primitive.GetColor(TriangleIndex, IsSelectedTriangleOpaque);
			Center = (selectedPrimitive as DebugRenderCircle).center;
			Radius = (selectedPrimitive as DebugRenderCircle).radius;
			Up = (selectedPrimitive as DebugRenderCircle).up;
		}

		[CategoryAttribute("Circle Info"), ReadOnlyAttribute(true), DescriptionAttribute("Radius")]
		public double Radius
		{
			get;
			private set;
		}

		[CategoryAttribute("Circle Info"), ReadOnlyAttribute(true), DescriptionAttribute("Center")]
		public Vector3 Center
		{
			get;
			private set;
		}

		[CategoryAttribute("Circle Info"), ReadOnlyAttribute(true), DescriptionAttribute("Up")]
		public Vector3 Up
		{
			get;
			private set;
		}

		[CategoryAttribute("Circle Info"), ReadOnlyAttribute(true), DescriptionAttribute("Color")]
		public Color32 Color
		{
			get;
			private set;
		}
	}
}
