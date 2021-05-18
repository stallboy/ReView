using _TestGame.Components;
using _TestGame.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Managers
{
	public class RenderManager
	{
		public static RenderManager Instance
		{
			get
			{
				// Double-checked locking
				if (sRenderManager == null)
				{
					lock (sInstanceLock)
					{
						if (sRenderManager == null)
						{
							sRenderManager = new RenderManager();
						}
					}
				}
				return sRenderManager;
			}
			private set { }
		}

		public void Shutdown()
		{
			sRenderManager = null;
		}

		public void Add(Drawable inDrawable)
		{
			lock (drawablesLock)
			{
				drawables.Add(inDrawable);
			}
		}

		public void Remove(Drawable inDrawable)
		{
			lock (drawablesLock)
			{
				drawables.Remove(inDrawable);
			}
		}

		public void Draw(Graphics g)
		{
			lock (GameUpdateManager.UpdateLock)
			{
				if (RequestFrame)
				{
					lock (drawablesLock)
					{
						List<Drawable> drawablesCopy = drawables.ToList();
						foreach (Drawable drawable in drawablesCopy)
						{
							drawable.Draw(g);
						}
					}
				}
			}
		}

		public GameObject GetGameObjectAt(int x, int y)
		{
			lock (drawablesLock)
			{
				List<Drawable> drawablesCopy = drawables.ToList();
				foreach (Drawable drawable in drawablesCopy)
				{
					if (drawable.GetRenderBounds().Contains(x, y))
					{
						return drawable.GameObject;
					}
				}
			}
			return null;
		}

		public bool RequestFrame
		{
			get;
			set;
		}

		private object drawablesLock = new object();
		private List<Drawable> drawables = new List<Drawable>();

		private static object sInstanceLock = new Object();
		private static RenderManager sRenderManager = null;
	}
}
