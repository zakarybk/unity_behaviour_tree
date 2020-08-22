using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/Should Purse")]
	public class ShouldPurse : Condition
	{
		public void OnEnable()
		{
			description = "Should the enemy purse the player?";
		}

		public override bool HasMetCondition(StateManager state)
		{
			return state.enemy.isPlayerDetected;
		}
	}
}