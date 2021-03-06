﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Behaviour.BehaviourEditor
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
		public bool isAssigned;

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
		public State currentState;
		[HideInInspector] public State previousState;
		public SerializedObject serializedState;
		public ReorderableList onStateList;
		public ReorderableList onEnterList;
		public ReorderableList onExitList;
	}

	[System.Serializable]
	public class TransitionNodeReferences
	{
		[HideInInspector] public Condition previousCondition;
		public int transitionId;
	}

}