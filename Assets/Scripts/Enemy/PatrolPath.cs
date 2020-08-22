using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [SerializeField] private Color pathColour;
    [SerializeField] private PatrolPoint[] patrolPoints; // path's control points
	[SerializeField] private bool shouldRouteLoop;

	public const int FORWARDS = 1;
	public const int BACKWARDS = -1;

	// Serialised patrol points - will only contain valid indexes
	private PatrolPoint[] validPatrolPoints;

	[Header("Debug")]
	public bool drawLinks = true;

	// Draw the path/route in the editor
	private void OnDrawGizmos()
    {
		if (!drawLinks)
			return;

        // At least two valid points for a line to be drawn
        if (patrolPoints.Length >= 2 && patrolPoints[0] != null && patrolPoints[1] != null)
		{ 
			for (int i = 0; i < patrolPoints.Length - 1; i++)
            {
				// If the point is valid, draw lines between each point
				if (patrolPoints[i] != null && patrolPoints[i + 1] != null)
				{
					Gizmos.color = pathColour;
					Gizmos.DrawLine(patrolPoints[i].transform.position, patrolPoints[i + 1].transform.position);
				}
            }

			// If the route should be a loop, visually join the first and last elements to create the connection
			if (ShouldRouteLoop())
			{
				Gizmos.DrawLine(patrolPoints[0].transform.position, patrolPoints[patrolPoints.Length-1].transform.position);
			}

        }
    }

	private void Awake()
	{
		// Count how many valid points there are
		int validCount = 0;
		for (int i = 0; i < patrolPoints.Length; i++)
		{
			if (patrolPoints[i] != null)
				validCount++;
		}

		// Make an array with all the valid points
		validPatrolPoints = new PatrolPoint[validCount];
		for (int i = 0; i < patrolPoints.Length; i++)
		{
			if (patrolPoints[i] != null)
				validPatrolPoints[i] = patrolPoints[i];
		}

		// Cache distances between all points
		if (patrolPoints.Length >= 2)
		{
			for (int i = 0; i < validPatrolPoints.Length - 1; i++)
			{
				int next = NextPatrolPoint(i, FORWARDS);
				int previous = NextPatrolPoint(i, BACKWARDS);

				// Case for non-looping routes
				if (!ShouldRouteLoop())
				{
					if (next < i)
					{
						next = i;
					}
					if (previous > i)
					{
						previous = i;
					}
				}

				// Distance to next point
				validPatrolPoints[i].distanceToNext = Vector3.Distance(
					validPatrolPoints[i].transform.position,
					validPatrolPoints[next].transform.position
				);

				// Distance to previous point
				validPatrolPoints[i].distanceToPrevious = Vector3.Distance(
					validPatrolPoints[i].transform.position,
					validPatrolPoints[previous].transform.position
				);
			}

		}
	}

	public int NumberOfPoints()
	{
		return validPatrolPoints.Length;
	}

	public PatrolPoint PointAtIndex(int index)
	{
		if (index < 0 || index >= validPatrolPoints.Length)
			return null;
		else
			return validPatrolPoints[index];
	}

	public int IndexOfPoint(PatrolPoint point)
	{
		for (int i = 0; i < validPatrolPoints.Length; i++)
		{
			if (validPatrolPoints[i] == point)
				return i;
		}
		return -1;
	}

	// Returns whether a link from the current path to the named path exists
	public bool HasConnectionToPath(PatrolPath patrolPath)
	{
        bool hasConnection = false;

        foreach (PatrolPoint point in validPatrolPoints)
        {
            if (point.HasConnectionToPath(patrolPath))
            {
                hasConnection = true;
                break;
            }
        }

        return hasConnection;
    }

	// Returns the patrol points which contain a link to the given path
	public List<PatrolPoint> PointsWithConnectionToPath(PatrolPath patrolPath)
	{
        List<PatrolPoint> links = new List<PatrolPoint>();

        foreach (PatrolPoint patrolPoint in validPatrolPoints)
        {
            if (patrolPoint.HasConnectionToPath(patrolPath))
                links.Add(patrolPoint);
        }

        return links;
	}

	public int FastestDirection(int start, int target)
	{
		return FastestDirection(start, target, new Vector3(0, 0, 0));
	}

	public int FastestDirection(int start, int target, Vector3 currentPosisiton)
	{
		int direction = 1;

		// If the route loops around (has no end)
		if (ShouldRouteLoop())
		{
			// Cumulative distance following the route forwards and backwards through each point
			float forwardDistance = DistanceBetweenPoints(start, target, FORWARDS);
			float backwardDistance = DistanceBetweenPoints(start, target, BACKWARDS);

			// Next and previous patrol point based off start point/index
			int nextPoint = NextPatrolPoint(start, PatrolPath.FORWARDS);
			int previousPoint = NextPatrolPoint(start, PatrolPath.BACKWARDS);

			// Workout the extra distance to each point from the current position
			float distanceToNext = Vector3.Distance(
				currentPosisiton,
				PointAtIndex(nextPoint).transform.position
			);
			float distanceToPrevious = Vector3.Distance(
				currentPosisiton,
				PointAtIndex(previousPoint).transform.position
			);
			float distanceToCurrent = Vector3.Distance(
				currentPosisiton,
				PointAtIndex(start).transform.position
			);

			// Add to the cumulative distance based on the position to points
			forwardDistance += distanceToNext - distanceToCurrent;
			backwardDistance += distanceToPrevious - distanceToCurrent;

			// Which direction is faster based on least distance
			if (forwardDistance < backwardDistance)
				direction = FORWARDS;
			else
				direction = BACKWARDS;

		}
		// Only one possible direction to destination
		else
		{
			if (start <= target)
				direction = FORWARDS;
			else
				direction = BACKWARDS;
		}

		// FORWARDS or BACKWARDS
		return direction;
	}

	// https://stackoverflow.com/a/6400477
	private float nfmod(float a, float b)
	{
		return a - b * Mathf.Floor(a / b);
	}

	// Returns the distance from one point in the route to another whilst taking into account the
	// distance between all the points on the way to the final/target point
	public float DistanceBetweenPoints(int startIndex, int targetIndex, int direction)
	{
		// Most number of points that can be looped through 
		int MAX_LOOPS = validPatrolPoints.Length;

		// Check for valid index
		startIndex = Mathf.Clamp(startIndex, 0, validPatrolPoints.Length - 2);
		targetIndex = Mathf.Clamp(targetIndex, 1, validPatrolPoints.Length - 1);

		// Cumulative distance
		float totalDistance = 0.0f;

		// Loop variables
		int loopCount = 0;
		int currentIndex = startIndex;

		while(currentIndex != targetIndex && loopCount <= MAX_LOOPS)
		{
			// Find the next point
			PatrolPoint currentPoint = validPatrolPoints[currentIndex];

			// Add to cumulative distance
			totalDistance += currentPoint.distanceToNext;

			// Update index based on direction
			currentIndex = NextPatrolPoint(currentIndex, direction);

			// Check for infinite loop
			loopCount++;
		}

		if (loopCount > MAX_LOOPS)
			Debug.LogError(
				"AVOIDED INFINITE LOOP IN ROUTE DistanceToPointFromIndex. Looped for: "
				+ loopCount
			);

		// Return cumulative distance
		return totalDistance;
	}

	public int NextPatrolPoint(int current, int direction = FORWARDS)
	{
		return (int)nfmod((float)(current + direction), (float)(validPatrolPoints.Length));
	}

	public bool ShouldRouteLoop()
	{
		return shouldRouteLoop;
	}

	public int OppositeDirection(int direction)
	{
		return direction * (-1);
	}

    public bool IsOnPoint(Enemy enemy, int point)
    {
        return Vector3.Distance(
            enemy.transform.position,
            PointAtIndex(point).transform.position
        ) < enemy.config.patrolSwitchDistance;
    }
}
