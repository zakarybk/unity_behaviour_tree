using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA;

namespace SA.BehaviourEditor
{
	public class TransitionNode : BaseNode
	{
		public bool isDuplicate;
		public Condition targetCondition;
		public Condition previousCondition;
		public Transition transition;

		public StateNode enterState;
		public StateNode targetState;

		public void Init(StateNode enterState, Transition transition)
		{
			this.enterState = enterState;
		}

		public override void DrawWindow()
		{
			// Directly editing public var
			// Setup the condition
			targetCondition = (Condition)EditorGUILayout.ObjectField(
				targetCondition,
				typeof(Condition),
				false
			);

			// No condition - warn the user
			if (targetCondition == null)
			{
				EditorGUILayout.LabelField("No condition set!");
			}
			// Allow the user to enable/disable the transition
			else
			{
				if (isDuplicate)
				{
					EditorGUILayout.LabelField("Duplicate condition!");
				}
				else
				{
//					transition.disable = EditorGUILayout.Toggle("Disable", transition.disable);
				}
			}

			if (previousCondition != targetCondition)
			{
				isDuplicate = BehaviourEditor.graph.IsTransitionDuplicate(this);
				if (!isDuplicate)
				{
					BehaviourEditor.graph.SetNode(this);
				}
				previousCondition = targetCondition;
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
