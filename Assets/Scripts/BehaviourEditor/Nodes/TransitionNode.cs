using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA;

namespace SA.BehaviourEditor
{
	public class TransitionNode : BaseNode
	{
		public Transition targetTransition;
		public StateNode enterState;
		public StateNode targetState;

		public void Init(StateNode enterState, Transition transition)
		{
			this.enterState = enterState;
			targetTransition = transition;
		}

		public override void DrawWindow()
		{
			if (targetTransition != null)
			{
				// Directly editing public var
				// Setup the condition
				targetTransition.condition = (Condition)EditorGUILayout.ObjectField(
					targetTransition.condition,
					typeof(Condition),
					false
				);

				// No condition - warn the user
				if (targetTransition.condition == null)
				{
					EditorGUILayout.LabelField("No condition set!");
				}
				// Allow the user to enable/disable the transition
				else
				{
					targetTransition.disable = EditorGUILayout.Toggle("Disable", targetTransition.disable);
				}

			}
		}

		public override void DrawCurve()
		{
			if (enterState)
			{
				Rect rect = windowRect;
				rect.y += windowRect.height * 0.5f;
				rect.width = 1;
				rect.height = 1;

				BehaviourEditor.DrawNodeCurve(enterState.windowRect, rect, true, Color.cyan);
			}
		}

	}
}
