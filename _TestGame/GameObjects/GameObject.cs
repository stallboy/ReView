using _TestGame.Components;
using _TestGame.Managers;
using _TestGame.TestGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.GameObjects
{
	public class GameObject
	{
		public virtual void OnInit()
		{
			GameUpdateManager.Instance.Register(this);
		}

#if _REVIEW_DEBUG
		public DebugObject DebugObject
		{
			get;
			protected set;
		}
#endif

		public virtual void OnUninit()
		{
			GameUpdateManager.Instance.Unregister(this);
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

		public string Name
		{
			get;
			set;
		}

		public void RequestRemove()
		{
			RemoveRequested = true;
		}

		public Spatial Spatial
		{
			get
			{
				return GetComponent<Spatial>();
			}
		}

		public Drawable Drawable
		{
			get
			{
				return GetComponent<Drawable>();
			}
		}

		public bool RemoveRequested
		{
			get;
			private set;
		}

		public List<Component> Components
		{
			get
			{
				return components.ToList();
			}
		}

		#region Component management

		public void AddComponent(Component inComponent)
		{
			inComponent.GameObject = this;
			components.Add(inComponent);
		}

		public void RemoveComponent(Component inComponent)
		{
			inComponent.GameObject = null;
			components.Remove(inComponent);
		}

		public IList<T> GetComponents<T>() where T : Component
		{
			return components.OfType<T>().ToList();
		}

		public T GetComponent<T>() where T : Component
		{
			List<T> comps = components.OfType<T>().ToList();
			return comps != null && comps.Count > 0 ? comps.First() : null;
		}

		public void RemoveComponents()
		{
			while (components.Count > 0)
			{
				LevelManager.Instance.DestroyComponent(components[0]);
			}
		}

		#endregion

		private List<Component> components = new List<Component>();
	}
}
