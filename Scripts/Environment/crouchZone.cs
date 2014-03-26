using UnityEngine;
using System.Collections;

public class crouchZone : MonoBehaviour {
	
	public delegate void crouchZn( bool inZone );
	public static event crouchZn onCrouchZone;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter( Collider obj ) {
		// if player collides
		if( obj.tag == "Player" ) {
			onCrouchZone( true );
		}
	
	}
	
	void OnTriggerExit( Collider obj ) {
		// if player collides
		if( obj.tag == "Player" ) {
			onCrouchZone( false );
		}
	
	}
}
