using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/Always False")]
	public class AlwaysFalse : Condition
	{
		public void OnEnable()
		{
			description = "This is always false";
		}

		public override bool HasMetCondition(StateManager state)
		{
			return false;
		}
	}
}