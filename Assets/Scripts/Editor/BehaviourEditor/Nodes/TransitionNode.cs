using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA;

namespace SA.BehaviourEditor
{
	[CreateAssetMenu(menuName = "Behaviour/Editor/Nodes/Transition Node")]
	public class TransitionNode : DrawNode
	{
		public void Init(StateNode enterState, Transition transition)
		{

		}

		public override void DrawWindow(BaseNode baseNode)
		{
			// Enter node
			BaseNode enterNode = BehaviourEditor.settings.graph.GetNodeWithIndex(baseNode.enterNode);

			if (enterNode == null)
				return;

			if (enterNode.stateRef.currentState == null)
			{
				BehaviourEditor.settings.graph.RemoveNode(baseNode.id);
				return;
			}

			// Transition
			Transition transition = enterNode.stateRef.currentState.GetTransition(baseNode.transitionRef.transitionId);

			if (transition == null)
				return;

			// Transition condition
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
					GUILayout.Label(transition.condition.description);

					BaseNode targetNode = BehaviourEditor.settings.graph.GetNodeWithIndex(baseNode.targetNode);

					// Setup targetNode reference
					if (targetNode != null)
					{
						if (targetNode.isDuplicate)
							transition.targetState = null;
						else
							transition.targetState = targetNode.stateRef.currentState;
					}
					else
					{
						transition.targetState = null;
					}
				}
			}

			if (baseNode.transitionRef.previousCondition != transition.condition)
			{
				baseNode.transitionRef.previousCondition = transition.condition;
				baseNode.isDuplicate = BehaviourEditor.settings.graph.IsTransitionDuplicate(baseNode);

				if (!baseNode.isDuplicate)
				{
					// Save the changes
					BehaviourEditor.forceSetDirty = true;
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

					if (exitNode.drawNode is StateNode)
					{ 
						if (!exitNode.isAssigned || exitNode.isDuplicate)
							targetColor = Color.red;
					}
					else
					{
						if (!exitNode.isAssigned)
							targetColor = Color.red;
						else
							targetColor = Color.cyan;
					}

					BehaviourEditor.DrawNodeCurve(rect, exitRect, false, targetColor);
				}
			}
		}
	}
}
