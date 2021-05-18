using _TestGame.Managers;
using ReView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Components
{
	public class Drawable : Component
	{
		public override void OnInit()
		{
			RenderManager.Instance.Add(this);
		}

		public override void OnUninit()
		{
			RenderManager.Instance.Remove(this);
		}

		public virtual void Draw(Graphics g)
		{
		}

		public Vector2 WorldPosition
		{
			get
			{
				if (GameObject.Spatial != null)
				{
					Matrix matrix = GameObject.Spatial.GetWorldMatrix();
					return new Vector2(matrix.OffsetX, matrix.OffsetY);
				}
				return new Vector2(0, 0);
			}
		}

		public virtual Size Size
		{
			get
			{
				return new Size(0, 0);
			}
		}

		public Rectangle GetRenderBounds()
		{
			Vector2 pos = WorldPosition;
			return new Rectangle((int)pos.X - Size.Width / 2, (int)pos.Y - Size.Height / 2, Size.Width, Size.Height);
		}
	}
}
