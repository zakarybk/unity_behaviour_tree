using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Conditions/Has Reached Sound Source")]
    public class HasReachedSoundSource : Condition
    {
        public void OnEnable()
        {
            description = "Has the enemy reached the sound?";
        }

        public override bool HasMetCondition(StateManager state)
        {
            float soundDistance = Vector3.Distance(
                state.enemy.transform.position, 
                state.enemy.soundPosition
            );

            if (state.enemy.IsPathPossible(state.enemy.soundPosition) && 
                soundDistance < state.enemy.config.patrolSwitchDistance)
            {
                return true;
            }

            return false;
        }
    }
}
