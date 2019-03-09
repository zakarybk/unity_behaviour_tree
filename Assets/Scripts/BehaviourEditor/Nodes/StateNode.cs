using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditorInternal;
using System;
using System.IO;
using SA;

namespace SA.BehaviourEditor
{
	[CreateAssetMenu(menuName = "Editor/Nodes/State Node")]
	public class StateNode : DrawNode
	{

		public override void DrawWindow(BaseNode baseNode)
		{
			if (baseNode.stateRef.currentState)
			{
				if (baseNode.stateRef.collapse)
				{
					baseNode.windowRect.height = 100; // Magic number
				}
				else
				{
					//windowRect.height = 300; // Magic number
				}

				baseNode.stateRef.collapse = EditorGUILayout.Toggle(" ", baseNode.stateRef.collapse);
			}
			else
			{
				EditorGUILayout.LabelField("Add state to modify:");
			}

			baseNode.stateRef.currentState = (State)EditorGUILayout.ObjectField(baseNode.stateRef.currentState, typeof(State), false);

			if (baseNode.stateRef.previousCollapse != baseNode.stateRef.collapse)
			{
				baseNode.stateRef.previousCollapse = baseNode.stateRef.collapse;
//				BehaviourEditor.graph.SetStateNode(this);
			}

			if (baseNode.stateRef.previousState != baseNode.stateRef.currentState)
			{
				baseNode.stateRef.serializedState = null;
				baseNode.stateRef.isDuplicate = BehaviourEditor.settings.graph.IsStateNodeDuplicate(this);

				if (!baseNode.stateRef.isDuplicate)
				{
//					BehaviourEditor.graph.SetNode(this);
					baseNode.stateRef.previousState = baseNode.stateRef.currentState;

					// Add references to the children
					for (int i = 0; i < baseNode.stateRef.currentState.transitions.Count; i++)
					{

					}
				}
			}

			if (baseNode.stateRef.isDuplicate)
			{
				EditorGUILayout.LabelField("State is a duplicate");
				baseNode.windowRect.height = 100; // Magic number
				return;
			}

			if (baseNode.stateRef.currentState)
			{
				if (baseNode.stateRef.serializedState == null)
				{
					baseNode.stateRef.serializedState = new SerializedObject(baseNode.stateRef.currentState);
					baseNode.stateRef.onStateList = new ReorderableList(baseNode.stateRef.serializedState, baseNode.stateRef.serializedState.FindProperty("onState"), true, true, true, true);
					baseNode.stateRef.onEnterList = new ReorderableList(baseNode.stateRef.serializedState, baseNode.stateRef.serializedState.FindProperty("onEnter"), true, true, true, true);
					baseNode.stateRef.onExitList = new ReorderableList(baseNode.stateRef.serializedState, baseNode.stateRef.serializedState.FindProperty("onExit"), true, true, true, true);
				}

				if (!baseNode.stateRef.collapse)
				{
					baseNode.stateRef.serializedState.Update();
					HandleReorderableList(baseNode.stateRef.onStateList, "On State");
					HandleReorderableList(baseNode.stateRef.onEnterList, "On Enter");
					HandleReorderableList(baseNode.stateRef.onExitList, "On Exit");

					EditorGUILayout.LabelField(""); // Spacing
					baseNode.stateRef.onStateList.DoLayoutList();
					EditorGUILayout.LabelField(""); // Spacing
					baseNode.stateRef.onEnterList.DoLayoutList();
					EditorGUILayout.LabelField(""); // Spacing
					baseNode.stateRef.onExitList.DoLayoutList();

					baseNode.stateRef.serializedState.ApplyModifiedProperties();

					// Resize the window as items get added
					float defaultHeight = 300.0f; // Magic number
					float scaledHeight = defaultHeight + baseNode.stateRef.onStateList.count * 20 + baseNode.stateRef.onEnterList.count * 20 + baseNode.stateRef.onExitList.count * 20; // Magic number
					baseNode.windowRect.height = scaledHeight;
				}
			}
		}

		void HandleReorderableList(ReorderableList list, string targetName)
		{
			// lambda function (anonymous delegate)
			list.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, targetName);
			};

			list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = list.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};
		}

		public override void DrawCurve(BaseNode baseNode)
		{
			
		}

		public Transition AddTransition()
		{
			return null;// currentState.AddTransition();
		}

		public void ClearReferences()
		{
//			BehaviourEditor.ClearWindowsFromList(childNodes);
//			childNodes.Clear();
		}
	}
}
