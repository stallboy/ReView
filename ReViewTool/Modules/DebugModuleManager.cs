using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReViewTool
{
	public class DebugModuleManager
	{
		private DebugModuleManager()
		{
		}

		public static DebugModuleManager Instance
		{
			get
			{
				// Double-checked locking
				if (sDebugModuleManager == null)
				{
					lock (sInstanceLock)
					{
						if (sDebugModuleManager == null)
						{
							sDebugModuleManager = new DebugModuleManager();
						}
					}
				}
				return sDebugModuleManager;
			}
			private set { }
		}

		public void OnResetSessions()
		{
			foreach (DebugModule module in debugModules.Values)
			{
				module.OnResetSessions();
			}
		}

		public void RegisterDebugModule(DebugModule module, bool isDefault = false)
		{
			if (!debugModules.ContainsKey(module.Name))
			{
				debugModules.Add(module.Name, module);
				module.OnInitDebugModule();
			}

			if (isDefault)
			{
				defaultModule = module;
			}
		}

		public void UnregisterDebugModule(DebugModule module)
		{
			if (debugModules.ContainsKey(module.Name))
			{
				debugModules.Remove(module.Name);
				module.OnDeactivateDebugModule();				
			}
		}

		public DebugModule GetDebugModule(string name)
		{
			if (debugModules.ContainsKey(name))
			{
				return debugModules[name];
			}
			return null;
		}

		public void Heartbeat(int time)
		{
			foreach (DebugModule module in debugModules.Values)
			{
				module.OnHeartbeat(time);
			}
		}

		public void TimelinePlaybackPositionChanged(int time)
		{
			foreach (DebugModule module in debugModules.Values)
			{
				module.OnTimelinePlaybackPositionChanged(time);
			}
		}

		public void TimelineZoomChanged(float timePixelRatio)
		{
			foreach (DebugModule module in debugModules.Values)
			{
				module.OnTimelineZoomChanged(timePixelRatio);
			}
		}

		public void TimelinePanOffsetChanged(System.Drawing.Point panOffset)
		{
			foreach (DebugModule module in debugModules.Values)
			{
				module.OnTimelinePanOffsetChanged(panOffset);
			}
		}

		public void RPCStateChanged(bool connected)
		{
			foreach (DebugModule module in debugModules.Values)
			{
				module.OnRPCStateChanged(connected);
			}
		}

		public DebugModule GetDefaultModule()
		{
			return defaultModule != null ? defaultModule : debugModules.Count > 0 ? debugModules.First().Value : null;
		}

		private Dictionary<string, DebugModule> debugModules = new Dictionary<string, DebugModule>();
		private DebugModule defaultModule;

		private static object sInstanceLock = new Object();
		private static DebugModuleManager sDebugModuleManager = null;
	}
}
