using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.BehaviourEditor;

namespace SA
{
	[CreateAssetMenu]
	public class BehaviourGraph : ScriptableObject
	{
		[SerializeField]
		public List<BaseNode> windows = new List<BaseNode>();
		[SerializeField]
		public int idCount;
		List<int> indexToDelete = new List<int>();

		#region Checkers
		public BaseNode GetNodeWithIndex(int index)
		{
			for (int i = 0; i < windows.Count; i++)
			{
				if (windows[i].id == index)
					return windows[i];
			}
			return null;
		}

		public void DeleteWindowsThatNeedTo()
		{
			for (int i = 0; i < indexToDelete.Count; i++)
			{
				BaseNode baseNode = GetNodeWithIndex(indexToDelete[i]);
				if (baseNode != null)
					windows.Remove(baseNode);
			}
		}

		public void RemoveNode(int index)
		{
			indexToDelete.Add(index);
		}

		public bool IsStateDuplicate(BaseNode baseNode)
		{
			for (int i = 0; i < windows.Count; i++)
			{
				if (windows[i].id == baseNode.id)
					continue;

				if (windows[i].stateRef.currentState == baseNode.stateRef.currentState && !windows[i].isDuplicate)
					return true;
			}

			return false;
		}

		public bool IsTransitionDuplicate(BaseNode baseNode)
		{
			BaseNode enter = GetNodeWithIndex(baseNode.enterNode);

			// Don't delete self
			if (enter == null)
				return false;

			for (int i = 0; i < enter.stateRef.currentState.transitions.Count; i++)
			{
				Transition transition = enter.stateRef.currentState.transitions[i];
				if (transition.condition == baseNode.transitionRef.previousCondition && transition.id != baseNode.transitionRef.transitionId)
					return true;
			}

			return false;
		}
		#endregion
	}

}

