using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/Has Lost Player")]
	public class HasLostPlayer : Condition
	{
		public void OnEnable()
		{
			description = "Has the enemy lost the player?";
		}

		public override bool HasMetCondition(StateManager state)
		{
			return !state.enemy.isPlayerDetected;
		}
	}
}