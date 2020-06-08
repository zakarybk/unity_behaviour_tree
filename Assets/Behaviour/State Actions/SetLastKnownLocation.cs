using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Actions/Set Last Known Location")]
	public class SetLastKnownLocation : StateActions
	{
		public override void Execute(StateManager states)
		{
			states.enemy.lastKnownLocation = states.player.transform.position;
		}

	}

}

