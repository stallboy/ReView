using _TestGame.Components;
using _TestGame.Managers;
using ReView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.GameObjects
{
	public class Weapon : GameObject
	{
		public override void OnInit()
		{
			base.OnInit();

			LevelManager.Instance.CreateComponent<Spatial>(this);
			LevelManager.Instance.CreateComponent<WeaponDrawable>(this);

			AttackCooldown = 0.0f;
		}

		public override void OnUninit()
		{
			base.OnUninit();
		}

		public override bool OnUpdate(float inDeltaSeconds)
		{
			base.OnUpdate(inDeltaSeconds);

			Actor actor = Spatial.Parent.GameObject as Actor;

			// Attack
			if (AttackCooldown > 0.0f)
			{
				AttackCooldown -= inDeltaSeconds;
			}

			if (FireRequested && actor.Blackboard.Target != null && AttackCooldown <= 0.0f)
			{
				Firing = true;
				(actor.Blackboard.Target as Actor).Damage(1);
				AttackCooldown = SRandom.Float(0.6f, 1.0f);
				FireRequested = false;
			}
			else
			{
				Firing = false;
			}

			return true;
		}

		private float AttackCooldown
		{
			get;
			set;
		}

		public bool FireRequested
		{
			get;
			set;
		}

		public bool Firing
		{
			get;
			set;
		}
	}
}
