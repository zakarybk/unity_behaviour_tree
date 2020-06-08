using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Actions/Stop Moving")]
	public class StopMoving : StateActions
	{
		public override void Execute(StateManager states)
		{
			states.enemy.MoveToPosition(states.enemy.transform.position);
		}

	}

}