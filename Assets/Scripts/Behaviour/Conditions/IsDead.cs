﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	[CreateAssetMenu(menuName = "Conditions/Is Dead")]
	public class IsDead : Condition
	{
		public override bool HasMetCondition(StateManager state)
		{
			return state.health <= 0;
		}
	}
}
