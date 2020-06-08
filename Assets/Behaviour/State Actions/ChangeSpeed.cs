using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Actions/Change Speed")]
    public class ChangeSpeed : StateActions
    {
        public override void Execute(StateManager states)
        {
            float moveSpeed;

            switch (states.currentState.name)
            {
                case "HeardSound":
                case "Searching":
                    moveSpeed = states.enemy.config.searchSpeed;
                    break;
                case "Pursuing":
                    moveSpeed = states.enemy.config.runSpeed;
                    break;
                default:
                    moveSpeed = states.enemy.config.walkSpeed;
                    break;
            }

            states.enemy.config.agent.speed = moveSpeed;
        }
    }
}