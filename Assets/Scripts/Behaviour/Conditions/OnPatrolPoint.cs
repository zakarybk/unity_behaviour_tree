using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Conditions/On Patrol Point")]
	public class OnPatrolPoint : Condition
	{
		public void OnEnable()
		{
			description = "Is the enemy on a patrol point?";
		}

		public override bool HasMetCondition(StateManager state)
		{
			int pointIndex = state.enemy.nextPatrolPoint;
			PatrolPoint point = state.enemy.path.PointAtIndex(pointIndex);

			float distanceToPoint = Vector3.Distance(
				state.enemy.transform.position,
				point.transform.position
			);

			return distanceToPoint < state.config.patrolSwitchDistance;
		}
	}
}
