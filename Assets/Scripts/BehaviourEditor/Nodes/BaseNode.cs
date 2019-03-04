using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.BehaviourEditor
{
	public abstract class BaseNode : ScriptableObject
	{
		public Rect windowRect;
		public string windowTitle;

		// Draw the node window
		public virtual void	DrawWindow()
		{

		}

		// Draw the curves between node windows
		public virtual void DrawCurve()
		{

		}

	}
}