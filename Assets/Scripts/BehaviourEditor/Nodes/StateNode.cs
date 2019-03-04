using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA;

namespace SA.BehaviourEditor
{
	public class StateNode : BaseNode
	{
		bool collapse;
		public State currentState;
		State previousState;

		List<BaseNode> childNodes = new List<BaseNode>();

		public override void DrawWindow()
		{
			if (currentState)
			{
				if (collapse)
				{
					windowRect.height = 100; // Magic number
				}
				else
				{
					windowRect.height = 300; // Magic number
				}

				collapse = EditorGUILayout.Toggle(" ", collapse);
			}
			else
			{
				EditorGUILayout.LabelField("Add state to modify:");
			}

			currentState = (State)EditorGUILayout.ObjectField(currentState, typeof(State), false);

			if (previousState != currentState)
			{
				previousState = currentState;
				ClearReferences();

				// Add references to the children
				for (int i = 0; i < currentState.transitions.Count; i++)
				{
					childNodes.Add(BehaviourEditor.AddTransitionNode(i, currentState.transitions[i], this));
				}
			}

			if (currentState)
			{
				// coming back
			}
		}

		public override void DrawCurve()
		{
			base.DrawCurve();
		}

		public Transition AddTransition()
		{
			return currentState.AddTransition();
		}

		public void ClearReferences()
		{
			BehaviourEditor.ClearWindowsFromList(childNodes);
			childNodes.Clear();
		}
	}
}
