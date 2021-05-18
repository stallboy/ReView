using _TestGame.Components;
using _TestGame.GameObjects;
using ReView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _TestGame.Managers
{
	public class GameUpdateManager
	{
		private GameUpdateManager()
		{
			previousTicks = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			currentTime = 0;
#if _REVIEW_DEBUG
			//ReViewFeedManager.Instance.AddSelectionChangedListener(OnSelectionChanged);
#endif
		}

		/// <summary>
		/// Return singleton instance
		/// </summary>
		public static GameUpdateManager Instance
		{
			get
			{
				// Check if instance is null, if not then we can simply return the existing instance without having to use a lock
				if (instance == null)
				{
					lock (instanceLock)
					{
						// Check instance against null again since the first check is not thread-safe, this double checking is to prevent use of lock every time
						if (instance == null)
						{
							instance = new GameUpdateManager();
						}
					}
				}
				return instance;
			}
		}

		public void Shutdown()
		{
#if _REVIEW_DEBUG
			//ReViewFeedManager.Instance.RemoveSelectionChangedListener(OnSelectionChanged);
#endif
			instance = null;
		}

#if _REVIEW_DEBUG
		protected void OnSelectionChanged(long selectedId)
		{
			// Update components
			List<GameObject> updateablesCopy = new List<GameObject>(updateables);
			foreach (GameObject updateable in updateablesCopy)
			{
				if (updateable.DebugObject != null && updateable.DebugObject.DebugID == selectedId)
				{
					DebugRenderer.Instance.SetDebugSelectedGameObject(updateable, false);
					break;
				}

				foreach (Component component in updateable.Components)
				{
					if (component.DebugObject != null && component.DebugObject.DebugID == selectedId)
					{
						DebugRenderer.Instance.SetDebugSelectedGameObject(updateable, false);
						break;
					}
				}
			}
		}
#endif

		public void Register(GameObject inGameObject)
		{
			updateables.Add(inGameObject);
		}

		public void Unregister(GameObject inGameObject)
		{
			updateables.Remove(inGameObject);
		}

		public void Update()
		{
			lock (UpdateLock)
			{
				if (Running)
				{
					UpdateTime();

					List<GameObject> updateablesCopy = new List<GameObject>(updateables);

					// Pre-update components
					foreach (GameObject updateable in updateablesCopy)
					{
						foreach (Component component in updateable.Components)
						{
							component.OnPreUpdate();
						}
					}

					// Update components
					foreach (GameObject updateable in updateablesCopy)
					{
						foreach (Component component in updateable.Components)
						{
							if (component.RemoveRequested || !component.OnUpdate(DeltaSeconds))
							{
								LevelManager.Instance.DestroyComponent(component);
							}
						}
					}

					// Pre-update gameobjects
					foreach (GameObject updateable in updateablesCopy)
					{
						updateable.OnPreUpdate();
					}

					// Update gameobjects
					foreach (GameObject updateable in updateablesCopy)
					{
						if (updateable.RemoveRequested || !updateable.OnUpdate(DeltaSeconds))
						{
							LevelManager.Instance.DestroyGameObject(updateable);
						}
					}

					// Post-update components
					foreach (GameObject updateable in updateablesCopy)
					{
						foreach (Component component in updateable.Components)
						{
							component.OnPostUpdate();
						}
					}

					// Post-update gameobjects
					foreach (GameObject updateable in updateablesCopy)
					{
						updateable.OnPostUpdate();
					}
				}
				RenderManager.Instance.RequestFrame = true;
			}
		}

		public void UpdateTime()
		{
			long tickDelta = DateTime.Now.Ticks - previousTicks;
			previousTicks = DateTime.Now.Ticks;
			deltaTime = (int)(tickDelta / TimeSpan.TicksPerMillisecond);
			currentTime += deltaTime;
		}

		public float Random
		{
			get { return (float)rand.NextDouble(); }
		}

		public int Now
		{
			get { return currentTime; }
		}

		public int Delta
		{
			get { return deltaTime; }
		}

		public float TimeSeconds
		{
			get { return (float)currentTime / 1000.0f; }
		}

		public float DeltaSeconds
		{
			get { return (float)deltaTime / 1000.0f; }
		}

		public bool Running
		{
			get
			{
				return running;
			}
		}

		public void SetRunning(bool value, bool userAction)
		{
			if (running != value)
			{
				running = value;

				if (running)
				{
					previousTicks = DateTime.Now.Ticks;
				}

				// Update components
				NotifyGamePlayStateChanged(running);

#if _REVIEW_DEBUG
				if (userAction)
				{
					ReViewFeedManager.Instance.DebugToggleChanged("Running", running);
				}
#endif
			}
		}

		private void NotifyGamePlayStateChanged(bool isRunning)
		{
			List<GameObject> updateablesCopy = new List<GameObject>(updateables);
			foreach (GameObject updateable in updateablesCopy)
			{
				foreach (Component component in updateable.Components)
				{
					component.OnGamePlayStateChanged(isRunning);
				}
				updateable.OnGamePlayStateChanged(isRunning);
			}

			DlgRunningStateChanged handler = RunningStateChanged;
			if (handler != null)
			{
				handler(isRunning);
			}
		}

		public delegate void DlgRunningStateChanged(bool newStateIsRunning);

		public event DlgRunningStateChanged RunningStateChanged;

		private Random rand = new Random();
		private List<GameObject> updateables = new List<GameObject>();
		private int deltaTime = 0;
		private int currentTime = 0;
		private long previousTicks = 0;

		private bool running;

		private static GameUpdateManager instance = null;
		private static object instanceLock = new object();

		public static Object UpdateLock = new Object();
	}
}
