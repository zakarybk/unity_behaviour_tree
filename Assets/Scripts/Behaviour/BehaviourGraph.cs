using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.BehaviourEditor;

namespace SA {
	[CreateAssetMenu]
	public class BehaviourGraph : ScriptableObject
	{
		public List<Saved_StateNode> saved_StateNodes = new List<Saved_StateNode>();
		Dictionary<StateNode, Saved_StateNode> stateNodesDict = new Dictionary<StateNode, Saved_StateNode>();
		Dictionary<State, StateNode> stateDict = new Dictionary<State, StateNode>();

		public void Init()
		{
			// Clear data on init
			stateNodesDict.Clear();
			stateDict.Clear();
		}

		public void SetNode(BaseNode node)
		{
			if (node is StateNode)
			{
				SetStateNode((StateNode)node);
			}
			else if (node is TransitionNode)
			{
				SetTransitionNode((TransitionNode)node);
			}
			else if (node is CommentNode)
			{
				//
			}
		}

		public bool IsStateNodeDuplicate(StateNode node)
		{
			bool isDuplicate = false;

			// Make sure no other node contains the same state - return if true
			StateNode previousNode = null;
			stateDict.TryGetValue(node.currentState, out previousNode);
			if (previousNode != null)
				isDuplicate = true;

			return isDuplicate;
		}

		void SetStateNode(StateNode node)
		{
			if (node.isDuplicate)
				return;

			// Cleanup previous state
			if (node.previousState != null)
			{
				stateDict.Remove(node.previousState);
			}

			// No state - pass/return
			if (node.currentState == null)
			{
				return;
			}

			Saved_StateNode saved = GetSavedState(node);

			// If no saved state node exists - create one
			if (saved == null)
			{
				saved = new Saved_StateNode();
				saved_StateNodes.Add(saved);
				stateNodesDict.Add(node, saved);
			}

			saved.state = node.currentState;
			saved.position = new Vector2(node.windowRect.x, node.windowRect.y);
			saved.isCollapsed = node.collapse;

			stateDict.Add(saved.state, node);
		}

		void ClearStateNode(StateNode node)
		{
			Saved_StateNode saved = GetSavedState(node);

			// If saved data exists - remove it
			if (saved != null)
			{
				saved_StateNodes.Remove(saved);
				stateNodesDict.Remove(node);
			}
		}

		// Get the saved data for the state node
		Saved_StateNode GetSavedState(StateNode node)
		{
			Saved_StateNode r = null;
			stateNodesDict.TryGetValue(node, out r);
			return r;
		}

		// Get the state node for the state 
		public StateNode GetStateNode(State state)
		{
			StateNode r = null;
			stateDict.TryGetValue(state, out r);
			return r;
		}

		#region Transition Nodes

		public bool IsTransitionDuplicate(TransitionNode node)
		{
			bool isDuplicate = false;

			Saved_StateNode savedStateNode = GetSavedState(node.enterState);
			isDuplicate = savedStateNode.IsTransitionDuplicate(node);

			return isDuplicate;
		}

		void SetTransitionNode(TransitionNode node)
		{
			Saved_StateNode savedStateNode = GetSavedState(node.enterState);
			savedStateNode.SetTransitionNode(node);
		}

		#endregion

	}

	[System.Serializable]
	public class Saved_StateNode
	{
		public State state;
		public Vector2 position;
		public bool isCollapsed;

		public List<Saved_Conditions> savedConditions = new List<Saved_Conditions>();
		Dictionary<TransitionNode, Saved_Conditions> savedTransitionsDict = new Dictionary<TransitionNode, Saved_Conditions>();
		Dictionary<Condition, TransitionNode> conditionsDict = new Dictionary<Condition, TransitionNode>();

		public void Init()
		{
			savedTransitionsDict.Clear();
			conditionsDict.Clear();
		}

		public bool IsTransitionDuplicate(TransitionNode node)
		{
			bool isDuplicate = false;

			// Make sure no other node contains the same state - return if true
			TransitionNode previousNode = null;
			conditionsDict.TryGetValue(node.targetCondition, out previousNode);
			if (previousNode != null)
				isDuplicate = true;

			return isDuplicate;
		}

		public void SetTransitionNode(TransitionNode node)
		{
			// Don't add duplicates
			if (node.isDuplicate)
				return;

			if (node.previousCondition != null)
			{
				conditionsDict.Remove(node.targetCondition);
			}

			// Don't add if target has no transition condition
			if (node.targetCondition == null)
			{
				return;
			}

			// Load the saved condition
			Saved_Conditions savedCondition = GetSavedCondition(node);
			if (savedCondition == null)
			{
				savedCondition = new Saved_Conditions();
				savedConditions.Add(savedCondition);
				savedTransitionsDict.Add(node, savedCondition);
				node.transition = node.enterState.currentState.AddTransition();
			}

			// Apply the saved condition and transition
			savedCondition.transition = node.transition;
			savedCondition.condition = node.targetCondition;
			savedCondition.transition.condition = node.targetCondition;
			savedCondition.position = new Vector2(node.windowRect.x, node.windowRect.y);
			conditionsDict.Add(savedCondition.condition, node);
		}


		Saved_Conditions GetSavedCondition(TransitionNode node)
		{
			Saved_Conditions r = null;
			savedTransitionsDict.TryGetValue(node, out r);
			return r;
		}


		TransitionNode GetTransitionNode(Transition transition)
		{
			TransitionNode r = null;
			conditionsDict.TryGetValue(transition.condition, out r);
			return r;
		}

	}

	[System.Serializable]
	public class Saved_Conditions
	{
		public Transition transition;
		public Condition condition;
		public Vector2 position;
	}
}

