using UnityEngine;
using System.Collections;

public class MovingPlat : MonoBehaviour {
	
	public float speed = 0.005f;
	public Transform pointA;
	public Transform pointB;
	
	private float xPos;
	private float yPos;
	private float zPos;
	private Vector3 moveDir;
	private Vector3 prevPos;
	private bool hitPathPlatform;
	
	public bool activated = true;
	
	
	// Use this for initialization
	void Start () {
		xPos = pointA.position.x;
		yPos = pointA.position.y;
		zPos = pointA.position.z;
		
		moveDir = getMoveDir( pointB.position, pointA.position );
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(activated){
			Vector3 translation = moveDir * speed;
			
			prevPos = transform.position;
			
			this.rigidbody.MovePosition( transform.position + translation  );
			
			if( moveDir == Vector3.right ) {
				
				// if patroller is passing the next path point
				if( transform.position.x >= pointB.transform.position.x  || hitPathPlatform ) {
					this.rigidbody.MovePosition( prevPos );
					moveDir = getMoveDir( pointA.position, pointB.position );
					hitPathPlatform = false;
				}
				
			} else if( moveDir == Vector3.left ) {
				
				// if patroller is passing the next path point
				if( transform.position.x <= pointA.transform.position.x || hitPathPlatform ) {
					this.rigidbody.MovePosition( prevPos );
					moveDir = getMoveDir( pointB.position, pointA.position );
					hitPathPlatform = false;
				}
				
			} else if( moveDir == Vector3.up ) {
				
				// if patroller is passing the next path point
				if( transform.position.y >= pointB.position.y || hitPathPlatform ) {
					this.rigidbody.MovePosition( prevPos );
					moveDir = getMoveDir( pointA.position, pointB.position );
					hitPathPlatform = false;
				}
				
			} else if( moveDir == Vector3.down ) {
				
				// if patroller is passing the next path point
				if( transform.position.y <= pointA.position.y || hitPathPlatform ) {
					this.rigidbody.MovePosition( prevPos );
					moveDir = getMoveDir( pointB.position, pointA.position );
					hitPathPlatform = false;
				}
				
			}
		}
	}
	
	Vector3 getMoveDir( Vector3 pointFrom, Vector3 pointTo ) {
		Vector3 dir = pointFrom - pointTo;
		return dir.normalized;
	}
	
	void OnTriggerEnter( Collider obj ) {
//		if( obj.gameObject.layer == LayerMask.NameToLayer( "PathPlatform" ) ) {
//			hitPathPlatform = true;
//		}
		
//		if( obj.tag == "Player" ) {
//			obj.transform.parent = this.transform;
//		}
	}
	
	void OnTriggerExit( Collider obj ) {
		
//		if( obj.tag == "Player" ) {
//			obj.transform.parent = null;
//		}
	}
}
