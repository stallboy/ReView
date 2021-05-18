using _TestGame.GameObjects;
using _TestGame.Managers;
using _TestGame.TestGame;
using ReView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Components
{
	public class SensorComponent : Component
	{
		public override void OnInit()
		{
			perceptionFov = 27.0f * SMath.DEG2RAD;
			perceptionDistance = 150.0f;

#if _REVIEW_DEBUG
			Actor actor = GameObject as Actor;
			DebugObject = new DebugObject(actor.DebugObject, "Sensor", ReView.ReViewFeedObject.EDebugType.Track);
#endif
		}

		public override void OnUninit()
		{
		}

		public static Vector2 RotateVector(Vector2 inVector, float inAngle)
		{
			System.Drawing.Drawing2D.Matrix rotationMatrix = new Matrix();
			rotationMatrix.Rotate(inAngle * SMath.RAD2DEG);
			PointF[] points = { new PointF(inVector.X, inVector.Y) };
			rotationMatrix.TransformVectors(points);
			return new Vector2(points[0].X, points[0].Y);
		}

		public override bool OnUpdate(float inDeltaSeconds)
		{
			// Aquire target

			Actor self = GameObject as Actor;
			Vector2 selfPosition = self.Spatial.WorldPosition;
			Vector2 selfForward = self.Spatial.WorldForward;

			DebugRenderer.Instance.Triangle(selfPosition, selfPosition + RotateVector(selfForward, -perceptionFov) * perceptionDistance, selfPosition + RotateVector(selfForward, perceptionFov) * perceptionDistance, DebugRenderer.Alpha(System.Drawing.Color.Blue, 128), true);

			IList<Actor> actors = LevelManager.Instance.FindGameObjects<Actor>();

			Actor foundTarget = null;
			
			float closestDistance = float.MaxValue;
			foreach (Actor actor in actors)
			{
				if (actor == self)
				{
					// Skip self
					continue;
				}

				Vector2 toOther = actor.Spatial.WorldPosition - selfPosition;
				float dot = (float)toOther.Normalize().Dot(selfForward);
				if (dot >= Math.Cos(perceptionFov))
				{
					// Within field-of-vision
					float distance = (float)toOther.Length();
					if (distance <= perceptionDistance)
					{
						// Within the perception range
						if (distance < closestDistance)
						{
							closestDistance = distance;
							foundTarget = actor;
						}
					}
				}
			}

			if (self.Blackboard.Target != foundTarget)
			{
				// Target changed
				new DebugObject(DebugObject, "Event", ReViewFeedObject.EDebugType.GenericItem).Log("New target '" + (foundTarget != null ? foundTarget.Name : "<none>") + "' aquired!").End();

				self.Blackboard.Target = foundTarget;
			}

			return true;
		}

		private float perceptionFov;
		private float perceptionDistance;
	}
}
