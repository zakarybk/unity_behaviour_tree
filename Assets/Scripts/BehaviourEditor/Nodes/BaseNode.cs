using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SA.BehaviourEditor
{
	[System.Serializable]
	public class BaseNode
	{
		public int id;
		public DrawNode drawNode;
		public Rect windowRect;
		public string windowTitle;
		public int enterNode;
		public int targetNode;
		public bool isDuplicate;
		public string comment;

		public bool collapse;
		[HideInInspector] public bool previousCollapse;

		[SerializeField] public StateNodeReferences stateRef;
		[SerializeField] public TransitionNodeReferences transitionRef;

		// Draw the node window
		public virtual void DrawWindow(BaseNode baseNode)
		{
			if (drawNode != null)
			{
				drawNode.DrawWindow(this);
			}
		}

		// Draw the curves between node windows
		public virtual void DrawCurve(BaseNode baseNode)
		{
			if (drawNode != null)
			{
				drawNode.DrawCurve(this);
			}
		}

	}

	[System.Serializable]
	public class StateNodeReferences
	{
		[HideInInspector] public State currentState;
		[HideInInspector] public State previousState;
	}

	[System.Serializable]
	public class TransitionNodeReferences
	{
		[HideInInspector] public Condition previousCondition;
		[HideInInspector] public int transitionId;
	}

}