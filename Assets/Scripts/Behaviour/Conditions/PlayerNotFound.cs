using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/Player Not Found")]
	public class PlayerNotFound : Condition
	{
		public void OnEnable()
		{
			description = "Is the player nowhere to be seen?";
		}

		// No detection level and at last seen location
		public override bool HasMetCondition(StateManager state)
		{
			return state.enemy.detectionLevel <= 0 && (Vector3.Distance(
				state.enemy.lastKnownLocation, 
				state.enemy.transform.position
			) < 1 || !state.enemy.IsPathPossible(state.enemy.lastKnownLocation));
		}
	}
}
