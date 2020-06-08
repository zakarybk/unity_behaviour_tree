using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Actions/Update Light Colour")]
	public class UpdateLightColour : StateActions
	{
		public override void Execute(StateManager states)
		{
			Color colour;

			switch (states.currentState.name)
			{
				case "Idle":
					colour = states.enemy.config.idle;
					break;
                case "HeardSound":
				case "Searching":
					colour = states.enemy.config.searching;
					break;
				case "Pursuing":
					colour = states.enemy.config.pursuing;
					break;
				default:
					colour = Color.white;
					break;
			}

            if (states.enemy.GetComponentInChildren<Light>().color != colour)
			    states.enemy.GetComponentInChildren<Light>().color = colour;
		}
	}
}