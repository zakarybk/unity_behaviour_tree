using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/Has Detected Player")]
	public class HasDetectedPlayer : Condition
	{
		public void OnEnable()
		{
			description = "Has the player been detected?";
		}

		public override bool HasMetCondition(StateManager state)
		{
			return state.enemy.isPlayerDetected;
		}
	}
}