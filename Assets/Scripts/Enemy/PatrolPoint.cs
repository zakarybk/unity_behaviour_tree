using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
	[Header("Route")]
	[SerializeField] private PatrolPath path;
	[SerializeField] private PatrolPoint[] connectedPaths;
    public GameObject[] gazePoints;

    [Header("Gaze time (1 <= no looking)")]
	[SerializeField] private float gazeMin;
	[SerializeField] private float gazeMax;

    [Header("Debug")]
    public bool drawLinks = true;

    [HideInInspector] public float distanceToNext;
	[HideInInspector] public float distanceToPrevious;

	private Color32 hotPink = new Color32(255, 105, 180, 255);
	// The minimum amount of time required to be set for lookAroundMinTime and lookAroundMaxTime
	// to make the current RouteLinker point, a point where an enemy will pause to look around
	private float gazeActivation = 1.0f;

    private void Awake()
    {
        path = GetComponentInParent<PatrolPath>();
    }

    public string PathName()
	{
		return path.name;
	}

	public PatrolPath Path()
	{
		return path;
	}

    public bool ShouldGaze()
	{
		return gazeMin >= gazeActivation &&
            gazeMax >= gazeActivation &&
            gazeMax >= gazeMin &&
            gazePoints.Length > 0;
	}

	public float GazeTime()
	{
		return Random.Range(gazeMin, gazeMax);
	}

	public bool HasConnectionsToPaths()
	{
		return connectedPaths != null && connectedPaths.Length > 0;
	}

    public bool HasConnectionToPoint(PatrolPoint patrolPath)
    {
        int index = System.Array.IndexOf(connectedPaths, patrolPath);

        return index > -1;
    }

    public bool HasConnectionToPath(PatrolPath patrolPath)
    {
        return ConnectionToPath(patrolPath) != null;
    }

    public PatrolPoint ConnectionToPath(PatrolPath patrolPath)
    {
        foreach (PatrolPoint point in connectedPaths)
        {
            if (point.Path() == patrolPath)
                return point;
        }
        return null;
    }

	public PatrolPoint[] ConnectedPaths()
	{
        return connectedPaths;
	}

	private void OnDrawGizmos()
	{
		if (drawLinks && HasConnectionsToPaths())
		{
			for (int i = 0; i < connectedPaths.Length; i++)
			{
				if (connectedPaths[i] != null)
				{
					Gizmos.color = hotPink;
					Gizmos.DrawLine(transform.position, connectedPaths[i].transform.position);
				}
			}

		}
	}
}
