using _TestGame.Components;
using _TestGame.GameObjects;
using ReView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Managers
{
	[Serializable]
	public abstract class DrawCommand
	{
		public abstract void Draw(Graphics g);
	}

	[Serializable]
	public class DrawLineCommand : DrawCommand
	{
		public DrawLineCommand(Vector2 inStart, Vector2 inEnd, Color inColor)
		{
			start = inStart;
			end = inEnd;
			color = inColor;
		}

		public override void Draw(Graphics g)
		{
			Pen pen = new Pen(color);
			g.DrawLine(pen, new PointF((float)start.x, (float)start.y), new PointF((float)end.x, (float)end.y));
			pen.Dispose();
		}

		private Vector2 start;
		private Vector2 end;
		private Color color;
	}

	[Serializable]
	public class DrawCircleCommand : DrawCommand
	{
		public DrawCircleCommand(Vector2 inCenter, float inRadius, Color inColor, bool inFill)
		{
			center = inCenter;
			radius = inRadius;
			color = inColor;
			fill = inFill;
		}

		public override void Draw(Graphics g)
		{
			if (fill)
			{
				Brush brush = new SolidBrush(color);
				g.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2.0f, radius * 2.0f);
				brush.Dispose();
			}
			else
			{
				Pen pen = new Pen(color);
				g.DrawEllipse(pen, center.X - radius, center.Y - radius, radius * 2.0f, radius * 2.0f);
				pen.Dispose();
			}
		}

		private Vector2 center;
		private float radius;
		private Color color;
		private bool fill;
	}

	[Serializable]
	public class DrawRectCommand : DrawCommand
	{
		public DrawRectCommand(Vector2 inCenter, float inHalfSize, Color inColor, bool inFill)
		{
			center = inCenter;
			halfSize = inHalfSize;
			color = inColor;
			fill = inFill;
		}

		public override void Draw(Graphics g)
		{
			if (fill)
			{
				Brush brush = new SolidBrush(color);
				g.FillRectangle(brush, center.X - halfSize, center.Y - halfSize, halfSize * 2.0f, halfSize * 2.0f);
				brush.Dispose();
			}
			else
			{
				Pen pen = new Pen(color);
				g.DrawRectangle(pen, center.X - halfSize, center.Y - halfSize, halfSize * 2.0f, halfSize * 2.0f);
				pen.Dispose();
			}
		}

		private Vector2 center;
		private float halfSize;
		private Color color;
		private bool fill;
	}

	[Serializable]
	public class DrawTriangleCommand : DrawCommand
	{
		public DrawTriangleCommand(Vector2 inA, Vector2 inB, Vector2 inC, Color inColor, bool inFill)
		{
			a = inA;
			b = inB;
			c = inC;
			color = inColor;
			fill = inFill;
		}

		public override void Draw(Graphics g)
		{
			if (fill)
			{
				Brush brush = new SolidBrush(color);
				g.FillPolygon(brush, new Point[] { Conversion.ToPoint(a), Conversion.ToPoint(b), Conversion.ToPoint(c) });
				brush.Dispose();
			}
			else
			{
				Pen pen = new Pen(color);
				g.DrawPolygon(pen, new Point[] { Conversion.ToPoint(a), Conversion.ToPoint(b), Conversion.ToPoint(c) });
				pen.Dispose();
			}
		}

		private Vector2 a;
		private Vector2 b;
		private Vector2 c;
		private Color color;
		private bool fill;
	}

	public class DebugRenderer : Drawable, INotifyPropertyChanged
	{
		public static Color Alpha(Color inColor, int inAlpha)
		{
			return Color.FromArgb(inAlpha, inColor.R, inColor.G, inColor.B);
		}

		public static DebugRenderer Instance
		{
			get
			{
				// Double-checked locking
				if (sDebugRenderer == null)
				{
					lock (sInstanceLock)
					{
						if (sDebugRenderer == null)
						{
							GameObject go = LevelManager.Instance.CreateGameObject<GameObject>("DebugRenderer");
							sDebugRenderer = LevelManager.Instance.CreateComponent<DebugRenderer>(go);
						}
					}
				}
				return sDebugRenderer;
			}
			private set { }
		}

		public void Shutdown()
		{
			sDebugRenderer = null;
		}

		public DebugRenderer()
		{
			enabled = false;
		}

		public bool Enabled
		{
			get
			{
				return enabled;
			}
		}

		public void SetEnabled(bool value, bool userAction)
		{
			if (enabled != value)
			{
				enabled = value;

				NotifyPropertyChanged("Enabled");

#if _REVIEW_DEBUG
				if (userAction)
				{
					//ReViewFeedManager.Instance.DebugToggleChanged("DebugRenderer", enabled);
				}
#endif
			}
		}

		public void Line(Vector2 start, Vector2 end, Color color)
		{
			drawCommands.Add(new DrawLineCommand(start, end, color));
		}

		public void Circle(Vector2 center, float radius, Color color, bool fill)
		{
			drawCommands.Add(new DrawCircleCommand(center, radius, color, fill));
		}

		public void Rect(Vector2 center, float halfSize, Color color, bool fill)
		{
			drawCommands.Add(new DrawRectCommand(center, halfSize, color, fill));
		}

		public void Triangle(Vector2 a, Vector2 b, Vector2 c, Color color, bool fill)
		{
			drawCommands.Add(new DrawTriangleCommand(a, b, c, color, fill));
		}

		public override void OnInit()
		{
			base.OnInit();

#if _REVIEW_DEBUG
			binaryDataFeed = ReViewFeedManager.Instance.RegisterBinaryDataFeed();
			if (binaryDataFeed != null)
			{
				binaryDataFeed.OnDataReceived += OnDataReceived;
			}
#endif
		}

		public override void OnUninit()
		{
			base.OnUninit();

#if _REVIEW_DEBUG
			if (binaryDataFeed != null)
			{
				binaryDataFeed.OnDataReceived -= OnDataReceived;
				ReViewFeedManager.Instance.UnregisterBinaryDataFeed(binaryDataFeed);
			}
#endif
		}

		public GameObject DebugSelectedGameObject
		{
			get
			{
				return debugSelectedGameObject;
			}
		}

		public void SetDebugSelectedGameObject(GameObject newSelection, bool notify)
		{
			if (debugSelectedGameObject != newSelection)
			{
				debugSelectedGameObject = newSelection;

#if _REVIEW_DEBUG
				if (notify)
				{
					long selectedId = (debugSelectedGameObject != null && debugSelectedGameObject.DebugObject != null) ? debugSelectedGameObject.DebugObject.DebugID : -1;
					ReViewFeedManager.Instance.SelectionChanged(selectedId);
				}
#endif
			}
		}

		public override void OnPreUpdate()
		{
			base.OnPreUpdate();

			drawCommands.Clear();
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);

			if (Enabled)
			{
				foreach (DrawCommand drawCommand in drawCommands)
				{
					drawCommand.Draw(g);
				}
			}
		}

		public override void OnPostUpdate()
		{
			base.OnPostUpdate();

#if _REVIEW_DEBUG
			if (GameUpdateManager.Instance.Running && binaryDataFeed != null)
			{
				MemoryStream stream = new MemoryStream();
				stream.Write(BitConverter.GetBytes(drawCommands.Count()), 0, 4);
				IFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, drawCommands);

				byte[] buffer = stream.ToArray();
				binaryDataFeed.Store(GameUpdateManager.Instance.Now, ref buffer);
			}
#endif
		}

#if _REVIEW_DEBUG
		public override void OnGamePlayStateChanged(bool isPlaying)
		{
			base.OnGamePlayStateChanged(isPlaying);

			if (isPlaying)
			{
				drawCommands.Clear();
			}
		}

		protected void OnDataReceived(int time, ref byte[] data)
		{
			// Pause game if receiving data
			GameUpdateManager.Instance.SetRunning(false, true);

			if (!GameUpdateManager.Instance.Running)
			{
				int offset = 0;
				int count = BitConverter.ToInt32(data, offset); offset += 4;

				for (int i = 0; i < count; i++)
				{
					IFormatter formatter = new BinaryFormatter();
					MemoryStream stream = new MemoryStream(data, offset, data.Length - offset);
					stream.Seek(0, SeekOrigin.Begin);
					drawCommands = formatter.Deserialize(stream) as List<DrawCommand>;
				}
			}
		}
#endif

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

#if _REVIEW_DEBUG
		private ReViewFeedBinaryData binaryDataFeed;
#endif

		private List<DrawCommand> drawCommands = new List<DrawCommand>();

		private Pen defaultPen = new Pen(Color.Black);

		private GameObject debugSelectedGameObject = null;

		private bool enabled;

		private static object sInstanceLock = new Object();
		private static DebugRenderer sDebugRenderer = null;
	}
}
