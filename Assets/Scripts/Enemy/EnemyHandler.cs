using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class EnemyHandler : MonoBehaviour
{
	[SerializeField]
	private Enemy[] enemies;

	public Vector3 lastPlayerLocation;
    public Text detectionText;

    void Awake ()
    {
		enemies = FindObjectsOfType<Enemy>();
	}

    private void Update()
    {
        if (detectionText != null)
        {
            float detectionLevel = HighestDetection();

            // Set the detection text ui
            if (detectionLevel > 0)
            {
                detectionText.text = detectionLevel.ToString();
                detectionText.enabled = true;
            }
            else
            {
                detectionText.enabled = false;
            }
        }
    }

	public void TagEnemy(GameObject enemy)
	{
		EnemyTag enemyTag = enemy.GetComponentInParent<Enemy>().GetComponentInChildren<EnemyTag>();
		enemyTag.ShowTag(true);
		Debug.Log(enemyTag);
	}

    public void AllHearSound(Vector3 pos)
    {
        float time = Time.time;

        foreach (Enemy enemy in enemies)
            enemy.HearSound(pos, time);
    }

    public float HighestDetection()
    {
        float highest = 0;

        foreach (Enemy enemy in enemies)
        {
            if (enemy.detectionLevel > highest)
                highest = enemy.detectionLevel;
        }

        return highest;
    }

    public Enemy[] Enemies()
    {
        return enemies;
    }

}
