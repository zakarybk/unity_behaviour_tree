using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SA.BehaviourEditor
{
	public class GraphNode : BaseNode
	{
		BehaviourGraph previousGraph;

		public override void DrawWindow()
		{
			if (BehaviourEditor.graph == null)
			{
				// No graph found - notify user for one
				EditorGUILayout.LabelField("Add graph");
			}

			BehaviourEditor.graph = (BehaviourGraph)EditorGUILayout.ObjectField(BehaviourEditor.graph, typeof(BehaviourGraph), false);

			if (BehaviourEditor.graph == null)
			{
				if (previousGraph != null)
				{
					previousGraph = null;
				}
				EditorGUILayout.LabelField("No graph assigned");
				return;
			}

			if (previousGraph != BehaviourEditor.graph)
			{
				previousGraph = BehaviourEditor.graph;
				BehaviourEditor.LoadGraph();
			}
		}

		public override void DrawCurve()
		{
			base.DrawCurve();
		}

	}
}
