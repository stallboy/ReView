using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;
using ReView;
using _TestGame.Managers;
using _TestGame.GameObjects;
using System.Windows.Forms;
using System.Drawing;

namespace _TestGame.TestGame
{
	public class Game
	{
		public static string debugServerAddress = "localhost";
		public static int debugServerPort = 5000;

		public Game(GamePanel inPanel)
		{
			gamePanel = inPanel;
			gamePanel.MouseDown += MouseDown;
#if _REVIEW_DEBUG
			Console.Write("Trying to connect to '" + debugServerAddress + ":" + debugServerPort + "'...");
			if (ReViewFeedManager.Instance.Connect(debugServerAddress, debugServerPort))
			{
				Console.WriteLine("OK");
			}
			else
			{
				Console.WriteLine("FAIL!");
			}
#endif
			Start();
		}

#if _REVIEW_DEBUG
		protected void OnDebugToggleChanged(string name, bool state)
		{
			if (name.Equals("DebugRenderer", StringComparison.CurrentCultureIgnoreCase))
			{
				DebugRenderer.Instance.SetEnabled(state, false);
			}
			else if (name.Equals("Running", StringComparison.CurrentCultureIgnoreCase))
			{
				GameUpdateManager.Instance.SetRunning(state, false);
			}
		}
#endif

		protected void MouseDown(object sender, MouseEventArgs e)
		{
			GameObject go = RenderManager.Instance.GetGameObjectAt(e.X, e.Y);
			DebugRenderer.Instance.SetDebugSelectedGameObject(go, true);
		}

		public void Start()
		{
			lock (threadOpLock)
			{
				if (clientThread == null)
				{
					Console.WriteLine("Starting game main thread");

					clientThread = new Thread(new ThreadStart(ClientMain));
					clientThread.IsBackground = true;
					clientThread.Start();
				}
			}
		}

		public void Shutdown()
		{
			lock (threadOpLock)
			{
#if _REVIEW_DEBUG
				Console.WriteLine("Disconnecting debug manager");

				ReViewFeedManager.Instance.Disconnect();
#endif
				if (clientThread != null)
				{
					Console.WriteLine("Stopping game main thread");

					running = false;

					clientThread.Join();
					clientThread = null;
				}
			}

			GameUpdateManager.Instance.Shutdown();
			RenderManager.Instance.Shutdown();
			DebugRenderer.Instance.Shutdown();
			LevelManager.Instance.Shutdown();
		}

		public void ClientMain()
		{
			Console.WriteLine("Game main loop start");

			Level level = LevelManager.Instance.CreateGameObject<Level>("Level01");

			running = true;
			while (running)
			{
				GameUpdateManager.Instance.Update();

#if _REVIEW_DEBUG
				ReViewFeedManager.Instance.Update(GameUpdateManager.Instance.Now, GameUpdateManager.Instance.Delta);
#endif
				gamePanel.Invalidate();

				Thread.Sleep(33);
			}

			Console.WriteLine("Game main loop end");

			NotifyShutdown();
		}

		public delegate void DlgShutdown(object sender);
		public event DlgShutdown OnShutdown;

		private void NotifyShutdown()
		{
			DlgShutdown handler = OnShutdown;
			if (handler != null)
			{
				handler(this);
			}
		}

		private GamePanel gamePanel;

		private Thread clientThread = null;
		private bool running = false;

		private Object threadOpLock = new Object();
	}
}
