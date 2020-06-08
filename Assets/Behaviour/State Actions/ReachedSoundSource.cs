using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Actions/Reached Sound Source")]
    public class ReachedSoundSource : StateActions
    {
        public override void Execute(StateManager states)
        {
            states.enemy.ClearSound();
        }
    }
}
