using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.BehaviourEditor
{
	[CreateAssetMenu(menuName = "Behaviour/Editor/Nodes/Comment Node")]
	public class CommentNode : DrawNode
	{
		public override void DrawWindow(BaseNode baseNode)
		{
			baseNode.comment = GUILayout.TextArea(baseNode.comment, 200); // Magic number
		}

		public override void DrawCurve(BaseNode baseNode)
		{

		}
	}
}
