using _TestGame.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _TestGame.TestGame
{
	public class GamePanel : Panel
	{
		public GamePanel()
		{
			DoubleBuffered = true;
		}

		public Game Game
		{
			get;
			set;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			RenderManager.Instance.Draw(g);
		}
	}
}
