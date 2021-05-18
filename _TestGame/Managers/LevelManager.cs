using _TestGame.Components;
using _TestGame.GameObjects;
using _TestGame.TestGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _TestGame.Managers
{
	public class LevelManager
	{
		private LevelManager()
		{
		}

		public static LevelManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new LevelManager();
				}
				return instance;
			}
		}

		public void Shutdown()
		{
			instance = null;
		}

		public Level CurrentLevel
		{
			get { return currentLevel; }
			set { currentLevel = value; }
		}

		public T CreateGameObject<T>(string inName) where T : GameObject, new()
		{
			T newGameObject = new T();
			newGameObject.Name = inName;
			newGameObject.OnInit();

			gameObjects.Add(newGameObject);

			return newGameObject;
		}

		public T CreateComponent<T>(GameObject owner) where T : Component, new()
		{
			T newComponent = new T();
			owner.AddComponent(newComponent);
			newComponent.OnInit();
			return newComponent;
		}

		public void DestroyComponent(Component inComponent)
		{
			inComponent.OnUninit();
			inComponent.GameObject.RemoveComponent(inComponent);
			inComponent.GameObject = null;
		}

		public void DestroyGameObject(GameObject inGameObject)
		{
			inGameObject.OnUninit();
			inGameObject.RemoveComponents();

			gameObjects.Remove(inGameObject);
		}

		public IList<T> FindGameObjects<T>() where T : GameObject
		{
			return gameObjects.OfType<T>().ToList();
		}

		public T FindGameObject<T>() where T : GameObject
		{
			List<T> objects = gameObjects.OfType<T>().ToList();
			return objects != null && objects.Count > 0 ? objects.First() : null;
		}

		private List<GameObject> gameObjects = new List<GameObject>();
		private Level currentLevel = null;
		private static LevelManager instance = null;
	}
}