using _TestGame.Behaviors;
using _TestGame.Components;
using _TestGame.Data;
using _TestGame.Managers;
using _TestGame.TestGame;
using ReView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _TestGame.GameObjects
{
	public class Actor : GameObject
	{
		public Actor()
		{
			TurnSpeed = SRandom.Float(45.0f, 135.0f); // degrees / second
			MoveSpeed = SRandom.Float(100.0f, 150.0f); // pixels / second
		}

		public override void OnInit()
		{
			base.OnInit();

#if _REVIEW_DEBUG
			DebugObject = new DebugObject(LevelManager.Instance.CurrentLevel.DebugObject, Name, ReView.ReViewFeedObject.EDebugType.Track);

			binaryDataFeed = ReViewFeedManager.Instance.RegisterBinaryDataFeed();
			if (binaryDataFeed != null)
			{
				binaryDataFeed.OnDataReceived += OnDataReceived;
			}
#endif

			LevelManager.Instance.CreateComponent<Spatial>(this);
			LevelManager.Instance.CreateComponent<ActorDrawable>(this);

			LevelManager.Instance.CreateComponent<BehaviorSelector>(this);
			LevelManager.Instance.CreateComponent<SensorComponent>(this);

			Weapon = LevelManager.Instance.CreateGameObject<Weapon>("Weapon");
			Weapon.Spatial.Parent = Spatial;

			Respawn(0.0f);
		}

		public override void OnUninit()
		{
			base.OnUninit();

#if _REVIEW_DEBUG
			if (binaryDataFeed != null)
			{
				binaryDataFeed.OnDataReceived -= OnDataReceived;
				ReViewFeedManager.Instance.UnregisterBinaryDataFeed(binaryDataFeed);
			}
#endif
		}

		public Weapon Weapon
		{
			get;
			private set;
		}

		private void Respawn(float inRespawnDelay)
		{
			Spatial.SetPosition(Vector2.Random(1024, 512));
			Spatial.SetFacing(SRandom.Float(360.0f));

			Blackboard = new Blackboard();

			Blackboard.SetFloat("RespawnDelay", inRespawnDelay);

			Health = 10;
		}

		public void Damage(int damageAmount)
		{
			Health -= damageAmount;
			if (Health <= 0)
			{
				Respawn(2.0f);
			}
		}

		public override bool OnUpdate(float inDeltaSeconds)
		{
			base.OnUpdate(inDeltaSeconds);

			return true;
		}

		public float TurnSpeed
		{
			get;
			set;
		}

		public float MoveSpeed
		{
			get;
			set;
		}

		public int Health
		{
			get
			{
				return debugHealth != Int32.MinValue ? debugHealth : health;
			}
			set
			{
				health = value;
			}
		}

		public Blackboard Blackboard
		{
			get;
			private set;
		}

		public override void OnPostUpdate()
		{
			base.OnPostUpdate();

#if _REVIEW_DEBUG
			if (GameUpdateManager.Instance.Running && binaryDataFeed != null)
			{
				MemoryStream stream = new MemoryStream();
				stream.Write(BitConverter.GetBytes(health), 0, 4);
				byte[] buffer = stream.ToArray();
				binaryDataFeed.Store(GameUpdateManager.Instance.Now, ref buffer);
			}
#endif
		}

#if _REVIEW_DEBUG
		public override void OnGamePlayStateChanged(bool isPlaying)
		{
			base.OnGamePlayStateChanged(isPlaying);

			if (isPlaying)
			{
				debugHealth = Int32.MinValue;
			}
		}

		protected void OnDataReceived(int time, ref byte[] data)
		{
			// Pause game if receiving data
			GameUpdateManager.Instance.SetRunning(false, true);

			if (!GameUpdateManager.Instance.Running)
			{
				debugHealth = BitConverter.ToInt32(data, 0);
			}
		}

		private ReViewFeedBinaryData binaryDataFeed;
		private int debugHealth = Int32.MinValue;
#endif

		private int health;
	}
}
