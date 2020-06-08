using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Actions/Move Towards Sound")]
    public class MoveTowardsSound : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.enemy.MoveToPosition(states.enemy.soundPosition);
        }
    }

}
