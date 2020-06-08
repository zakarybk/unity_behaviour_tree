using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Actions/Move Towards Patrol Point")]
    public class MoveTowardsPatrolPoint : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.enemy.MoveToPatrolPoint(states.enemy.nextPatrolPoint);
        }
    }

}