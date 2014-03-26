using UnityEngine;
using System.Collections;

public class pauseMenu : MonoBehaviour {
	
	/* public vars */
	public KeyCode pauseKey = KeyCode.Escape;
	public bool paused;
	
	// Use this for initialization
	void Start () {
		paused = false;
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(pauseKey) ) {
			paused = !paused; // toggle paused state
		}
		
		
		if( paused ) {
			Time.timeScale = 0.0f;
		} else {
			Time.timeScale = 1.0f;
		}
		
	}
	
	
}
