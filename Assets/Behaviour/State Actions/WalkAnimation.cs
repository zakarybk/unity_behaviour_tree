using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Actions/Walk Animation")]
    public class WalkAnimation : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.enemy.animator.SetBool("isWalking", true);
            states.enemy.animator.SetBool("isRunning", false);
        }
    }
}