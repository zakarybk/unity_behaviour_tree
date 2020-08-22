using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Conditions/Should Switch Point")]
    public class ShouldSwitchPoint : Condition
    {
        public void OnEnable()
        {
            description = "Should the enemy switch points?";
        }

        public override bool HasMetCondition(StateManager state)
        {
            // If at the patrol point
            if (state.enemy.path.IsOnPoint(state.enemy, state.enemy.nextPatrolPoint))
            {
                // Switch points
                return true;
            }

            return false;
        }
    }
}