using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // Editor specifics

namespace Behaviour.BehaviourEditor
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
		GUIStyle activeStyle;
		Vector2 scrollPos;
		Vector2 scrollStartPos;
		static BehaviourEditor editor;
		public static StateManager stateManager;
		public static bool forceSetDirty;
		static StateManager previousStateManager;
		static State previousState;

		public enum UserActions
		{
			addState,
			addTransitionNode,
			removeNode,
			addComment,
			makeTransition,
			makePortal,
			resetPan
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
			activeStyle = settings.activeSkin.GetStyle("window");
		}

		private void Update()
		{
			// Repaint if anything has changed
			if (stateManager != null)
			{
				if (previousState != stateManager.currentState)
				{
					previousState = stateManager.currentState;
					Repaint();
				}
			}
		}

		#endregion

		#region GUI Methods
		private void OnGUI()
		{
			// Switch to the StateManager of the currently selected object in the hierarchy
			if (Selection.activeTransform != null)
			{
				stateManager = Selection.activeTransform.GetComponentInChildren<StateManager>();

				if (previousStateManager != stateManager)
				{
					previousStateManager = stateManager;
					Repaint();
				}
			}

			Event e = Event.current;
			mousePosition = e.mousePosition;

			UserInput(e); // Run mouse click events
			DrawWindows();

			if (e.type == EventType.MouseDrag)
			{
				settings.graph.DeleteWindowsThatNeedTo();
				Repaint();
			}

			if (GUI.changed)
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

			// If we should save
			if (forceSetDirty)
			{
				forceSetDirty = false;
				// Save the settings and graph
				EditorUtility.SetDirty(settings);
				EditorUtility.SetDirty(settings.graph);

				for (int i = 0; i < settings.graph.windows.Count; i++)
				{
					BaseNode baseNode = settings.graph.windows[i];

					// Save the states
					if (baseNode.stateRef.currentState != null)
						EditorUtility.SetDirty(baseNode.stateRef.currentState);
				}
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
					/*
					settings.graph.windows[i].windowRect = GUI.Window(
						i,
						settings.graph.windows[i].windowRect,
						DrawNodeWindow,
						settings.graph.windows[i].windowTitle
					);
					*/

					BaseNode baseNode = settings.graph.windows[i];

					if (baseNode.drawNode is StateNode)
					{
						// Draw differently if the window is selected
						if (stateManager != null && baseNode.stateRef.currentState == stateManager.currentState)
						{
							baseNode.windowRect = GUI.Window(i, baseNode.windowRect,
								DrawNodeWindow, baseNode.windowTitle, activeStyle);
						}
						else
						{
							baseNode.windowRect = GUI.Window(i, baseNode.windowRect,
								DrawNodeWindow, baseNode.windowTitle);
						}
					}
					else
					{
						baseNode.windowRect = GUI.Window(i, baseNode.windowRect,
								DrawNodeWindow, baseNode.windowTitle);
					}
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
			int MIDDLE = 2;

			// Creating nodes
			if (!settings.makeTransition && e.type == EventType.MouseDown)
			{
				if (e.button == LEFT)
					LeftClick(e);
				else if (e.button == RIGHT)
					RightClick(e);
			}

			// Moving
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

			// Creating transitions
			if (settings.makeTransition && e.button == LEFT)
			{
				if (e.type == EventType.MouseDown)
				{
					MakeTransition();
				}
			}

			// Panning
			if (e.button == MIDDLE)
			{
				if (e.type == EventType.MouseDown)
				{
					scrollStartPos = e.mousePosition;
				}
				else if (e.type == EventType.MouseDrag)
				{
					HandlePanning(e);
				}
				else if (e.type == EventType.MouseUp)
				{

				}
			}
		}

		void HandlePanning(Event e)
		{
			// Workout the offset of the mouse movement
			Vector2 offset = e.mousePosition - scrollStartPos;
			offset *= 0.6f;
			scrollStartPos = e.mousePosition;
			scrollPos += offset;

			// Move all windows accordingly to pan across
			for (int i = 0; i < settings.graph.windows.Count; i++)
			{
				BaseNode baseNode = settings.graph.windows[i];

				baseNode.windowRect.x += offset.x;
				baseNode.windowRect.y += offset.y;
			}
		}

		void ResetScroll()
		{
			for (int i = 0; i < settings.graph.windows.Count; i++)
			{
				BaseNode baseNode = settings.graph.windows[i];

				baseNode.windowRect.x -= scrollPos.x;
				baseNode.windowRect.y -= scrollPos.y;
			}

			scrollPos = Vector2.zero;
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
				if (selectedNode.drawNode is StateNode || selectedNode.drawNode is PortalNode)
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
				menu.AddItem(new GUIContent("Add Portal"), false, ContextCallback, UserActions.makePortal);
				menu.AddItem(new GUIContent("Add Comment"), false, ContextCallback, UserActions.addComment);
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Reset camera"), false, ContextCallback, UserActions.resetPan);
			}
			else
			{
				// Add items to the menu
				menu.AddDisabledItem(new GUIContent("Add State"));
				menu.AddDisabledItem(new GUIContent("Add Portal"));
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

			// PortalNode
			if (selectedNode.drawNode is PortalNode) // poly instead?
			{
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
				case UserActions.makePortal:
					settings.AddNodeOnGraph(settings.portalNode, 200, 50, "Portal", mousePosition);
					break;
				case UserActions.addComment:
					BaseNode commentNode = settings.AddNodeOnGraph(settings.commentNode, 200, 100, "Comment", mousePosition);
					commentNode.comment = "This is a comment";

					break;
				case UserActions.addTransitionNode:
					AddTransitionNode(selectedNode, mousePosition);

					break;
				case UserActions.removeNode:
					if (selectedNode.drawNode is TransitionNode)
					{
						BaseNode enterNode = settings.graph.GetNodeWithIndex(selectedNode.enterNode);
						enterNode.stateRef.currentState.RemoveTransition(selectedNode.transitionRef.transitionId);
					}
					settings.graph.RemoveNode(selectedNode.id);
					break;
				case UserActions.makeTransition:
					transitionFromId = selectedNode.id;
					settings.makeTransition = true;
					break;
				case UserActions.resetPan:
					ResetScroll();
					break;
				default:
					Debug.Log("State not found!");
					break;
			}

			// Save the settings
			forceSetDirty = true;
			
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

		public static BaseNode AddTransitionNode(BaseNode enterNode, Vector3 pos)
		{
			// Create a transition node and set the enter node reference
			BaseNode transitionNode = settings.AddNodeOnGraph(settings.transitionNode, 200, 100, "Condition", pos); // Magic numbers
			transitionNode.enterNode = enterNode.id;

			// Create a transition and assign the transition id to the transition node
			Transition transition = settings.stateNode.AddTransition(enterNode);
			transitionNode.transitionRef.transitionId = transition.id;

			return transitionNode;
		}

		public static BaseNode AddTransitionNodeFromTransition(Transition transition, BaseNode enterNode, Vector3 pos)
		{
			// Create a transition node and set the enter node reference
			BaseNode transitionNode = settings.AddNodeOnGraph(settings.transitionNode, 200, 100, "Condition", pos); // Magic numbers
			transitionNode.enterNode = enterNode.id;

			// Assign the transition id to the transition node
			transitionNode.transitionRef.transitionId = transition.id;

			return transitionNode;
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