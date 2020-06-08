using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Behaviour;

public class PlayerKill : MonoBehaviour
 {
    public GameObject player;
    public float killRange;

	void Start ()
    {
        player = GetComponent<StateManager>().player.gameObject;
	}

    private void Update()
    {
        KillPlayer();
    }

    public void KillPlayer()
    {
        float distanceToPlayer = Vector3.Distance(this.gameObject.transform.position, player.transform.position);
        if (distanceToPlayer < killRange)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
