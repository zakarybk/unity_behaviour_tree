using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/Should Switch Route")]
	public class ShouldSwitchRoute : Condition
	{
		public void OnEnable()
		{
			description = "Should the enemy switch routes?";
		}

		public override bool HasMetCondition(StateManager state)
		{
            // If at the patrol point
            if (state.enemy.targetPath != null && state.enemy.path.IsOnPoint(state.enemy, state.enemy.nextPatrolPoint))
            {
                // If the point has a connection to the target route
                PatrolPoint point = state.enemy.path.PointAtIndex(state.enemy.nextPatrolPoint);

                if (point.HasConnectionToPath(state.enemy.targetPath))
                {
                    // Switch routes
                    return true;
                }
            }

            return false;
		}

        
	}
}
