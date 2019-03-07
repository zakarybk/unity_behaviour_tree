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
	public class StateNode : BaseNode
	{
		public bool collapse;
		bool previousCollapse;

		public bool isDuplicate;

		public State currentState;
		public State previousState;

		SerializedObject serializedState;
		ReorderableList onStateList;
		ReorderableList onEnterList;
		ReorderableList onExitList;

		public List<BaseNode> childNodes = new List<BaseNode>();

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
					//windowRect.height = 300; // Magic number
				}

				collapse = EditorGUILayout.Toggle(" ", collapse);
			}
			else
			{
				EditorGUILayout.LabelField("Add state to modify:");
			}

			currentState = (State)EditorGUILayout.ObjectField(currentState, typeof(State), false);

			if (previousCollapse != collapse)
			{
				previousCollapse = collapse;
//				BehaviourEditor.graph.SetStateNode(this);
			}

			if (previousState != currentState)
			{
				serializedState = null;
				isDuplicate = BehaviourEditor.graph.IsStateNodeDuplicate(this);

				if (!isDuplicate)
				{
					BehaviourEditor.graph.SetStateNode(this);
					previousState = currentState;

					// Add references to the children
					for (int i = 0; i < currentState.transitions.Count; i++)
					{

					}
				}
			}

			if (isDuplicate)
			{
				EditorGUILayout.LabelField("State is a duplicate");
				windowRect.height = 100; // Magic number
				return;
			}

			if (currentState)
			{
				if (serializedState == null)
				{
					serializedState = new SerializedObject(currentState);
					onStateList = new ReorderableList(serializedState, serializedState.FindProperty("onState"), true, true, true, true);
					onEnterList = new ReorderableList(serializedState, serializedState.FindProperty("onEnter"), true, true, true, true);
					onExitList = new ReorderableList(serializedState, serializedState.FindProperty("onExit"), true, true, true, true);
				}

				if (!collapse)
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
					float scaledHeight = defaultHeight + onStateList.count * 20 + onEnterList.count * 20 + onExitList.count * 20; // Magic number
					windowRect.height = scaledHeight;
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

		public override void DrawCurve()
		{
			base.DrawCurve();
		}

		public Transition AddTransition()
		{
			return currentState.AddTransition();
		}

		public void ClearReferences()
		{
			BehaviourEditor.ClearWindowsFromList(childNodes);
			childNodes.Clear();
		}
	}
}
