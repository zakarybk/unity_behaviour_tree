using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	[CreateAssetMenu(menuName = "Behaviour/State")]
	public class State : ScriptableObject
	{
		public StateActions[] onState;
		public StateActions[] onEnter;
		public StateActions[] onExit;

		public int idCount;
		[SerializeField]
		public List<Transition> transitions = new List<Transition>();

		public void OnEnter(StateManager stateManager)
		{
			ExecuteActions(stateManager, onEnter);
		}

		public void FixedTick(StateManager stateManager)
		{
			ExecuteActions(stateManager, onState);
		}

		public void Tick(StateManager stateManager)
		{
			ExecuteActions(stateManager, onState);
			UpdateState(stateManager);
		}

		public void OnExit(StateManager stateManager)
		{
			ExecuteActions(stateManager, onExit);
		}

		public void ExecuteActions(StateManager stateManager, StateActions[] actions)
		{
			for (int i = 0; i < actions.Length; i++)
			{
				if (actions[i])
					actions[i].Execute(stateManager);
			}
		}

		// Update the state if conditions are met
		public void UpdateState(StateManager stateManager)
		{
			for (int i = 0; i < transitions.Count; i++)
			{
				if (transitions[i].disable)
					continue;

				if (transitions[i].condition != null && transitions[i].condition.HasMetCondition(stateManager))
				{
					if (transitions[i].targetState != null)
					{
						// Set the new state
						stateManager.currentState = transitions[i].targetState;

						// Run exit state of the current state
						OnExit(stateManager);

						// Run the enter state of the new state
						stateManager.currentState.OnEnter(stateManager);
					}
					return;
				}
			}
		}

		public Transition AddTransition()
		{
			Transition retVal = new Transition(); // retval return val
			transitions.Add(retVal);
			retVal.id = idCount;
			idCount++;
			return retVal;
		}

		public void RemoveTransition(int id)
		{
			Transition toRemove = GetTransition(id);

			if (toRemove != null)
			{
				transitions.Remove(toRemove);
			}
		}

		public Transition GetTransition(int id)
		{
			for (int i = 0; i < transitions.Count; i++)
			{
				if (transitions[i].id == id)
					return transitions[i];
			}

			return null;
		}

	}
}
