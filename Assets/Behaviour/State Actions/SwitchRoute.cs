using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Actions/Switch Route")]
    public class SwitchRoute : StateActions
    {
        public override void Execute(StateManager states)
        {
            PatrolPoint startPoint = states.enemy.path.PointAtIndex(states.enemy.nextPatrolPoint);
            PatrolPoint targetPoint = startPoint.ConnectionToPath(states.enemy.targetPath);

            if (targetPoint != null)
            {
                // Index to move to in other path
                int indexOfTargetPoint = targetPoint.Path().IndexOfPoint(targetPoint);

                // Assign new route and starting point
                states.enemy.path = states.enemy.targetPath;
                states.enemy.nextPatrolPoint = indexOfTargetPoint;
                states.enemy.lastPatrolPoint = indexOfTargetPoint;

                // Reset values
                states.enemy.targetPath = null;
            }
        }
    }
}
