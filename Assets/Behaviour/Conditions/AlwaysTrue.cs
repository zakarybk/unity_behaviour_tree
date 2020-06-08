using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/Always True")]
	public class AlwaysTrue : Condition
	{
		public void OnEnable()
		{
			description = "This is always true";
		}

		public override bool HasMetCondition(StateManager state)
		{
			return true;
		}
	}
}