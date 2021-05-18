using _TestGame.Managers;
using ReView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.TestGame
{
	public class DebugObject : ReViewFeedObject
	{
		public DebugObject(DebugObject inParentDebugObject, string inDebugName, EDebugType inType) : base(inParentDebugObject, inDebugName, GameUpdateManager.Instance.Now, inType)
		{
		}

		public DebugObject End()
		{
			EndBlock(GameUpdateManager.Instance.Now);
			return this;
		}

		public DebugObject Log(String message)
		{
			Log(message, GameUpdateManager.Instance.Now, 0);
			return this;
		}

		public DebugObject LogInfo(String message)
		{
			Log(message, GameUpdateManager.Instance.Now, 1);
			return this;
		}

		public DebugObject LogWarning(String message)
		{
			Log(message, GameUpdateManager.Instance.Now, 2);
			return this;
		}

		public DebugObject LogError(String message)
		{
			Log(message, GameUpdateManager.Instance.Now, 4);
			return this;
		}
	}
}
