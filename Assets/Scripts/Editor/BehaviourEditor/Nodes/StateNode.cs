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
			if (baseNode.stateRef.currentState != null)
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
				baseNode.stateRef.previousState = baseNode.stateRef.currentState;

				if (!baseNode.isDuplicate)
				{
					Vector3 pos = new Vector3(baseNode.windowRect.x, baseNode.windowRect.y, 0);
					pos.x += baseNode.windowRect.width * 2;

					SetupReorderableLists(baseNode);

					// Load transitions
					for (int i = 0; i < baseNode.stateRef.currentState.transitions.Count; i++)
					{
						pos.y += i * 100;
						BehaviourEditor.AddTransitionNodeFromTransition(baseNode.stateRef.currentState.transitions[i], baseNode, pos);
					}
				}
			}

			if (baseNode.isDuplicate)
			{
				EditorGUILayout.LabelField("State is a duplicate");
				baseNode.windowRect.height = 100; // Magic number
				return;
			}

			if (baseNode.stateRef.currentState != null)
			{
				baseNode.isAssigned = true;

				if (!baseNode.collapse)
				{
					if (baseNode.stateRef.serializedState == null)
						SetupReorderableLists(baseNode);

					baseNode.stateRef.serializedState.Update();

					// On State
					EditorGUILayout.LabelField(""); // Spacing
					baseNode.stateRef.onStateList.DoLayoutList();

					// On Enter
					EditorGUILayout.LabelField(""); // Spacing
					baseNode.stateRef.onEnterList.DoLayoutList();

					// On Exit
					EditorGUILayout.LabelField(""); // Spacing
					baseNode.stateRef.onExitList.DoLayoutList();

					baseNode.stateRef.serializedState.ApplyModifiedProperties();

					// Resize the window as items get added
					float defaultHeight = 300.0f; // Magic number
					float scaledHeight = defaultHeight + (baseNode.stateRef.onStateList.count + baseNode.stateRef.onEnterList.count + baseNode.stateRef.onExitList.count) * 20; // Magic number
					baseNode.windowRect.height = scaledHeight;
				}
			}
			else
			{
				baseNode.isAssigned = false; // Said true - probably meant false
			}
		}

		void SetupReorderableLists(BaseNode baseNode)
		{
			baseNode.stateRef.serializedState = new SerializedObject(baseNode.stateRef.currentState);

			baseNode.stateRef.onStateList = new ReorderableList(baseNode.stateRef.serializedState, baseNode.stateRef.serializedState.FindProperty("onState"), true, true, true, true);
			baseNode.stateRef.onEnterList = new ReorderableList(baseNode.stateRef.serializedState, baseNode.stateRef.serializedState.FindProperty("onEnter"), true, true, true, true);
			baseNode.stateRef.onExitList = new ReorderableList(baseNode.stateRef.serializedState, baseNode.stateRef.serializedState.FindProperty("onExit"), true, true, true, true);

			HandleReorderableList(baseNode.stateRef.onStateList, "On State");
			HandleReorderableList(baseNode.stateRef.onEnterList, "On Enter");
			HandleReorderableList(baseNode.stateRef.onExitList, "On Exit");
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
