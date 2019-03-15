using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.BehaviourEditor
{
	public abstract class DrawNode : ScriptableObject
	{
		public abstract void DrawWindow(BaseNode baseNode);
		public abstract void DrawCurve(BaseNode baseNode);

	}
}