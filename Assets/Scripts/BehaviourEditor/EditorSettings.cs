using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.BehaviourEditor
{
	[CreateAssetMenu(menuName ="Editor/Settings")]
	public class EditorSettings : ScriptableObject
	{
		public BehaviourGraph graph;
		public StateNode stateNode;
		public TransitionNode transitionNode;
		public CommentNode commentNode;
	}
}