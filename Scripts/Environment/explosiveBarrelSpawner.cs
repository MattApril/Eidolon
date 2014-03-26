using UnityEngine;
using System.Collections;

public class explosiveBarrelSpawner : MonoBehaviour {
	
	public float spawnRate = 4.0f;
	public GameObject barrel;
	
	private float spawnTimer;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	public void SpawnBarrel()
	{
		Instantiate( barrel, this.transform.position, barrel.transform.rotation );	
	}
}
