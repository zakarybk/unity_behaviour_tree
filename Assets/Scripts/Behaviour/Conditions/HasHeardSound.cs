using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Conditions/Has Heard Sound")]
    public class HasHeardSound : Condition
    {
        public void OnEnable()
        {
            description = "Has the enemy heard a sound?";
        }

        public override bool HasMetCondition(StateManager state)
        {
            return state.enemy.HasHeardSound();
        }
    }
}