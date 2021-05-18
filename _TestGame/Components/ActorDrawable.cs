using _TestGame.GameObjects;
using _TestGame.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Components
{
	public class ActorDrawable : Drawable
	{
		public override Size Size
		{
			get
			{
				return new Size(16, 16);
			}
		}

		public override void Draw(Graphics g)
		{
			Actor actor = GameObject as Actor;

			Pen pen = new Pen(actor.Health < 3 ? Color.Red : actor.Health < 7 ? Color.Orange : Color.Black);

			Size size = Size;

			Spatial spatial = GameObject.GetComponent<Spatial>();
			if (spatial != null)
			{
				Matrix matrix = spatial.GetWorldMatrix();
				g.DrawEllipse(pen, matrix.OffsetX - size.Width / 2, matrix.OffsetY - size.Height / 2, size.Width, size.Height);
				PointF center = new PointF(matrix.OffsetX, matrix.OffsetY);
				PointF edge = new PointF(8.0f, 0.0f);
				PointF[] points = { edge };
				matrix.TransformPoints(points);

				g.DrawLine(pen, center, points[0]);

				if (DebugRenderer.Instance.DebugSelectedGameObject == GameObject)
				{
					// We are debug selected, draw selection bounds
					Pen debugPen = new Pen(Color.Black);
					g.DrawRectangle(debugPen, matrix.OffsetX - (size.Width / 2 + 2), matrix.OffsetY - (size.Height / 2 + 2), size.Width + 4, size.Height + 4);
					debugPen.Dispose();
				}
			}

			pen.Dispose();
		}
	}
}
