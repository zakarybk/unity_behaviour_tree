using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TunnelGeneration : MonoBehaviour {

	public GameObject[] TunnelObjects;
	public int speed;
	public int tunnelType = 0;

	
	Queue<GameObject> Tunnels = new Queue<GameObject>();
	float tunnelLength;
	[SerializeField]
	float offset;
	public int tunnelSections;

	Vector3 startPos = new Vector3(-20, 0, 0);
	

	void Start()
	{
		offset = 0f;
		tunnelLength = 5f;

		for (int i = 0; i < tunnelSections; i++)
		{
            CreateTunnelSection();
        }
	}

	void CreateTunnelSection()
	{
        GameObject newTunnel = Instantiate(
            TunnelObjects[tunnelType], 
            new Vector3(startPos.x, startPos.y, transform.position.z + (Tunnels.Count()) + (tunnelLength * (tunnelSections / 2))), 
            Quaternion.Euler(0, 0, 0)
        );
        Tunnels.Enqueue(newTunnel);
        tunnelType = (tunnelType + 1) % (TunnelObjects.Length);
    }

	void Update()
	{
		offset += speed * Time.deltaTime;

        if (offset >= tunnelLength)
        {
            SwapTunnelBit();
        }

        // Wrap the offset around to stop potential jumping
        offset = offset % tunnelLength;

        MoveTunnelSections();
	}

	void MoveTunnelSections()
	{
		for (int i = 0; i < tunnelSections; i++)
		{
			Tunnels.ElementAt(i).gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, startPos.z + (i * tunnelLength) - offset - (tunnelLength * (tunnelSections / 2)));
		}
			
	}

	void SwapTunnelBit()
	{
		GameObject endTunnel = Tunnels.Dequeue();
        Tunnels.Enqueue(endTunnel);
    }
}
