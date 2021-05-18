using _TestGame.Managers;
using _TestGame.TestGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _TestGame.GameObjects
{
	public class Level : GameObject
	{
		public Level()
		{
		}

		public override void OnInit()
		{
			base.OnInit();

#if _REVIEW_DEBUG
			DebugObject = new DebugObject(null, Name, ReView.ReViewFeedObject.EDebugType.Track);
#endif

			LevelManager.Instance.CurrentLevel = this;
			for (int k = 0; k < 5; k++)
			{
				string actorName = "Actor (" + k + ")";
				Actor actor = LevelManager.Instance.CreateGameObject<Actor>(actorName);
			}
		}
	}
}
