using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Actions/Switch Point")]
    public class SwitchPoint : StateActions
    {
        public override void Execute(StateManager states)
        {
            SetNextPatrolPoint(states.enemy);
        }

        // Finds the next patrol point based on the direction -- changing the direction if that's
        // the only way to the destination
        public void SetNextPatrolPoint(Enemy enemy)
        {
            // Move along the patrol
            int targetIndex = enemy.path.NextPatrolPoint(enemy.nextPatrolPoint, enemy.patrolDirection);

            // Special state for when a route doesn't loop around
            if (!enemy.path.ShouldRouteLoop())
            {
                // Forwards run around check
                if (enemy.nextPatrolPoint > targetIndex && enemy.patrolDirection == PatrolPath.FORWARDS)
                {
                    enemy.patrolDirection = PatrolPath.BACKWARDS;
                    targetIndex = enemy.path.NextPatrolPoint(targetIndex, PatrolPath.BACKWARDS);
                }

                // Backwards run around check
                if (targetIndex > enemy.nextPatrolPoint && enemy.patrolDirection == PatrolPath.BACKWARDS)
                {
                    enemy.patrolDirection = PatrolPath.FORWARDS;
                    targetIndex = enemy.path.NextPatrolPoint(targetIndex, PatrolPath.FORWARDS);
                }
            }

            // Assign new and last patrol point indexes
            enemy.lastPatrolPoint = enemy.nextPatrolPoint;
            enemy.nextPatrolPoint = targetIndex;
        }
    }
}