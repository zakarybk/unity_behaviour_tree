using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Behaviour
{
	[CreateAssetMenu(menuName = "Behaviour/Actions/Update Detection")]
	public class UpdateDetection : StateActions
	{ 
		public override void Execute(StateManager states)
		{
			// Update the direction to the player
			states.enemy.UpdateDirectionToPlayer();

			// If the player can be seen - update detection
			UpdateDetectionLevels(states.enemy, states.player);
		}


		private void UpdateDetectionLevels(Enemy enemy, Player player)
		{
			bool canSeePlayer = enemy.CanSeePlayer();
			float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.transform.position);

			// Modify detectionLevel based on distance and time since last seen
			if (canSeePlayer)
			{
                enemy.detectionLevel += enemy.config.detectionGainAmount * DistanceModifierCalculator(distanceToPlayer, enemy.config.distanceModifier) * Time.deltaTime;

                // isPlayerDetected based on distance and detection levels
                if (distanceToPlayer < enemy.config.fastDetectionRadius)
                {
                    enemy.detectionLevel = 100f;
                    enemy.isPlayerDetected = true;
                }
            }
			else
			{
                enemy.detectionLevel -= enemy.config.detectionLossAmount * Time.deltaTime;
			}

            enemy.detectionLevel = Mathf.Clamp(enemy.detectionLevel, 0, 100);

            // isPlayerDetected based on distance and detection levels
            if (distanceToPlayer < enemy.config.surroundingDistance)
			{
                enemy.detectionLevel = 100f;
				enemy.isPlayerDetected = true;
			}
			else if (enemy.detectionLevel > 99f)
			{
				enemy.isPlayerDetected = true;
			}
			else if (enemy.detectionLevel < 25)
			{
				enemy.isPlayerDetected = false;
			}
		}

		// A Josh function
		float DistanceModifierCalculator(float distance, float distanceModifier)
		{
			float testVar = (distanceModifier / (distance + 10));
			return testVar;
		}
	}

}