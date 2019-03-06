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
		#endregion

		#region GUI Methods
		private void OnGUI()
		{
			Event e = Event.current;
			mousePosition = e.mousePosition;

			UserInput(e); // Run mouse click events
			DrawWindows();
		}

		private void OnEnable()
		{
			//windows.Clear(); // Clear the list on enable as it's a static list
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
			if (!makingTransition && e.type == EventType.MouseDown)
			{
				if (e.button == 0)
					LeftClick(e);
				else if (e.button == 1)
					RightClick(e);
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

			// Add items to the menu
			menu.AddItem(new GUIContent("Add State"), false, ContextCallback, UserActions.addState);
			menu.AddItem(new GUIContent("Add Comment"), false, ContextCallback, UserActions.addComment);

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
					// Create a new state node and set the position and size
					StateNode state = ScriptableObject.CreateInstance(typeof(StateNode)) as StateNode;
					state.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 300); // magic numbers
					state.windowTitle = "State";

					// Add the node to our list
					windows.Add(state);
					break;
				case UserActions.addComment:
					// Create a new comment node and set the position and size
					CommentNode comment = ScriptableObject.CreateInstance(typeof(CommentNode)) as CommentNode;
					comment.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 100); // magic numbers
					comment.windowTitle = "Comment";

					// Add the node to our list
					windows.Add(comment);
					break;
				case UserActions.addTransitionNode:
					// Create a new transition node and set the position and size
					if (selectedNode is StateNode)
					{
						StateNode from = (StateNode)selectedNode;
						Transition transition = from.AddTransition();

						AddTransitionNode(from.currentState.transitions.Count, transition, from);
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
						if (target.enterState.currentState.transitions.Contains(target.targetTransition))
						{
							target.enterState.currentState.transitions.Remove(target.targetTransition);
						}
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

			TransitionNode transitionNode = CreateInstance<TransitionNode>();
			transitionNode.Init(from, transition);
			transitionNode.windowRect = new Rect(fromRect.x + 200 + 100, fromRect.y + (fromRect.height * 0.7f), 200, 80); // Magic numbers
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

		#endregion

	}
}