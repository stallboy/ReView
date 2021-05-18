using _TestGame.GameObjects;
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
	public class WeaponDrawable : Drawable
	{
		public override void OnInit()
		{
			base.OnInit();

			weaponPen = new Pen(Color.DarkGray);
			firePen = new Pen(Color.Red);
		}

		public override void OnUninit()
		{
			base.OnUninit();

			weaponPen.Dispose();
			weaponPen = null;

			firePen.Dispose();
			firePen = null;
		}

		public override void Draw(Graphics g)
		{
			Weapon weapon = GameObject as Weapon;

			Spatial spatial = GameObject.GetComponent<Spatial>();
			if (spatial != null)
			{
				Actor actor = spatial.Parent.GameObject as Actor;

				Vector2 center = actor.Spatial.WorldPosition;

				g.DrawLine(weaponPen, Conversion.ToPoint(center + spatial.WorldForward * 8.0f), Conversion.ToPoint(center + spatial.WorldForward * 16.0f));

				if (weapon.Firing && actor.Blackboard.Target != null)
				{
					Vector2 a = center + spatial.WorldForward * 8.0f;
					Vector2 b = actor.Blackboard.Target.Spatial.WorldPosition;

					Vector2 direction = (b - a).Normalize();
					float distance = (float)(b - a).Length();
					float aFrac = SRandom.Float(0.0f, 0.4f);
					float bFrac = SRandom.Float(0.6f, 1.0f);

					g.DrawLine(firePen, Conversion.ToPoint(a + direction * aFrac * distance), Conversion.ToPoint(a + direction * bFrac * distance));
				}
			}
		}

		private Pen weaponPen;
		private Pen firePen;
	}
}
