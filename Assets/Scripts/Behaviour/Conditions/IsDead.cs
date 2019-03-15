using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/Is Dead")]
	public class IsDead : Condition
	{
		public void OnEnable()
		{
			description = "Is the health below the minimum threshold?";
		}

		public override bool HasMetCondition(StateManager state)
		{
			return state.health <= 0;
		}
	}
}
