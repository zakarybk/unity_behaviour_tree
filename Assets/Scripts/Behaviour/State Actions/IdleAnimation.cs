using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Actions/Idle Animation")]
    public class IdleAnimation : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.enemy.animator.SetBool("isWalking", false);
            states.enemy.animator.SetBool("isRunning", false);
        }
    }
}