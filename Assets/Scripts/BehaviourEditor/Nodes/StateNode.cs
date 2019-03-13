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
				if (baseNode.collapse)
				{
					baseNode.windowRect.height = 100; // Magic number
				}
				else
				{
					//windowRect.height = 300; // Magic number
				}

				baseNode.collapse = EditorGUILayout.Toggle(" ", baseNode.collapse);
			}
			else
			{
				EditorGUILayout.LabelField("Add state to modify:");
			}

			baseNode.stateRef.currentState = (State)EditorGUILayout.ObjectField(baseNode.stateRef.currentState, typeof(State), false);

			if (baseNode.previousCollapse != baseNode.collapse)
			{
				baseNode.previousCollapse = baseNode.collapse;
			}

			if (baseNode.stateRef.previousState != baseNode.stateRef.currentState)
			{
				//baseNode.serializedState = null;
				baseNode.isDuplicate = BehaviourEditor.settings.graph.IsStateDuplicate(baseNode);
			}

			if (baseNode.isDuplicate)
			{
				EditorGUILayout.LabelField("State is a duplicate");
				baseNode.windowRect.height = 100; // Magic number
				return;
			}

			if (baseNode.stateRef.currentState != null)
			{
				SerializedObject serializedState = new SerializedObject(baseNode.stateRef.currentState);

				ReorderableList onStateList;
				ReorderableList onEnterList;
				ReorderableList onExitList;

				onStateList = new ReorderableList(serializedState, serializedState.FindProperty("onState"), true, true, true, true);
				onEnterList = new ReorderableList(serializedState, serializedState.FindProperty("onEnter"), true, true, true, true);
				onExitList = new ReorderableList(serializedState, serializedState.FindProperty("onExit"), true, true, true, true);

				if (!baseNode.collapse)
				{
					serializedState.Update();
					HandleReorderableList(onStateList, "On State");
					HandleReorderableList(onEnterList, "On Enter");
					HandleReorderableList(onExitList, "On Exit");

					EditorGUILayout.LabelField(""); // Spacing
					onStateList.DoLayoutList();
					EditorGUILayout.LabelField(""); // Spacing
					onEnterList.DoLayoutList();
					EditorGUILayout.LabelField(""); // Spacing
					onExitList.DoLayoutList();

					serializedState.ApplyModifiedProperties();

					// Resize the window as items get added
					float defaultHeight = 300.0f; // Magic number
					float scaledHeight = defaultHeight + (onStateList.count + onEnterList.count + onExitList.count) * 20; // Magic number
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

		public Transition AddTransition(BaseNode baseNode)
		{
			return baseNode.stateRef.currentState.AddTransition();
		}

		public void ClearReferences()
		{
//			BehaviourEditor.ClearWindowsFromList(childNodes);
//			childNodes.Clear();
		}
	}
}
