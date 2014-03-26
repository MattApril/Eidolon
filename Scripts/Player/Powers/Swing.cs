using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Swing : MonoBehaviour {
	
	/* config vars */
	public float jumpForce = 30.0f;
	public float flipSpeed = 7.0f;
	public KeyCode inputKey = KeyCode.S;
	
	/* private vars */
	private bool inRange;
	private bool movementDisabled;
	private bool swinging;
	private bool levelOut;
	private Vector3 pivotLocation;
	private ArrayList pivotTracker;
	private bool swingMask;
	
	private LineRenderer lineRenderer;
	private GameObject swing;
	
	/* event dispatcher */
	public delegate void onSwing(bool swing);
	public static event onSwing swingState;
	
	public AudioClip soundEffect;
	bool soundPlayed;
	
	void OnEnable() {
		
		playerLife.disablePlayerMovement += disableMovement;
		
	}
	
	void OnDisable() {
		
		playerLife.disablePlayerMovement -= disableMovement;
		
	}
	
	void Start() {
		inRange = false;
		levelOut = false;
		swinging = false;
		soundPlayed = false;
		
		pivotTracker = new ArrayList();
		
		lineRenderer = this.gameObject.GetComponent<LineRenderer>();
		lineRenderer.SetWidth( GameConstants.trailWidth , GameConstants.trailWidth );
		
		
	}
	
	void Update() {
		
		//Debug.Log( transform.rotation.y );
		
		// while in range of swing pivot
		if( inRange && !movementDisabled ) {
			
			// start swing
			if( Input.GetKeyDown( inputKey ) ) {
				
				//Play sound effect
				if(!soundPlayed){
					audio.PlayOneShot(soundEffect);
					soundPlayed = true;	
				}
				
				// TODO: Fix falling off bug
				// // don't latch on unless we are far enough into the collider bounds.
				// // collider radius is modified by scale, so multiply them.
				// // make sure that players distance from pivot is significantly < collider radius.
//				Debug.Log("----");
//				Debug.Log( swing.GetComponent<SphereCollider>().radius * swing.transform.localScale.x  );
//				Debug.Log( (pivotLocation - transform.position).magnitude );
				
				float minDistance = 9999f;
				
				foreach( Vector3 pivot in pivotTracker ) {
					Vector3 difference = pivot - transform.position;
					if( difference.magnitude < minDistance ) {
						pivotLocation = pivot;
					}
				}
				
				// set players forward transform
				if( transform.position.x > pivotLocation.x ) {
					transform.forward = Vector3.forward * -1;	
				} else {
					transform.forward = Vector3.forward;
				}
				
				// rotate player to face swing pivot
				transform.Rotate(  new Vector3( 0, 0, ( 90.0f - Vector3.Angle( Vector3.up, (transform.position - pivotLocation).normalized ) ) * -1.0f)  );
				
				
				this.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX
											| RigidbodyConstraints.FreezeRotationY
											| RigidbodyConstraints.FreezePositionZ;
				// add + configure hinge
				this.gameObject.AddComponent<HingeJoint>();
				this.gameObject.hingeJoint.anchor = this.transform.InverseTransformPoint( pivotLocation ); // get pivot coordinates and convert to local space
				this.gameObject.hingeJoint.axis = Vector3.forward; // set hinge along Z axis
				
				lineRenderer.enabled = true;
				lineRenderer.SetVertexCount(2);
				swinging = true;
				
				// dispatch event to prevent keyboard movement
				swingState( true );
				
				levelOut = false;
			}
			
			if(swinging) {
				
				//Debug.Log( Vector3.Angle( Vector3.up, (transform.position - pivotLocation) ) );
				
				// Draw connecting line every frame
				lineRenderer.SetPosition( 0, transform.position + transform.up * 0.2f );
				lineRenderer.SetPosition( 1, pivotLocation );
				
				// unlatch from swing
				if( Input.GetKeyUp( inputKey ) ) {
					endSwing();
				}
				
				// jump from swing
				if( Input.GetKeyDown( KeyCode.Space ) ) {
					
					endSwing();
					
					// jump off swing
					this.rigidbody.AddForce(Vector3.up * jumpForce );
				}
			}
			
		}
		
		// regain balance
		if( levelOut ) {
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler( 0, transform.eulerAngles.y, 0 ), Time.deltaTime * flipSpeed );
			
			if( transform.rotation == Quaternion.Euler( 0, transform.eulerAngles.y, 0 )) {
				levelOut = false;
			}
			
		}
		
	}
	
	void endSwing() {
		
		soundPlayed = false;
		
		Destroy( GetComponent<HingeJoint>() );
		
		this.rigidbody.freezeRotation = true;
		
		// global angle when releasing from swing
		float releaseAngle = Vector3.Angle( Vector3.up, (transform.position - pivotLocation) );
				
		if( releaseAngle < 45.0f ) {
			Debug.Log( "level out" );
		}else if( releaseAngle >= 45.0f && releaseAngle <= 135.0f  ) {
			
			if( this.transform.rotation.eulerAngles.z >= 135.0f && this.transform.rotation.eulerAngles.z <= 225.0f ) {
				Debug.Log( "flip" );
			} else {
				Debug.Log( "level out" );
			}
			
		}else if( releaseAngle > 135.0f ) {
			Debug.Log( "level out" );
		} else {
			Debug.Log( "level out" );
		}
		
		levelOut = true;
		
		lineRenderer.SetVertexCount(0);
		lineRenderer.enabled = false;
		
		swingState( false );
		
		swinging = false;
		
		// only set to false if no other pivots in range
		if( pivotTracker.Count == 0 ) {
			inRange = false;
		}
	}
	
	void OnTriggerEnter( Collider obj ) {
		
		if( obj.tag == "swingMask" ) {
			swingMask = true;
		}
		
		if( obj.name == "swingPivot" && !swingMask )  {
			
			inRange = true;
			pivotTracker.Add( obj.transform.position );
		}
		
		
		
	}
	
	void disableMovement( bool disable ) {
		
		if(disable) {
			if( swinging ) {
				endSwing();
			}
			movementDisabled = true;
		} else {
			movementDisabled = false;
		}
	}
	
	void OnTriggerExit( Collider obj ) {
		
		if( obj.tag == "swingMask" ) {
			swingMask = false;	
		}
		
		if( obj.name == "swingPivot" && !swingMask ) {
			
			//swing = obj.gameObject;
			
			pivotTracker.Remove( obj.transform.position );
			
			if( !swinging && pivotTracker.Count == 0 ) {
				inRange = false;
			}
			
		}
		
	}
	
}