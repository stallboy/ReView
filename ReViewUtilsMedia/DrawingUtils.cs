using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ReView
{
	/// <summary>
	/// Random utility methods to help with drawing
	/// </summary>
	public class DrawingUtils
	{
		public static Point ToPoint(Vector2 vector)
		{
			return new Point((int)vector.x, (int)vector.y);
		}

		public static PointF ToPointF(Vector2 vector)
		{
			return new PointF((float)vector.x, (float)vector.y);
		}

		public static Color BlendColors(Color colorA, Color colorB, float alpha)
		{
			float oneMinusAlpha = 1.0f - alpha;
			return Color.FromArgb((int)(colorA.R * oneMinusAlpha + colorB.R * alpha), (int)(colorA.G * oneMinusAlpha + colorB.G * alpha), (int)(colorA.B * oneMinusAlpha + colorB.B * alpha));
		}

		public static void DrawArrow(Graphics g, Pen pen, Vector2 vStart, Vector2 vEnd, float fSize, bool bBiDirectional)
		{
			Vector2 vToEndDir = (vEnd - vStart);
			if (!vToEndDir.IsZero())
			{
				vToEndDir = vToEndDir.Normalize();
				Vector2 vRight = (vToEndDir.Cross() - vToEndDir) / 2.0f * fSize;
				Vector2 vLeft = (-vToEndDir.Cross() - vToEndDir) / 2.0f * fSize;

				g.DrawLine(pen, ToPoint(vStart), ToPoint(vEnd));
				g.DrawLine(pen, ToPoint(vEnd), ToPoint(vEnd + vRight));
				g.DrawLine(pen, ToPoint(vEnd), ToPoint(vEnd + vLeft));
				if (bBiDirectional)
				{
					g.DrawLine(pen, ToPoint(vStart), ToPoint(vStart - vRight));
					g.DrawLine(pen, ToPoint(vStart), ToPoint(vStart - vLeft));
				}
			}
		}

		public static void FillRectangle(Graphics g, Rectangle rect, Color backgroundColor, Color borderColor, Color frontColor, String text, StringFormat format, Point textPosition)
		{
			Rectangle rectA = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

			// Enable anti-alias
			g.SmoothingMode = SmoothingMode.AntiAlias;

			// Draw content filled rectangle
			if (rectA.Width >= 1.5f)
			{
				Brush sectionBrush = new System.Drawing.Drawing2D.LinearGradientBrush(rectA, backgroundColor, BlendColors(backgroundColor, Color.Black, 0.4f), System.Drawing.Drawing2D.LinearGradientMode.Vertical);
				g.FillRectangle(sectionBrush, rectA);
				sectionBrush.Dispose();
			}

			// Draw text
			if (text != null)
			{
				Region oldClip = g.Clip;
				RectangleF clipRect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
				clipRect.Intersect(g.ClipBounds);
				g.SetClip(clipRect);

				Font font = new Font("Tahoma", 8, FontStyle.Regular);
				Brush brush = new SolidBrush(frontColor);
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.DrawString(text, font, brush, textPosition, format);
				g.SmoothingMode = SmoothingMode.None;

				font.Dispose();
				brush.Dispose();
				format.Dispose();

				g.Clip = oldClip;
			}

			// Draw border
			Pen borderPen = new Pen(Color.FromArgb(192, borderColor.R, borderColor.G, borderColor.B));
			g.DrawRectangle(borderPen, rectA);
			borderPen.Dispose();

			g.SmoothingMode = SmoothingMode.None;
		}

		public static void FillRectangle(Graphics g, Rectangle rect, Color backgroundColor, Color frontColor, String text, StringFormat format, Point textPosition)
		{
			Region oldClip = g.Clip;
			RectangleF clipRect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
			clipRect.Intersect(g.ClipBounds);
			g.SetClip(clipRect);

			Rectangle rectA = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
			Rectangle rectB = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height / 2 - 1);

			Color colorBrighter = BlendColors(backgroundColor, Color.White, 0.2f);
			Color highlightColorA = Color.FromArgb(16, 255, 255, 255);
			Color highlightColorB = Color.FromArgb(64, 255, 255, 255);

			// Create brushes
			Brush sectionBrush = new SolidBrush(backgroundColor);// new System.Drawing.Drawing2D.LinearGradientBrush(rectA, backgroundColor, colorBrighter, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
			Brush highlightBrush = new System.Drawing.Drawing2D.LinearGradientBrush(rectB, highlightColorA, highlightColorB, System.Drawing.Drawing2D.LinearGradientMode.Vertical);

			// Create pens
			Pen highlightPen = new Pen(Color.FromArgb(192, 255, 255, 255));
			Pen shadowPen = new Pen(Color.FromArgb(192, 0, 0, 0));

			// Draw (Anti-Aliased)
			g.SmoothingMode = SmoothingMode.AntiAlias;

			g.FillRectangle(sectionBrush, rectA); // Box

			if (text != null)
			{
				// Draw text
				Font font = new Font("Tahoma", 8, FontStyle.Regular);
				Brush brush = new SolidBrush(frontColor);
				
				//g.SmoothingMode = SmoothingMode.AntiAlias;
				g.DrawString(text, font, brush, textPosition, format);
				g.SmoothingMode = SmoothingMode.None;

				font.Dispose();
				brush.Dispose();
				format.Dispose();
			}

			//g.FillRectangle(highlightBrush, rectB); // Highlight

			// 3D-borders
			/*
			g.DrawLine(highlightPen, rectA.X, rectA.Y, rectA.X + rectA.Width, rectA.Y);
			g.DrawLine(highlightPen, rectA.X, rectA.Y, rectA.X, rectA.Y + rectA.Height);
			g.DrawLine(shadowPen, rectA.X + rectA.Width, rectA.Y, rectA.X + rectA.Width, rectA.Y + rectA.Height);
			g.DrawLine(shadowPen, rectA.X, rectA.Y + rectA.Height, rectA.X + rectA.Width, rectA.Y + rectA.Height);
			*/
			g.SmoothingMode = SmoothingMode.None;

			// Dispose pens
			highlightPen.Dispose();
			shadowPen.Dispose();

			// Dispose brushes
			sectionBrush.Dispose();
			highlightBrush.Dispose();

			g.Clip = oldClip;
		}

		public static void FillRectangle(Graphics g, Rectangle rect, Color color)
		{
			FillRectangle(g, rect, color, Color.White, null, null, Point.Empty);
		}

		[Flags]
		public enum Corners
		{
			C_UpperLeft = 1,
			C_UpperRight = 2,
			C_LowerLeft = 4,
			C_LowerRight = 8,

			C_Upper = 3,
			C_Left = 5,
			C_Lower = 12,
			C_Right = 10,
	
			C_All = 15
		};

		public static void FillRoundedRectangle(Graphics g, Rectangle rect, int radius, Color color, Corners roundedCorners, Color shadowColor, Point shadowOffset)
		{
			Color colorOther = DrawingUtils.BlendColors(color, Color.White, 1.0f);
			Brush background = new System.Drawing.Drawing2D.LinearGradientBrush(rect, colorOther, color, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
			Brush shadow = new System.Drawing.SolidBrush(shadowColor);

			int x = rect.X;
			int y = rect.Y;
			int b = y + rect.Height;
			int r = x + rect.Width;

			GraphicsPath backgroundPath = new GraphicsPath();
			if ((roundedCorners & Corners.C_UpperLeft) != 0)
			{
				// Draw rounded upper-left
				backgroundPath.AddArc(x, y, radius * 2, radius * 2, 180, 90);
			} else
			{
				// Draw flat upper-left
				backgroundPath.AddLine(x, y + radius, x, y);
				backgroundPath.AddLine(x, y, radius, y);
			}

			// Draw top
			backgroundPath.AddLine(x + radius, y, r - radius, y);

			if ((roundedCorners & Corners.C_UpperRight) != 0)
			{
				// Draw rounded upper-right
				backgroundPath.AddArc(r - radius * 2, y, radius * 2, radius * 2, 270, 90);
			} else
			{
				// Draw flat upper-right
				backgroundPath.AddLine(r - radius, y, r, y);
				backgroundPath.AddLine(r, y, r, y + radius);
			}

			// Draw right
			backgroundPath.AddLine(r, y + radius, r, b - radius);

			if ((roundedCorners & Corners.C_LowerRight) != 0)
			{
				// Draw rounded lower-right
				backgroundPath.AddArc(r - radius * 2, b - radius * 2, radius * 2, radius * 2, 0, 90);
			}
			else
			{
				// Draw flat lower-right
				backgroundPath.AddLine(r, b - radius, r, b);
				backgroundPath.AddLine(r, b, r - radius, b);
			}

			// Draw bottom
			backgroundPath.AddLine(r - radius, b, x + radius, b);

			if ((roundedCorners & Corners.C_LowerLeft) != 0)
			{
				// Draw rounded lower-left
				backgroundPath.AddArc(x, b - radius * 2, radius * 2, radius * 2, 90, 90);
			}
			else
			{
				// Draw flat lower-left
				backgroundPath.AddLine(x + radius, b, x, b);
				backgroundPath.AddLine(x, b, x, b - radius);
			}

			// Draw left
			backgroundPath.AddLine(x, b - radius, x, y + radius);

			backgroundPath.CloseFigure();

			g.SmoothingMode = SmoothingMode.AntiAlias;

			g.FillPath(background, backgroundPath);
			if (shadowColor != null)
			{
				g.FillPath(shadow, backgroundPath);

				Matrix translateMatrix = new Matrix();
				translateMatrix.Translate(shadowOffset.X, shadowOffset.Y);
				backgroundPath.Transform(translateMatrix);
				g.FillPath(background, backgroundPath);
			}

			g.SmoothingMode = SmoothingMode.None;

			background.Dispose();
			shadow.Dispose();

			backgroundPath.Dispose();
		}

		public static void FillTriangle(Graphics g, Rectangle rect, Color color, float angle = 0.0f)
		{
			Brush background = new SolidBrush(color);

			int x = rect.X;
			int y = rect.Y;
			int w = rect.Width;
			int h = rect.Height;

			GraphicsPath backgroundPath = new GraphicsPath();
			backgroundPath.AddLine(x, y, x + w, y);
			backgroundPath.AddLine(x + w, y, x + w / 2, y + h);
			backgroundPath.AddLine(x + w / 2, y + h, x, y);
			backgroundPath.CloseFigure();

			Matrix rotateMatrix = new Matrix();
			rotateMatrix.Translate((x + w / 2), (y + h / 2));
			rotateMatrix.Rotate(angle);
			rotateMatrix.Translate(-(x + w / 2), -(y + h / 2));
			backgroundPath.Transform(rotateMatrix);

			g.SmoothingMode = SmoothingMode.AntiAlias;

			g.FillPath(background, backgroundPath);

			g.SmoothingMode = SmoothingMode.None;

			background.Dispose();
			backgroundPath.Dispose();
		}

		public static void FillPointer(Graphics g, Rectangle rect, int tipSize, int thickness, Color color, Color borderColor, float angle = 0.0f)
		{
			Color colorOther = DrawingUtils.BlendColors(color, Color.White, 1.0f);
			Brush background = new LinearGradientBrush(rect, colorOther, color, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
			Pen borderPen = new Pen(borderColor);

			int x = rect.X;
			int y = rect.Y;
			int w = rect.Width;
			int h = rect.Height;

			GraphicsPath backgroundPath = new GraphicsPath();
			backgroundPath.AddLine(x, y, x + w, y);
			backgroundPath.AddLine(x + w, y, x + w, y + h - tipSize);
			backgroundPath.AddLine(x + w, y + h - tipSize, x + w - thickness, y + h - tipSize);
			backgroundPath.AddLine(x + w - thickness, y + h - tipSize, x + w - thickness, y + thickness);
			backgroundPath.AddLine(x + w - thickness, y + thickness, x + thickness, y + thickness);
			backgroundPath.AddLine(x + thickness, y + thickness, x + thickness, y + h - tipSize);
			backgroundPath.AddLine(x + thickness, y + h - tipSize, x + w / 2, y + h - thickness);
			backgroundPath.AddLine(x + w / 2, y + h - thickness, x + w - thickness, y + h - tipSize);
			backgroundPath.AddLine(x + w - thickness, y + h - tipSize, x + w, y + h - tipSize);
			backgroundPath.AddLine(x + w, y + h - tipSize, x + w / 2, y + h);
			backgroundPath.AddLine(x + w / 2, y + h, x, y + h - tipSize);
			backgroundPath.AddLine(x, y + h - tipSize, x, y);
			backgroundPath.CloseFigure();

			Matrix rotateMatrix = new Matrix();
			rotateMatrix.Translate((x + w / 2), (y + h / 2));
			rotateMatrix.Rotate(angle);
			rotateMatrix.Translate(-(x + w / 2), -(y + h / 2));
			backgroundPath.Transform(rotateMatrix);

			g.SmoothingMode = SmoothingMode.AntiAlias;

			g.FillPath(background, backgroundPath);
			g.DrawPath(borderPen, backgroundPath);

			g.SmoothingMode = SmoothingMode.None;

			background.Dispose();
			borderPen.Dispose();
			backgroundPath.Dispose();
		}
	}
}
