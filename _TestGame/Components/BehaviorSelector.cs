using _TestGame.Components;
using _TestGame.GameObjects;
using _TestGame.TestGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Behaviors
{
	public class BehaviorSelector : Component
	{
		public BehaviorSelector()
		{
			currentBehavior = null;
		}

		public override void OnInit()
		{
#if _REVIEW_DEBUG
			Actor actor = GameObject as Actor;
			DebugObject = new DebugObject(actor.DebugObject, "Behavior", ReView.ReViewFeedObject.EDebugType.Track);
#endif
		}

		public override void OnUninit()
		{
			if (currentBehavior != null)
			{
				currentBehavior.debugObject.LogError(">>> BehaviorSelector being shutdown! <<<");
			}
		}

		private Behavior SelectBehavior()
		{
			// Behavior selection logic
			Behavior selection = null;

			Actor actor = GameObject as Actor;
			if (actor.Blackboard.GetFloat("RespawnDelay") > 0.0f)
			{
				selection = new Wait();
			}
			else if (actor.Blackboard.Target != null)
			{
				selection = new Attack();
			}
			else
			{
				selection = new MoveRandomly();
			}

			return selection;
		}

		private void CreateNewBehavior(Behavior newBehavior)
		{
			if (currentBehavior != null)
			{
				// Uninit old behavior
				if (newBehavior != null)
				{
					// Being replaced by something else
					currentBehavior.debugObject.LogWarning(">>> Behavior being replaced by another behavior <<<");
				}
				currentBehavior.OnUninit();
				currentBehavior = null;
			}
			
			currentBehavior = newBehavior != null ? newBehavior : SelectBehavior();

			currentBehavior.SelectorComponent = this;

			if (currentBehavior != null)
			{
				// Init new behavior
				currentBehavior.OnInit();
			}
		}

		public override bool OnUpdate(float inDeltaSeconds)
		{
			if (currentBehavior != null)
			{
				Behavior newBehavior = SelectBehavior();
				if (currentBehavior.GetType() != newBehavior.GetType())
				{
					CreateNewBehavior(newBehavior);
				}

				if (!currentBehavior.OnUpdate(inDeltaSeconds))
				{
					CreateNewBehavior(null);
				}
			}
			else
			{
				CreateNewBehavior(null);
			}
			return true;
		}

		private Behavior currentBehavior;
	}
}
