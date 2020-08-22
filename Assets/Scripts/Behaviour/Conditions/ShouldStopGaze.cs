using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Conditions/Should Stop Gaze")]
    public class ShouldStopGaze : Condition
    {
        public void OnEnable()
        {
            description = "Should the enemy stop gazing?";
        }

        public override bool HasMetCondition(StateManager state)
        {
            // If gaze time up up!
            if (state.enemy.gazeEndTime <= Time.time)
            {
                // Get back to work
                return true;
            }

            return false;
        }
    }
}