using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	public abstract class StateActions : ScriptableObject
	{
		public abstract void Execute(StateManager states);

	}
}