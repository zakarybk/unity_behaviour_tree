using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour
{
	public class StateManager : MonoBehaviour
	{
		// State
		public State currentState;
		[HideInInspector] public EnemyConfig config;

		[HideInInspector] public Enemy enemy;
		[HideInInspector] public Player player;

		private void Update()
		{
			if (currentState != null)
			{
				currentState.Tick(this);
			}
		}

		private void Awake()
		{
			// References
			player = FindObjectOfType<Player>();
			enemy = GetComponent<Enemy>();
			config = GetComponent<EnemyConfig>();

			// Set the detection radius of the sphere collider
			GetComponent<SphereCollider>().radius = config.slowDetectionRadius;
		}

        // Not yet implimented
        // Will allow an enemy to calculate distance to a patrol
        // point through linked paths
		public int DistanceToPatrolPoint()
		{
			return 1;
		}
	}
}
