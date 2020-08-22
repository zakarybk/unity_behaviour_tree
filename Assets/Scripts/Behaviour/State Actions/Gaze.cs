using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
    [CreateAssetMenu(menuName = "Behaviour/Actions/Gaze")]
    public class Gaze : StateActions
    {
        // Time it takes to start/stop looking at something
        private float cumulativeLooking;
        private Quaternion lookStartRotation;
        public float transitionLookSpeed = 1.0f;

        public override void Execute(StateManager states)
        {
            // If no gaze object then get one
            if(states.enemy.gazeObject == null)
            {
                PatrolPoint point = states.enemy.PatrolPoint();

                // Set the random gaze object
                int index = Random.Range(0, point.gazePoints.Length);
                states.enemy.gazeObject = point.gazePoints[index].gameObject;

                // Set the random gaze time
                states.enemy.gazeEndTime = Time.time + point.GazeTime();

                lookStartRotation = states.enemy.transform.rotation;
                cumulativeLooking = 0.0f;
            }

            // Look towards gaze object
            if (cumulativeLooking < 1.0f)
            {
                Quaternion toInteraction = Quaternion.LookRotation(states.enemy.gazeObject.transform.position - states.enemy.gameObject.transform.position);

                Quaternion quaternion = Quaternion.Slerp(lookStartRotation, toInteraction, cumulativeLooking);
                quaternion.x = states.enemy.transform.rotation.x;
                quaternion.z = states.enemy.transform.rotation.z;

                states.enemy.transform.rotation = quaternion;
                cumulativeLooking = Mathf.Clamp(cumulativeLooking + transitionLookSpeed * Time.deltaTime, 0, 1);
            }
        }

    }
}
