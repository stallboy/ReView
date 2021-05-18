using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TestGame.Behaviors
{
	public class Wait : Behavior
	{
		public Wait()
		{
		}

		public override void OnInit()
		{
			base.OnInit();

			debugObject.LogError("Waiting for respawn!");
		}

		public override bool OnUpdate(float inDeltaSeconds)
		{
			base.OnUpdate(inDeltaSeconds);

			Actor.Blackboard.SetFloat("RespawnDelay", Actor.Blackboard.GetFloat("RespawnDelay") - inDeltaSeconds);

			return Actor.Blackboard.GetFloat("RespawnDelay") > 0.0f;
		}
	}
}
