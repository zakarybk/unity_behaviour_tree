using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // Editor specifics

namespace SA.BehaviourEditor
{
	public class BehaviourEditor : EditorWindow
	{
		#region Variables
		static List<BaseNode> windows = new List<BaseNode>();
		Vector3 mousePosition;
		bool makingTransition = false;
		bool clickedOnWindow = false;
		BaseNode selectedNode;

		public static BehaviourGraph graph;
		static GraphNode graphNode;

		public enum UserActions
		{
			addState,
			addTransitionNode,
			removeNode,
			addComment
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
			if (graphNode == null)
			{
				graphNode = CreateInstance<GraphNode>();
				graphNode.windowRect = new Rect(10, position.height * 0.7f, 200, 100);
				graphNode.windowTitle = "Graph";

			}

			windows.Clear(); // Clear the list on enable as it's a static list

			windows.Add(graphNode);

			LoadGraph();
		}
		#endregion

		#region GUI Methods
		private void OnGUI()
		{
			Event e = Event.current;
			mousePosition = e.mousePosition;

			UserInput(e); // Run mouse click events
			DrawWindows();
		}

		void DrawWindows()
		{
			BeginWindows(); // ? where did this come from?

			// Draw curves between node windows
			foreach (BaseNode node in windows)
			{
				node.DrawCurve();
			}

			// Draw node windows
			for (int i = 0; i < windows.Count; i++)
			{
				windows[i].windowRect = GUI.Window(
					i,
					windows[i].windowRect,
					DrawNodeWindow,
					windows[i].windowTitle
				);
			}

			EndWindows();
		}

		// Draw passed node window based on index in windows
		void DrawNodeWindow(int i)
		{
			windows[i].DrawWindow();
			GUI.DragWindow();
		}

		void UserInput(Event e)
		{
			int LEFT = 0;
			int RIGHT = 1;

			if (!makingTransition && e.type == EventType.MouseDown)
			{
				if (e.button == LEFT)
					LeftClick(e);
				else if (e.button == RIGHT)
					RightClick(e);
			}

			if (!makingTransition && e.type == EventType.MouseDrag)
			{
				if (e.button == LEFT)
				{
					// Find the new selection
					selectedNode = NodeFromMousePosition();

					if (selectedNode != null && graph != null)
						graph.SetNode(selectedNode);
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
			if (graph != null)
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
			if (selectedNode is StateNode) // poly instead?
			{
				StateNode stateNode = (StateNode)selectedNode; // Why cast? Why create a var when selectedNode could be used?

				if (stateNode.currentState)
				{
					menu.AddItem(new GUIContent("Add Transition"), false, ContextCallback, UserActions.addTransitionNode);
				}
				else
				{
					menu.AddDisabledItem(new GUIContent("Add Transition"));
				}

				// Add items to the menu
				menu.AddItem(new GUIContent("Add Transition"), false, ContextCallback, UserActions.addTransitionNode);
				menu.AddSeparator(""); // padding/spacing out
				menu.AddItem(new GUIContent("Remove"), false, ContextCallback, UserActions.removeNode);
			}

			// TransitionNode
			if (selectedNode is TransitionNode)
			{
				// Add items to the menu
				menu.AddItem(new GUIContent("Remove"), false, ContextCallback, UserActions.removeNode);
			}

			// CommentNode
			if (selectedNode is CommentNode)
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
					AddStateNode(mousePosition);
					break;
				case UserActions.addComment:
					AddCommentNode(mousePosition);
					break;
				case UserActions.addTransitionNode:
					// Create a new transition node and set the position and size
					if (selectedNode is StateNode)
					{
						StateNode from = (StateNode)selectedNode;
//						Transition transition = from.AddTransition();

						AddTransitionNode(from.currentState.transitions.Count, null, from);
					}
					break;
				case UserActions.removeNode:
					if (selectedNode is StateNode)
					{
						StateNode target = (StateNode)selectedNode;
						target.ClearReferences();
						windows.Remove(target);
					}

					else if (selectedNode is TransitionNode)
					{
						TransitionNode target = (TransitionNode)selectedNode;
						windows.Remove(target);

						// Remove target reference from parent/enterState
//						if (target.enterState.currentState.transitions.Contains(target.targetCondition))
//						{
//							target.enterState.currentState.transitions.Remove(target.targetCondition);
//						}
					}

					else if (selectedNode is CommentNode)
					{
						windows.Remove(selectedNode); // Removing from list but never undefining selectedNode
					}

					break;
				default:
					Debug.Log("State not found!");
					break;
			}
		}

		// Current mouse position -- no fallback if no node is found
		BaseNode NodeFromMousePosition()
		{
			return NodeFromMousePosition(mousePosition);
		}
		// Custom mouse position
		BaseNode NodeFromMousePosition(Vector3 mousePosition)
		{
			for (int i = 0; i < windows.Count; i++)
			{
				if (windows[i].windowRect.Contains(mousePosition))
					return windows[i];
			}
			return null;
		}

		void LeftClick(Event e)
		{

		}
		#endregion

		#region Helper Methods
		public static StateNode AddStateNode(Vector2 position)
		{
			// Create a new state node and set the position and size
			StateNode stateNode = CreateInstance<StateNode>();
			stateNode.windowRect = new Rect(position.x, position.y, 200, 300); // magic numbers
			stateNode.windowTitle = "State";

			// Add the node to our list
			windows.Add(stateNode);

			// Add to the graph
//			graph.SetStateNode(stateNode);

			return stateNode;
		}

		public static CommentNode AddCommentNode(Vector2 position)
		{
			// Create a new comment node and set the position and size
			CommentNode commentNode = CreateInstance<CommentNode>();
			commentNode.windowRect = new Rect(position.x, position.y, 200, 100); // magic numbers
			commentNode.windowTitle = "Comment";

			// Add the node to our list
			windows.Add(commentNode);

			return commentNode;
		}

		public static TransitionNode AddTransitionNode(int index, Transition transition, StateNode from)
		{
			Rect fromRect = from.windowRect;
			fromRect.x += 50;
			float targetY = fromRect.y - fromRect.height;

			if (from.currentState)
			{
				targetY += (index * 100); // Magic number
			}

			fromRect.y = targetY;
			fromRect.x += 200 + 100;
			fromRect.y += fromRect.height * 0.7f;

			Vector2 position = new Vector2(fromRect.x, fromRect.y);

			return AddTransitionNode(position, transition, from);

		}

		public static TransitionNode AddTransitionNode(Vector2 position, Transition transition, StateNode from)
		{
			TransitionNode transitionNode = CreateInstance<TransitionNode>();
			transitionNode.Init(from, transition);
			transitionNode.windowRect = new Rect(position.x, position.y, 200, 80); // Magic numbers
			transitionNode.windowTitle = "Condition Check";

			windows.Add(transitionNode);

			// Add the new child to the StateNode
			from.childNodes.Add(transitionNode);

			return transitionNode;
		}

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
				Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, (i + 1) * 0.5f);
			}

			// Draw the actual line
			Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColour, null, 1);
		}

		public static void ClearWindowsFromList(List<BaseNode> nodes)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				if (windows.Contains(nodes[i]))
					windows.Remove(nodes[i]);
			}
		}

		public static void LoadGraph()
		{
			windows.Clear();
			windows.Add(graphNode);

			// No graph - no loading
			if (graph == null)
				return;

			graph.Init();

			// Clear the graph saved_StateNodes - copy for ourselfs first
			List<Saved_StateNode> saved_StateNodes = new List<Saved_StateNode>();
			saved_StateNodes.AddRange(graph.saved_StateNodes);
			graph.saved_StateNodes.Clear();

			for (int i = saved_StateNodes.Count - 1; i >= 0; i--)
			{
				StateNode stateNode = AddStateNode(saved_StateNodes[i].position);
				stateNode.currentState = saved_StateNodes[i].state;
				stateNode.collapse = saved_StateNodes[i].isCollapsed;

				for (int transitionIndex = saved_StateNodes[i].savedConditions.Count - 1; transitionIndex >= 0 ; transitionIndex--)
				{
					TransitionNode transitionNode = AddTransitionNode(
						saved_StateNodes[i].savedConditions[transitionIndex].position,
						saved_StateNodes[i].savedConditions[transitionIndex].transition,
						stateNode
					);

					transitionNode.targetCondition = saved_StateNodes[i].savedConditions[transitionIndex].condition;
				}

				// Load transitions
			}
		}

		#endregion

	}
}