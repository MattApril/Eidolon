using UnityEngine;
using System.Collections;

// checkpoint event dispatcher
public class checkpoint : MonoBehaviour {

	public delegate void newCheckpoint( bool playerMadeContact, Vector3 pos );
	public static event newCheckpoint onNewCheckpoint;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnTriggerEnter(Collider obj) {
		// if player collides
		if( obj.tag == "Player" ) {
			onNewCheckpoint( true, this.transform.position );
			Destroy( this.gameObject );
		}
	
	}
}
