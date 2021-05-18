using _TestGame.Components;
using _TestGame.GameObjects;
using _TestGame.Managers;
using ReView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Behaviors
{
	public class Attack : Behavior
	{
		public override void OnInit()
		{
			base.OnInit();

			turning = false;
		}

		private bool Turning
		{
			get
			{
				return turning;
			}
			set
			{
				if (turning != value)
				{
					turning = value;

					if (turning)
					{
						Actor.DebugObject.Log("Started turning");
					}
					else
					{
						Actor.DebugObject.Log("Stopped turning");
					}
				}
			}
		}

		public override bool OnUpdate(float inDeltaSeconds)
		{
			base.OnUpdate(inDeltaSeconds);

			if (Actor.Blackboard.Target != null)
			{
				Vector2 target = Actor.Blackboard.Target.Spatial.WorldPosition;

				float targetHeading = (float)Actor.Spatial.WorldPosition.AngleTo(target) * SMath.RAD2DEG;

				float headingDelta = SMath.GetDeltaAngle(Actor.Spatial.WorldHeading, targetHeading);
				float headingToChange = Math.Sign(headingDelta) * Math.Min(Math.Abs(headingDelta), inDeltaSeconds * Actor.TurnSpeed);

				Turning = Math.Abs(headingToChange) > 10.0f;

				if (Turning)
				{
					// Turn
					Actor.Spatial.SetFacing(Actor.Spatial.WorldHeading + headingToChange);
				}
				else
				{
					// Attack
					Actor.Weapon.FireRequested = true;
				}

				DebugRenderer.Instance.Line(Actor.Spatial.WorldPosition, target, turning ? Color.Blue : Color.Red);
			}

			return true;
		}

		private bool turning;
	}
}
