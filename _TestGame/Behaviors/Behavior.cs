using _TestGame.GameObjects;
using _TestGame.Managers;
using _TestGame.TestGame;
using ReView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _TestGame.Behaviors
{
	public class Behavior
	{
		public virtual void OnInit()
		{
#if _REVIEW_DEBUG
			debugObject = new DebugObject(SelectorComponent.DebugObject, Name, ReView.ReViewFeedObject.EDebugType.Item);
			debugObject.Log("Starting " + Name + " behavior");
#endif
		}

		public virtual void OnUninit()
		{
#if _REVIEW_DEBUG
			debugObject.Log("Terminating behavior");
			debugObject.End();
#endif
		}

		public Actor Actor
		{
			get
			{
				return SelectorComponent.GameObject as Actor;
			}
		}

		public BehaviorSelector SelectorComponent
		{
			get;
			set;
		}

		protected virtual string Name
		{
			get
			{
				return GetType().Name;
			}
		}

		public virtual bool OnUpdate(float inDeltaSeconds)
		{
			return false;
		}

#if _REVIEW_DEBUG
		public DebugObject debugObject;
#endif
	}
}
