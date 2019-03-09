using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.BehaviourEditor;

namespace SA
{
	[CreateAssetMenu]
	public class BehaviourGraph : ScriptableObject
	{
		public List<BaseNode> windows = new List<BaseNode>();

		#region Helpers
		public bool IsStateNodeDuplicate(StateNode node)
		{
			bool isDuplicate = false;

			return isDuplicate;
		}
		#endregion
	}

}

