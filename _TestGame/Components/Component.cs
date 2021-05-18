using _TestGame.GameObjects;
using _TestGame.TestGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Components
{
	public class Component
	{
		public virtual void OnInit()
		{
		}

		public virtual void OnUninit()
		{
		}

		public virtual void OnPreUpdate()
		{
		}

		public virtual bool OnUpdate(float inDeltaSeconds)
		{
			return true;
		}

		public virtual void OnPostUpdate()
		{
		}

		public virtual void OnGamePlayStateChanged(bool isPlaying)
		{
		}

		public void RequestRemove()
		{
			RemoveRequested = true;
		}

		public bool RemoveRequested
		{
			get;
			private set;
		}

		public GameObject GameObject
		{
			get
			{
				return owner;
			}
			set
			{
				if (owner != value)
				{
					owner = value;
				}
			}
		}

#if _REVIEW_DEBUG
		public DebugObject DebugObject
		{
			get;
			protected set;
		}
#endif

		private GameObject owner;
	}
}
