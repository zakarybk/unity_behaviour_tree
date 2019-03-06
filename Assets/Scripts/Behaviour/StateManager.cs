﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	public class StateManager : MonoBehaviour
	{
		public float health;
		public State currentState;

		[HideInInspector]
		public float delta;

		private void Update()
		{
			if (currentState)
			{
				currentState.Tick(this);
			}
		}

	}
}
