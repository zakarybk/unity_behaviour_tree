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
				//
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

		public void SetStateNode(StateNode node)
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

		public void ClearStateNode(StateNode node)
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

	}

	[System.Serializable]
	public class Saved_StateNode
	{
		public State state;
		public Vector2 position;
		public bool isCollapsed;

	}

	[System.Serializable]
	public class Saved_Transition
	{

	}
}

