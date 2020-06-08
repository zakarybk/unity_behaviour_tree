using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Actions/Move Towards Player")]
	public class MoveTowardsPlayer : StateActions
	{
		public override void Execute(StateManager states)
		{
			states.enemy.MoveToPosition(states.player.transform.position);
		}

	}

}
