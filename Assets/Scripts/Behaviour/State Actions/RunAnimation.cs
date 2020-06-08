using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Actions/Run Animation")]
    public class RunAnimation : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.enemy.animator.SetBool("isWalking", false);
            states.enemy.animator.SetBool("isRunning", true);
        }
    }
}