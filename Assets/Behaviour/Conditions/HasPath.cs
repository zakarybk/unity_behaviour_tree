using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/Has Path")]
	public class HasPath : Condition
	{
		public void OnEnable()
		{
			description = "Has the enemy got anywhere to go?";
		}

		public override bool HasMetCondition(StateManager state)
		{
			return state.enemy.path != null && state.enemy.path.NumberOfPoints() > 0;
		}
	}
}