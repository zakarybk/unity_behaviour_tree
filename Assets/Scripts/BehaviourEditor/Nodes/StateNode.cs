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
		}

		public override void DrawCurve()
		{
			base.DrawCurve();
		}
	}
}
