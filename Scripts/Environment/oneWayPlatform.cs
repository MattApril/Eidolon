using UnityEngine;
using System.Collections;

public class oneWayPlatform : MonoBehaviour {
	
	public delegate void playerCollision( bool playerMadeContact );
	public static event playerCollision onPlayerCollision;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	// create a manager script to track the number of intersecting collisions (add to PathPlatform.cs)
	// if it is > 0, ignore collisions
	// if < 0, enable collisions
	void OnTriggerEnter( Collider obj ) {
		
		if( obj.tag == "Player" ) {
			
			onPlayerCollision( true );
			
//			if( obj.rigidbody.velocity.normalized.y > 0 ) {
//				
//				Physics.IgnoreLayerCollision( 10, 11, true );
//			
//				//Transform platform = transform.parent;
//				
//				//Physics.IgnoreCollision( obj.collider, platform.collider );
//				
//			}
		}
			
	}
	
	void OnTriggerExit( Collider obj ) {
		
		if( obj.tag == "Player" ) {
			
			onPlayerCollision( false );
			
			//Transform platform = transform.parent;
			
			//Physics.IgnoreCollision( obj.collider, platform.collider, false );
			//Physics.IgnoreLayerCollision( 10, 11, false );
		}
			
	}
}
