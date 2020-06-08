using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Conditions/Should Search")]
    public class ShouldSearch : Condition
    {
        public void OnEnable()
        {
            description = "Should the search for the player?";
        }

        public override bool HasMetCondition(StateManager state)
        {
            if (state.enemy.detectionLevel >= state.enemy.config.searchThreshold)
                return true;

            return false;
        }
    }
}