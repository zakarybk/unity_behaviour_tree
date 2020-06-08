using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behaviour;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	[HideInInspector] public StateManager stateManager;
	[HideInInspector] public EnemyHandler enemyHandler;

    [HideInInspector] public Vector3 directionToPlayer;
	public bool isPlayerDetected; // Setup later (not every guard goes after you - just ones in area/groups)
	public float detectionLevel;

	// Route and patrol points
	[HideInInspector] public PatrolPath path;
    [HideInInspector] public int nextPatrolPoint = 0; // 0 to n
    [HideInInspector] public int lastPatrolPoint = 0; // 0 to n
    [HideInInspector] public int patrolDirection = 1; // 1 or -1

    // The route the enemy is moving to (recursive - can find through route linkers) -- TO be implimented
    [Tooltip("The path the enemy will switch to when a patrol point in the current path has a link to it.")]
    public PatrolPath targetPath;

    [HideInInspector] public NavMeshPath navMeshPath;
    [HideInInspector] public Player player;
    [HideInInspector] public EnemyConfig config;

    [HideInInspector] public GameObject gazeObject;
    [HideInInspector] public float gazeEndTime;
    [HideInInspector] public Animator animator;

    // Hearing
    [HideInInspector] float timeWhenHeard = 0.0f;
    [HideInInspector] bool heardSound = false;
    [HideInInspector] public Vector3 soundPosition = Vector3.zero;

    // Use this for initialization
    private void Start ()
	{
		patrolDirection = PatrolPath.FORWARDS;

		stateManager = GetComponent<StateManager>();
		enemyHandler = FindObjectOfType<EnemyHandler>();
		player = FindObjectOfType<Player>();
		config = GetComponent<EnemyConfig>();

        // Default/starting route
        path = config.path;
        patrolDirection = config.patrolDirection;

        navMeshPath = new NavMeshPath();
        animator = GetComponentInChildren<Animator>();

        if (path != null && config.startingPoint != null)
        {
            nextPatrolPoint = path.IndexOfPoint(config.startingPoint);
        }
    }

    public Vector3 lastKnownLocation
	{
		get { return enemyHandler.lastPlayerLocation; }
		set { enemyHandler.lastPlayerLocation = value; }
	}

	public Vector3 EyePos()
	{
		return transform.position + (Vector3.up * 1.9f);
	}

	public void UpdateDirectionToPlayer()
	{
		directionToPlayer = player.transform.position - transform.position;
	}

	public float DistanceToPlayer()
	{
		return DistanceToGameObject(player.gameObject);
	}

	public float DistanceToGameObject(GameObject gameObject)
	{
		return Vector3.Distance(
			transform.position,
			gameObject.transform.position
		);
	}

    public float DistanceToGameObjectFromEyes(GameObject gameObject)
    {
        return Vector3.Distance(
            EyePos(),
            gameObject.transform.position
        );
    }

    public bool CanSeePlayer()
	{
		return PlayerInFOV() && PlayerInSight();
	}

	private bool PlayerInFOV()
	{
		bool inFOV = false;
		float angle = Vector3.Angle(directionToPlayer, transform.forward);

		if (angle < config.detectionAngle / 2)
			inFOV = true;

		return inFOV;
	}

	private bool PlayerInSight()
	{
		return ObjectInSight("Player", directionToPlayer);
	}

	public bool ObjectInSight(string tag, Vector3 direction)
	{
		bool inSight = false;

		Ray enemyRay = new Ray(EyePos(), direction);

		RaycastHit enemyRayInfo;
		bool isValid = Physics.Raycast(enemyRay, out enemyRayInfo);

		if (isValid && enemyRayInfo.collider.tag == tag)
			inSight = true;

		return inSight;
	}

	// Draw debug/for designers, enemy detection
	private void OnDrawGizmos()
	{
		EnemyConfig config = GetComponent<EnemyConfig>();
		// Only draw if wanted
		if (!config.drawFOVDetection)
			return;

		Gizmos.color = Color.red;

		// Left (maximum distance)
		Vector3 direction = Quaternion.Euler(0, -config.detectionAngle / 2, 0) * transform.forward;
		Gizmos.DrawLine(
			EyePos(),
			EyePos() + Vector3.up + direction * config.slowDetectionRadius
		);

		// Right (maximum distance)
		direction = Quaternion.Euler(0, config.detectionAngle / 2, 0) * transform.forward;
		Gizmos.DrawLine(
			EyePos(),
			EyePos() + direction * config.slowDetectionRadius
		);

		// Minimum distance
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(
			EyePos(),
			EyePos() + transform.forward * config.fastDetectionRadius
		);
	}

    public void MoveToPatrolPoint(int index)
    {
        MoveToPosition(path.PointAtIndex(index).transform.position);
    }

	public void MoveToPosition(Vector3 newPos)
	{
		config.agent.SetDestination(newPos);
	}

    public bool IsPathPossible(Vector3 targetPos)
    {
        config.agent.CalculatePath(targetPos, navMeshPath);

        return navMeshPath.status == NavMeshPathStatus.PathComplete;
    }

    public PatrolPoint PatrolPoint()
    {
        return path.PointAtIndex(nextPatrolPoint);
    }

    public void HearSound(Vector3 pos, float eventTime)
    {
        // If played too long ago - do nothing
        if (Time.time - eventTime > config.timeSinceHeard)
            return;

       // If within hearing distance
        if (PathLength(transform.position, pos) < config.hearingDistance)
        {
            soundPosition = pos;
            timeWhenHeard = eventTime;
            heardSound = true;
        }
    }

    public float PathLength(Vector3 startPos, Vector3 endPos)
    {
        // Calculate the path
        NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, navMeshPath);

        // Less than two points means no path
        if (navMeshPath.corners.Length < 2 || navMeshPath.status != NavMeshPathStatus.PathComplete)
            return Mathf.Infinity;

        Vector3 previousCorner = navMeshPath.corners[0];
        float lengthSoFar = 0.0F;
        int i = 1;

        // Calculate cumulative path distance
        while (i < navMeshPath.corners.Length)
        {
            Vector3 currentCorner = navMeshPath.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
            i++;
        }

        return lengthSoFar;
    }

    public bool HasHeardSound()
    {
        return heardSound == true && (Time.time - timeWhenHeard < config.timeSinceHeard) && soundPosition != Vector3.zero;
    }

    public void ClearSound()
    {
        heardSound = false;
        timeWhenHeard = 0.0f;
        soundPosition = Vector3.zero;
    }
}
