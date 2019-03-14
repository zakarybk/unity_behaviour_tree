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
				baseNode.isAssigned = false;
			}
			// Allow the user to enable/disable the transition
			else
			{
				baseNode.isAssigned = true;

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

			BaseNode enter = BehaviourEditor.settings.graph.GetNodeWithIndex(baseNode.enterNode);

			if (enter == null)
			{
				BehaviourEditor.settings.graph.RemoveNode(baseNode.id);
			}
			else
			{
				Color targetColor = Color.green;

				if (!baseNode.isAssigned || baseNode.isDuplicate)
					targetColor = Color.red;

				Rect r = enter.windowRect;
				BehaviourEditor.DrawNodeCurve(r, rect, true, targetColor);
			}

			// Don't draw lines from duplicates
			if (baseNode.isDuplicate)
				return;

			if (baseNode.targetNode > 0) // Zero is default - nothing will be set to it
			{
				BaseNode exitNode = BehaviourEditor.settings.graph.GetNodeWithIndex(baseNode.targetNode);

				if (exitNode == null)
				{
					baseNode.targetNode = -1;
				}
				else
				{
					rect = baseNode.windowRect;
					rect.x += rect.width;
					Rect exitRect = exitNode.windowRect;
					exitRect.x -= exitRect.width * 0.5f;

					Color targetColor = Color.green;
					if (!exitNode.isAssigned || exitNode.isDuplicate)
						targetColor = Color.red;

					BehaviourEditor.DrawNodeCurve(rect, exitRect, false, targetColor);
				}
			}
		}
	}
}
