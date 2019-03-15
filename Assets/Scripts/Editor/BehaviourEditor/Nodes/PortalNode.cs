using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SA.BehaviourEditor
{
	[CreateAssetMenu(menuName = "Editor/Nodes/Portal Node")]
	public class PortalNode : DrawNode
	{
		public override void DrawCurve(BaseNode baseNode)
		{
			return;
		}

		public override void DrawWindow(BaseNode baseNode)
		{
			baseNode.stateRef.currentState = (State)EditorGUILayout.ObjectField(baseNode.stateRef.currentState, typeof(State), false);
			baseNode.isAssigned = baseNode.stateRef.currentState != null;

			// If the state has changed
			if (baseNode.stateRef.previousState != baseNode.stateRef.currentState)
			{
				baseNode.stateRef.previousState = baseNode.stateRef.currentState;

				// Save the changes
				BehaviourEditor.forceSetDirty = true;
			}
		}
	}
}
