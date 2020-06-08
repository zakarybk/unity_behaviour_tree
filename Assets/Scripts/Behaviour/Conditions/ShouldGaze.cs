using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/Should Gaze")]
	public class ShouldGaze : Condition
	{
		public void OnEnable()
		{
			description = "Should the enemy gaze?";
		}

		public override bool HasMetCondition(StateManager state)
		{
            // Don't stop and gaze if you need to get somewhere
            if (state.enemy.targetPath == null)
            {
                PatrolPoint point = state.enemy.path.PointAtIndex(state.enemy.nextPatrolPoint);

                if (point.ShouldGaze())
                {
                    state.enemy.gazeObject = null;
                    return true;
                }
            }

            return false;
        }
	}
}