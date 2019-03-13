using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA;

namespace SA.BehaviourEditor
{
	[CreateAssetMenu(menuName = "Editor/Nodes/Transition Node")]
	public class TransitionNode : DrawNode
	{
		public void Init(StateNode enterState, Transition transition)
		{

		}

		public override void DrawWindow(BaseNode baseNode)
		{
			BaseNode enterNode = BehaviourEditor.settings.graph.GetNodeWithIndex(baseNode.enterNode);

			Transition transition = enterNode.stateRef.currentState.GetTransition(baseNode.transitionRef.transitionId);

			transition.condition = (Condition)EditorGUILayout.ObjectField(
				transition.condition,
				typeof(Condition),
				false
			);

			if (transition.condition == null)
			{
				EditorGUILayout.LabelField("No condition set!");
			}
			// Allow the user to enable/disable the transition
			else
			{
				if (baseNode.isDuplicate)
				{
					EditorGUILayout.LabelField("Duplicate condition!");
				}
				else
				{
					//					transition.disable = EditorGUILayout.Toggle("Disable", transition.disable);
				}
			}

			if (baseNode.transitionRef.previousCondition != transition.condition)
			{
				baseNode.transitionRef.previousCondition = transition.condition;
				baseNode.isDuplicate = BehaviourEditor.settings.graph.IsTransitionDuplicate(baseNode);

				if (!baseNode.isDuplicate)
				{

				}
			}
		}

		public override void DrawCurve(BaseNode baseNode)
		{
			Rect rect = baseNode.windowRect;
			rect.y += baseNode.windowRect.height * 0.5f;
			rect.width = 1;
			rect.height = 1;

			BaseNode temp = BehaviourEditor.settings.graph.GetNodeWithIndex(baseNode.enterNode);

			if (temp == null)
			{
				BehaviourEditor.settings.graph.RemoveNode(baseNode.id);
			}
			else
			{
				Rect r = temp.windowRect;
				BehaviourEditor.DrawNodeCurve(r, rect, true, Color.cyan);
			}
		}



		/*
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
		*/
	}
}
