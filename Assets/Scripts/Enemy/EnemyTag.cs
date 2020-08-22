using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTag : MonoBehaviour {

	[SerializeField] private float tagDuration = 5.0f;

	private CameraController cameraController;
	private bool shouldShow;
	private float timeWhenEnabled;

	void Start ()
	{
		cameraController = FindObjectOfType<CameraController>();
	}

	private void Awake()
	{
		GetComponent<Renderer>().enabled = false;
	}

	void Update ()
	{
		if (shouldShow)
		{
			FacePlayer();
			UpdateTagDuration();
		}
	}

	void FacePlayer()
	{
		Camera playerCamera = cameraController.GetActiveCamera();
		GameObject player = playerCamera.gameObject;

		transform.LookAt(player.transform.position, Vector3.up);
		transform.Rotate(new Vector3(90, 0, 0));
	}

	void UpdateTagDuration()
	{
		if (timeWhenEnabled + tagDuration <= Time.time)
			ShowTag(false);
	}

	public void ShowTag(bool shouldShow)
	{
		// Enable
		if (shouldShow)
		{
			GetComponent<Renderer>().enabled = true;
			this.shouldShow = true;
			timeWhenEnabled = Time.time;
		}
		// Disable
		else
		{
			GetComponent<Renderer>().enabled = false;
			this.shouldShow = false;
		}
	}

}
