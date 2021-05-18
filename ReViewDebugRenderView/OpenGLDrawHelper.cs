using ReView;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReViewDebugRenderView
{
	public class OpenGLDrawHelper
	{
		// Fill rectangle to given coordinates, Z assumed 0. Expected to be used when in Ortho mode.
		// Ensure states option is to ensure all enable/disable states to be set before drawing, if false then assumed that correct states are set (optimization)
		public static void FillRectangle(OpenGL gl, Vector2 topLeft, Vector2 bottomRight, Color32 color, bool ensureStates)
		{
			if (ensureStates)
			{
				gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
				gl.Disable(OpenGL.GL_LIGHTING);
				gl.Disable(OpenGL.GL_TEXTURE_2D);
				gl.Enable(OpenGL.GL_COLOR_MATERIAL);
				gl.Enable(OpenGL.GL_BLEND);
				gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
				gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
			}

			gl.Begin(OpenGL.GL_QUADS);

			gl.Color(color.R_Double, color.G_Double, color.B_Double, color.A_Double);

			gl.Vertex(topLeft.x, topLeft.y);
			gl.Vertex(bottomRight.x, topLeft.y);
			gl.Vertex(bottomRight.x, bottomRight.y);
			gl.Vertex(topLeft.x, bottomRight.y);
			gl.End();

			if (ensureStates)
			{
				gl.PopAttrib();
			}
		}

		public static void DrawTriangle(OpenGL gl, Vector3 a, Vector3 b, Vector3 c, Color32 color, bool usePolygonOffset)
		{
			gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
			gl.Disable(OpenGL.GL_LIGHTING);
			gl.Disable(OpenGL.GL_TEXTURE_2D);
			gl.Enable(OpenGL.GL_COLOR_MATERIAL);
			gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
			gl.Enable(OpenGL.GL_BLEND);

			if (usePolygonOffset)
			{
				gl.Enable(OpenGL.GL_POLYGON_OFFSET_FILL);
				gl.PolygonOffset(-1.0f, -1.0f);
			}

			gl.Begin(OpenGL.GL_TRIANGLES);

			gl.Color(color.R_Double, color.G_Double, color.B_Double, color.A_Double);

			gl.Vertex(a.x, a.y, a.z);
			gl.Vertex(b.x, b.y, b.z);
			gl.Vertex(c.x, c.y, c.z);
			
			gl.End();

			gl.PopAttrib();
		}

		public static void DrawBounds(OpenGL gl, Bounds bounds, Color32 color, bool usePolygonOffset)
		{
			if (!bounds.IsValid)
			{
				return;
			}

			gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
			gl.Disable(OpenGL.GL_LIGHTING);
			gl.Disable(OpenGL.GL_TEXTURE_2D);
			gl.Enable(OpenGL.GL_COLOR_MATERIAL);
			gl.Enable(OpenGL.GL_BLEND);
			gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			if (usePolygonOffset)
			{
				gl.DepthRange(0.0, 0.9);
			}

			gl.Begin(OpenGL.GL_LINES);

			gl.Color(color.R_Double, color.G_Double, color.B_Double, color.A_Double);

			// X lines
			gl.Vertex(bounds.Min.x, bounds.Min.y, bounds.Min.z);
			gl.Vertex(bounds.Max.x, bounds.Min.y, bounds.Min.z);

			gl.Vertex(bounds.Min.x, bounds.Max.y, bounds.Min.z);
			gl.Vertex(bounds.Max.x, bounds.Max.y, bounds.Min.z);

			gl.Vertex(bounds.Min.x, bounds.Min.y, bounds.Max.z);
			gl.Vertex(bounds.Max.x, bounds.Min.y, bounds.Max.z);

			gl.Vertex(bounds.Min.x, bounds.Max.y, bounds.Max.z);
			gl.Vertex(bounds.Max.x, bounds.Max.y, bounds.Max.z);

			// Y lines
			gl.Vertex(bounds.Min.x, bounds.Min.y, bounds.Min.z);
			gl.Vertex(bounds.Min.x, bounds.Max.y, bounds.Min.z);

			gl.Vertex(bounds.Max.x, bounds.Min.y, bounds.Min.z);
			gl.Vertex(bounds.Max.x, bounds.Max.y, bounds.Min.z);

			gl.Vertex(bounds.Min.x, bounds.Min.y, bounds.Max.z);
			gl.Vertex(bounds.Min.x, bounds.Max.y, bounds.Max.z);

			gl.Vertex(bounds.Max.x, bounds.Min.y, bounds.Max.z);
			gl.Vertex(bounds.Max.x, bounds.Max.y, bounds.Max.z);

			// Z lines
			gl.Vertex(bounds.Min.x, bounds.Min.y, bounds.Min.z);
			gl.Vertex(bounds.Min.x, bounds.Min.y, bounds.Max.z);

			gl.Vertex(bounds.Max.x, bounds.Min.y, bounds.Min.z);
			gl.Vertex(bounds.Max.x, bounds.Min.y, bounds.Max.z);

			gl.Vertex(bounds.Min.x, bounds.Max.y, bounds.Min.z);
			gl.Vertex(bounds.Min.x, bounds.Max.y, bounds.Max.z);

			gl.Vertex(bounds.Max.x, bounds.Max.y, bounds.Min.z);
			gl.Vertex(bounds.Max.x, bounds.Max.y, bounds.Max.z);

			gl.End();

			if (usePolygonOffset)
			{
				gl.DepthRange(0.0, 1.0);
			}

			gl.PopAttrib();
		}

		public static void DrawGrid_XY(OpenGL gl, Bounds bounds, int majorTicks, int minorTicks, Color32 color)
		{
			if (!bounds.IsValid)
			{
				bounds = new Bounds(new Vector2(-0.5, -0.5), new Vector2(0.5, 0.5));
			}

			gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
			gl.Disable(OpenGL.GL_LIGHTING);
			gl.Disable(OpenGL.GL_TEXTURE_2D);
			gl.Enable(OpenGL.GL_COLOR_MATERIAL);
			gl.Enable(OpenGL.GL_BLEND);
			gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			gl.Begin(OpenGL.GL_LINES);

			double snap = Math.Pow((int)Math.Log10(bounds.HalfSize.Length()) + 1, 10.0) * 0.5;
			Vector3 min = bounds.Min.Snap(snap);
			Vector3 max = bounds.Max.Snap(snap);

			int totalTicks = majorTicks * minorTicks;
			for (int i = 0; i <= totalTicks; i++)
			{
				double colorScale = (i % majorTicks == 0) ? 1.0 : 0.5;
				gl.Color(color.R_Double * colorScale, color.G_Double * colorScale, color.B_Double * colorScale, color.A_Double);

				double x = Vector3.Lerp(min, max, (double)i / (double)totalTicks).x;
				gl.Vertex(x, min.y, 0.0);
				gl.Vertex(x, max.y, 0.0);

				double y = Vector3.Lerp(min, max, (double)i / (double)totalTicks).y;
				gl.Vertex(min.x, y, 0.0);
				gl.Vertex(max.x, y, 0.0);
			}

			gl.End();

			gl.PopAttrib();
		}

		public static void DrawAxis(OpenGL gl, Vector3 position, Vector3 forward, Vector3 up, double scale)
		{
			gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
			gl.Disable(OpenGL.GL_LIGHTING);
			gl.Disable(OpenGL.GL_TEXTURE_2D);
			gl.Enable(OpenGL.GL_COLOR_MATERIAL);
			gl.Enable(OpenGL.GL_BLEND);
			gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			Vector3 side = forward.Cross(up);

			gl.Begin(OpenGL.GL_LINES);

			gl.Color(1.0, 0.0, 0.0, 1.0);
			gl.Vertex(position.x, position.y, position.z);
			gl.Vertex(position.x + side.x * scale, position.y + side.y * scale, position.z + side.z * scale);

			gl.Color(0.0, 1.0, 0.0, 1.0);
			gl.Vertex(position.x, position.y, position.z);
			gl.Vertex(position.x + up.x * scale, position.y + up.y * scale, position.z + up.z * scale);

			gl.Color(0.0, 0.0, 1.0, 1.0);
			gl.Vertex(position.x, position.y, position.z);
			gl.Vertex(position.x + forward.x * scale, position.y + forward.y * scale, position.z + forward.z * scale);

			gl.End();

			gl.PopAttrib();
		}
	}
}
