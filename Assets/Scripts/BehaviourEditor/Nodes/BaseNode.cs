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
		public DrawNode drawNode;

		public Rect windowRect;
		[HideInInspector] public string windowTitle;

		public StateNodeReferences stateRef;

		// Draw the node window
		public void	DrawWindow()
		{
			if (drawNode != null)
			{
				drawNode.DrawWindow(this);
			}
		}

		// Draw the curves between node windows
		public void DrawCurve()
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
		[HideInInspector] public bool collapse;
		[HideInInspector] public bool previousCollapse;

		[HideInInspector] public bool isDuplicate;

		[HideInInspector] public State currentState;

		[HideInInspector] public State previousState;

		[HideInInspector] public SerializedObject serializedState;
		[HideInInspector] public ReorderableList onStateList;
		[HideInInspector] public ReorderableList onEnterList;
		[HideInInspector] public ReorderableList onExitList;
	}
}