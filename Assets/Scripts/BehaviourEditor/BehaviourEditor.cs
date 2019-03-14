using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // Editor specifics

namespace SA.BehaviourEditor
{
	public class BehaviourEditor : EditorWindow
	{
		#region Variables
		Vector3 mousePosition;
		bool makingTransition = false;
		bool clickedOnWindow = false;
		BaseNode selectedNode;

		public static EditorSettings settings;

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
			// Load saved settings
			settings = Resources.Load("EditorSettings") as EditorSettings;
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
		}

		void DrawWindows()
		{
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

//					if (selectedNode != null && graph != null)
//						graph.SetNode(selectedNode);
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
			if (selectedNode.drawNode is TransitionNode)
			{
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
					BaseNode transitionNode = settings.AddNodeOnGraph(settings.transitionNode, 200, 100, "Transition", mousePosition);
					transitionNode.enterNode = selectedNode.id;
					Transition transition = settings.stateNode.AddTransition(selectedNode);
					transitionNode.transitionRef.transitionId = transition.id;

					break;
				case UserActions.removeNode:
					settings.graph.RemoveNode(selectedNode.id);
					break;
				default:
					Debug.Log("State not found!");
					break;
			}

			// Save the settings
			EditorUtility.SetDirty(settings);
			
		}

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
		#endregion

		#region Helper Methods
		/*
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
		*/

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
//				if (graph.windows.Contains(nodes[i]))
//					graph.windows.Remove(nodes[i]);
			}
		}

		#endregion

	}
}