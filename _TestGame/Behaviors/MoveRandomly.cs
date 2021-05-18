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
	public class MoveRandomly : Behavior
	{
		public override void OnInit()
		{
			base.OnInit();

			target = Vector2.Random(1024, 512);
			targetHeading = (float)Actor.Spatial.WorldPosition.AngleTo(target) * SMath.RAD2DEG;

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

			float headingDelta = SMath.GetDeltaAngle(Actor.Spatial.WorldHeading, targetHeading);
			float headingToChange = Math.Sign(headingDelta) * Math.Min(Math.Abs(headingDelta), inDeltaSeconds * Actor.TurnSpeed);

			Turning = Math.Abs(headingToChange) > 0.5f;

			if (Turning)
			{
				// Turn
				Actor.Spatial.SetFacing(Actor.Spatial.WorldHeading + headingToChange);
			}
			else
			{
				// Move
				Vector2 positionDelta = target - Actor.Spatial.WorldPosition;
				Vector2 positionToChange = positionDelta.Normalize() * Math.Min(positionDelta.Length(), Actor.MoveSpeed * inDeltaSeconds);

				if (positionToChange.Length() > 0.5f)
				{
					Actor.Spatial.SetPosition(Actor.Spatial.WorldPosition + positionToChange);
				}
				else
				{
					Actor.DebugObject.Log("Reached destination!");
					return false; // Done
				}
			}

			DebugRenderer.Instance.Rect(target, 3.0f, turning ? Color.Orange : Color.Green, true);
			DebugRenderer.Instance.Line(Actor.Spatial.WorldPosition, target, turning ? Color.Orange : Color.Green);

			return true;
		}

		private bool turning;

		private Vector2 target;
		private float targetHeading;
	}
}
