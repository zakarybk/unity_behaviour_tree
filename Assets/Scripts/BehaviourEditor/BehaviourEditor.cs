﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // Editor specifics

namespace SA.BehaviourEditor
{
	public class BehaviourEditor : EditorWindow
	{
		#region Variables
		Vector3 mousePosition;
		bool clickedOnWindow = false;
		BaseNode selectedNode;

		public static EditorSettings settings;
		int transitionFromId;
		Rect mouseRect = new Rect(0, 0, 1, 1);
		Rect all = new Rect(-5, -5, 10000, 10000);
		GUIStyle style;

		public enum UserActions
		{
			addState,
			addTransitionNode,
			removeNode,
			addComment,
			makeTransition
		}
		#endregion

		#region Init
		[MenuItem("Window/Behaviour Editor")]
		static void ShowEditor() // Seems like it should be able to take a bool - to show or not show
		{
			BehaviourEditor editor = EditorWindow.GetWindow<BehaviourEditor>();
			editor.minSize = new Vector3(800, 600); // Magic variables
		}

		private void OnEnable()
		{
			// Load saved settings
			settings = Resources.Load("EditorSettings") as EditorSettings;
			style = settings.skin.GetStyle("window");
		}
		#endregion

		#region GUI Methods
		private void OnGUI()
		{
			Event e = Event.current;
			mousePosition = e.mousePosition;

			UserInput(e); // Run mouse click events
			DrawWindows();

			if (e.type == EventType.MouseDrag)
			{
				settings.graph.DeleteWindowsThatNeedTo();
				Repaint();
			}

			if (settings.makeTransition)
			{
				// Draw a line/curve from selected node to mouse position
				mouseRect.x = mousePosition.x;
				mouseRect.y = mousePosition.y;
				Rect from = settings.graph.GetNodeWithIndex(transitionFromId).windowRect;
				DrawNodeCurve(from, mouseRect, true, Color.blue);
				Repaint(); // Repaint to stop it looking choppy
			}
		}

		void DrawWindows()
		{
			GUILayout.BeginArea(all, style); // Draw skin

			BeginWindows(); // ? where did this come from?

			EditorGUILayout.LabelField(" ", GUILayout.Width(100));
			EditorGUILayout.LabelField("Assign Graph:", GUILayout.Width(100));
			settings.graph = (BehaviourGraph)EditorGUILayout.ObjectField(settings.graph, typeof(BehaviourGraph), false, GUILayout.Width(200.0f));

			if (settings.graph != null)
			{
				// Draw curves between node windows
				foreach (BaseNode node in settings.graph.windows)
				{
					node.DrawCurve(node);
				}

				// Draw node windows
				for (int i = 0; i < settings.graph.windows.Count; i++)
				{
					settings.graph.windows[i].windowRect = GUI.Window(
						i,
						settings.graph.windows[i].windowRect,
						DrawNodeWindow,
						settings.graph.windows[i].windowTitle
					);
				}
			}

			EndWindows();
			GUILayout.EndArea();
		}

		// Draw passed node window based on index in windows
		void DrawNodeWindow(int i)
		{
			settings.graph.windows[i].DrawWindow(settings.graph.windows[i]);
			GUI.DragWindow();
		}

		void UserInput(Event e)
		{
			if (settings.graph == null)
				return;

			int LEFT = 0;
			int RIGHT = 1;

			if (!settings.makeTransition && e.type == EventType.MouseDown)
			{
				if (e.button == LEFT)
					LeftClick(e);
				else if (e.button == RIGHT)
					RightClick(e);
			}

			if (!settings.makeTransition && e.type == EventType.MouseDrag)
			{
				if (e.button == LEFT)
				{
					// Find the new selection
					selectedNode = NodeFromMousePosition();

//					if (selectedNode != null && graph != null)
//						graph.SetNode(selectedNode);
				}
			}

			if (settings.makeTransition && e.button == 0)
			{
				if (e.type == EventType.MouseDown)
				{
					MakeTransition();
				}
			}
		}

		void RightClick(Event e)
		{
			// Find the new selection
			selectedNode = NodeFromMousePosition();

			// null false | val true
			clickedOnWindow = selectedNode == null ? false : true;

			// Open the right context menu based on if you clicked the main window or a node
			if (clickedOnWindow)
				ModifyNode(e);
			else
				AddNewNode(e);
		}

		void MakeTransition()
		{
			settings.makeTransition = false;

			// Find the new selection
			selectedNode = NodeFromMousePosition();

			// null false | val true
			clickedOnWindow = selectedNode == null ? false : true;

			if (clickedOnWindow)
			{
				if (selectedNode.drawNode is StateNode)
				{
					if (selectedNode.id != transitionFromId)
					{
						// Get the transition and set the target
						BaseNode transitionNode = settings.graph.GetNodeWithIndex(transitionFromId);
						transitionNode.targetNode = selectedNode.id;

						// Set the targetState for the transition
						BaseNode enterNode = BehaviourEditor.settings.graph.GetNodeWithIndex(transitionNode.enterNode);
						Transition transition = enterNode.stateRef.currentState.GetTransition(transitionNode.transitionRef.transitionId);
						transition.targetState = selectedNode.stateRef.currentState;
					}
				}
			}
		}

		#endregion

		#region Context Menus

		/*	Creates a context menu like - for creating new nodes
		 * ----------
		 * | Item 1 |
		 * | Item 2 |
		 * | etc	|
		 * ----------
		 */
		void AddNewNode(Event e)
		{
			// Create the base
			GenericMenu menu = new GenericMenu();

			// No graph - no editing
			if (settings.graph != null)
			{
				// Add items to the menu
				menu.AddItem(new GUIContent("Add State"), false, ContextCallback, UserActions.addState);
				menu.AddItem(new GUIContent("Add Comment"), false, ContextCallback, UserActions.addComment);
			}
			else
			{
				// Add items to the menu
				menu.AddDisabledItem(new GUIContent("Add State"));
				menu.AddDisabledItem(new GUIContent("Add Comment"));
			}

			// Apply the menu
			menu.ShowAsContext();
			e.Use();
		}

		void ModifyNode(Event e)
		{
			// Create the base
			GenericMenu menu = new GenericMenu();

			// StateNode
			if (selectedNode.drawNode is StateNode) // poly instead?
			{
				if (selectedNode.stateRef.currentState != null)
				{
					menu.AddItem(new GUIContent("Add Condition"), false, ContextCallback, UserActions.addTransitionNode);
				}
				else
				{
					menu.AddDisabledItem(new GUIContent("Add Condition"));
				}

				// Add items to the menu
				menu.AddItem(new GUIContent("Add Condition"), false, ContextCallback, UserActions.addTransitionNode);
				menu.AddSeparator(""); // padding/spacing out
				menu.AddItem(new GUIContent("Remove"), false, ContextCallback, UserActions.removeNode);
			}

			// TransitionNode
			if (selectedNode.drawNode is TransitionNode)
			{
				// Only allow transitions from unassigned or duplicate nodes
				if (selectedNode.isDuplicate || !selectedNode.isAssigned)
				{
					menu.AddDisabledItem(new GUIContent("Make Transition"));
				}
				else
				{
					menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, UserActions.makeTransition);
				}

				// Add items to the menu
				menu.AddItem(new GUIContent("Remove"), false, ContextCallback, UserActions.removeNode);
			}

			// CommentNode
			if (selectedNode.drawNode is CommentNode)
			{
				// Add items to the menu
				menu.AddItem(new GUIContent("Remove"), false, ContextCallback, UserActions.removeNode);
			}

			// Apply the menu
			menu.ShowAsContext();
			e.Use();
		}

		void ContextCallback(object o)
		{
			// Get action from object
			UserActions actions = (UserActions)o;

			switch (actions)
			{
				case UserActions.addState:
					settings.AddNodeOnGraph(settings.stateNode, 200, 100, "State", mousePosition);

					break;
				case UserActions.addComment:
					BaseNode commentNode = settings.AddNodeOnGraph(settings.commentNode, 200, 100, "Comment", mousePosition);
					commentNode.comment = "This is a comment";

					break;
				case UserActions.addTransitionNode:
					BaseNode transitionNode = settings.AddNodeOnGraph(settings.transitionNode, 200, 100, "Condition", mousePosition);
					transitionNode.enterNode = selectedNode.id;
					Transition transition = settings.stateNode.AddTransition(selectedNode);
					transitionNode.transitionRef.transitionId = transition.id;

					break;
				case UserActions.removeNode:
					settings.graph.RemoveNode(selectedNode.id);
					break;
				case UserActions.makeTransition:
					transitionFromId = selectedNode.id;
					settings.makeTransition = true;
					break;
				default:
					Debug.Log("State not found!");
					break;
			}

			// Save the settings
			EditorUtility.SetDirty(settings);
			
		}
		#endregion

		// Current mouse position -- no fallback if no node is found
		BaseNode NodeFromMousePosition()
		{
			return NodeFromMousePosition(mousePosition);
		}
		// Custom mouse position
		BaseNode NodeFromMousePosition(Vector3 mousePosition)
		{
			for (int i = 0; i < settings.graph.windows.Count; i++)
			{
				if (settings.graph.windows[i].windowRect.Contains(mousePosition))
					return settings.graph.windows[i];
			}
			return null;
		}

		void LeftClick(Event e)
		{

		}

		#region Helper Methods

		public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColour)
		{
			Vector3 startPos = new Vector3(
				(left) ? start.x + start.width : start.x,
				start.y + (start.height * 0.5f), // 0 * 0.5 instead of diving by 0 by accident
				0
			);
			Vector3 endPos = new Vector3(
				end.x + (end.width * 0.5f),
				end.y + (end.height * 0.5f),
				0
			);

			// Tangents
			Vector3 startTan = startPos + Vector3.right * 50;
			Vector3 endTan = endPos + Vector3.left * 50;

			Color shadow = new Color(0, 0, 0, 0.06f);

			// Draw the shadow
			for (int i = 0; i < 3; i++)
			{
				Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, (i + 1) * 1);
			}

			// Draw the actual line
			Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColour, null, 3);
		}

		public static void ClearWindowsFromList(List<BaseNode> nodes)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
//				if (graph.windows.Contains(nodes[i]))
//					graph.windows.Remove(nodes[i]);
			}
		}

		#endregion

	}
}