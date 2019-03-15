using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using SA;

namespace SA.BehaviourCustomUI
{
	[CustomEditor(typeof(State))]
	public class StateGUI : Editor
	{
		SerializedObject serializedState;
		ReorderableList onFixedList;
		ReorderableList onUpdateList;
		ReorderableList onEnterList;
		ReorderableList onExitList;
		ReorderableList transitions;

		bool showDefaultGUI = false;
		bool showActions = true;
		bool showTransitions = true;

		private void OnEnable()
		{
			serializedState = null;
		}

		public override void OnInspectorGUI()
		{
			showDefaultGUI = EditorGUILayout.Toggle("Default GUI", showDefaultGUI);

			// Return original GUI if desired
			if (showDefaultGUI)
			{
				base.OnInspectorGUI();
				return;
			}

			showActions = EditorGUILayout.Toggle("Show actions", showActions);

			if (serializedState == null)
				SetupReorderableLists();

			serializedState.Update();

			if (showActions)
			{
				//EditorGUILayout.LabelField("Actions that execute on fixed update");
				//onFixedList.DoLayoutList();
				EditorGUILayout.LabelField("Actions that execute on update");
				onUpdateList.DoLayoutList();
				EditorGUILayout.LabelField("Actions that execute on enter");
				onEnterList.DoLayoutList();
				EditorGUILayout.LabelField("Actions that execute on exit");
				onExitList.DoLayoutList();
			}

			showTransitions = EditorGUILayout.Toggle("Show transitions", showTransitions);

			if (showTransitions)
			{
				EditorGUILayout.LabelField("Conditions to exit this state");
				transitions.DoLayoutList();
			}

			serializedState.ApplyModifiedProperties();
		}

		void SetupReorderableLists()
		{
			State state = (State)target;
			serializedState = new SerializedObject(state);
			//onFixedList = new ReorderableList(serializedState, serializedState.FindProperty("onFixed"), true, true, true, true);
			onUpdateList = new ReorderableList(serializedState, serializedState.FindProperty("onState"), true, true, true, true);
			onEnterList = new ReorderableList(serializedState, serializedState.FindProperty("onEnter"), true, true, true, true);
			onExitList = new ReorderableList(serializedState, serializedState.FindProperty("onExit"), true, true, true, true);
			transitions = new ReorderableList(serializedState, serializedState.FindProperty("transitions"), true, true, true, true);

			//HandleReorderableList(onFixedList, "On Fixed");
			HandleReorderableList(onUpdateList, "On Update");
			HandleReorderableList(onEnterList, "On Enter");
			HandleReorderableList(onExitList, "On Exit");
			HandleTransitionReorderable(transitions, "Exit Conditions");
		}

		void HandleReorderableList(ReorderableList list, string targetName)
		{
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

		void HandleTransitionReorderable(ReorderableList list, string targetName)
		{
			list.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, targetName);
			};

			// Inspector GUI
			list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = list.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.ObjectField(
					new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight), 
					element.FindPropertyRelative("condition"), 
					GUIContent.none
				);
				EditorGUI.ObjectField(
					new Rect(rect.x + (rect.width * 0.35f), rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight), 
					element.FindPropertyRelative("targetState"), 
					GUIContent.none
				);
				EditorGUI.LabelField(
					new Rect(rect.x + (rect.width * 0.75f), rect.y, rect.width * 0.2f, EditorGUIUtility.singleLineHeight),
					"Disable"
				);
				EditorGUI.ObjectField(
					new Rect(rect.x + (rect.width * 0.90f), rect.y, rect.width * 0.2f, EditorGUIUtility.singleLineHeight), 
					element.FindPropertyRelative("disable"), 
					GUIContent.none
				);
			};
		}
	}
}