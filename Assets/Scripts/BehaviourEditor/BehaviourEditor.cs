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
				// Add items to the menu
				menu.AddItem(new GUIContent("Add Transition"), false, ContextCallback, UserActions.addTransitionNode);
				menu.AddSeparator(""); // padding/spacing out
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
					// Create a new state node and set the position and size
					CommentNode comment = ScriptableObject.CreateInstance(typeof(CommentNode)) as CommentNode;
					comment.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 100); // magic numbers
					comment.windowTitle = "Comment";

					// Add the node to our list
					windows.Add(comment);
					break;
				case UserActions.removeNode:
					// Remove the node from the list - then it should drop out of scope and be garbage
					if (selectedNode)
					{
						windows.Remove(selectedNode);
						selectedNode = null;
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
		#endregion

	}
}