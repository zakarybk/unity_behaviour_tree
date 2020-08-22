using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyConfig : MonoBehaviour
{
	[Header("Path finding")]
	[HideInInspector] public NavMeshAgent agent;
    public PatrolPath path;
    public PatrolPoint startingPoint;
    [Tooltip("1 forward, -1 backwards")]
    [HideInInspector] public int patrolDirection = 1; // 1 or -1
    public float patrolSwitchDistance = 0.6f;

    [Header("Movement")]
	public float walkSpeed = 1.5f;
    public float searchSpeed = 3f;
    public float runSpeed = 6f;

	[Header("Colour")]
	public Color idle;
	public Color searching;
	public Color pursuing;

	[Header("Detection")]
	public float surroundingDistance = 1f;
	public float fastDetectionRadius = 4.0f;
	public float slowDetectionRadius = 20.0f;
	[Range(0.0f, 360.0f)]
	public float detectionAngle = 120.0f;
	[HideInInspector] public float distanceModifier = 60.0f;
	public float detectionGainAmount = 25.0f;
	public float detectionLossAmount = 10.0f;
	public Text detectionText;
    public float searchThreshold = 25.0f;

    [Header("Sound Detection")]
    public float timeSinceHeard = 1.0f;
    public float hearingDistance = 10.0f;

    [Header("Utility")]
	public float tagCooldownInSeconds;
	//public float searchThreshold = 50;

	[Header("Debug")]
	public bool drawFOVDetection = true;

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }
}
