﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Actions/Test/Add Health")]
	public class ChangeHealth : StateActions
	{
		public override void Execute(StateManager states)
		{
			//states.health += 10;
		}

	}
}
