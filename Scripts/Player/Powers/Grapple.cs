using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour {
	
	/* public vars */
	public int grappleSpeed = 160;
	public float stopDistance = 0.4f;
	public KeyCode inputKey = KeyCode.A;
	
	/* private vars */
	private bool inRange;
	private bool movementDisabled;
	private bool keyPressed;
	private bool grapple;
	private Vector3 grappleDir;
	private Vector3 hookLocation;
	private LineRenderer lineRenderer;
	private float keyPressedPosition;
	private Vector3 requiredDirection;
	
	private GameObject trail;
	
	/* events */
	public delegate void onGrapple(bool disable);
	public static event onGrapple isGrappling;
	
	public delegate void onWallGrab( bool isGrabbing );
	public static event onWallGrab wallGrabbing;
	
	public AudioClip soundEffect;
	bool soundPlayed;
	
	void OnEnable() {
		
		playerLife.disablePlayerMovement += disableMovement;
		
	}
	
	void OnDisable() {
		
		playerLife.disablePlayerMovement -= disableMovement;
		
	}
	
	// Use this for initialization
	void Start () {
		soundPlayed = false;
		
		lineRenderer = this.gameObject.GetComponent<LineRenderer>();
		lineRenderer.SetWidth(GameConstants.trailWidth, GameConstants.trailWidth);
		
		trail = GameObject.Find("trail");
	}
	
	// Update is called once per frame
	void Update () {
		
		// if entered collider
		if( inRange && !movementDisabled ) {
			
			// check if key is initially pressed in time.
			if( Input.GetKeyDown( inputKey ) ) {
				
				
				if( transform.position.x < hookLocation.x ) {
					grappleDir = Vector3.right;
					requiredDirection = new Vector3( 0, 0, 1 );
					
				} else if( transform.position.x > hookLocation.x ) {
					grappleDir = Vector3.left;	
					requiredDirection = new Vector3( 0, 0, -1 );
					
				}
				
				if( transform.forward == requiredDirection  ) {
					
					keyPressedPosition = transform.position.y;
					keyPressed = true;
					
					bool grounded = this.gameObject.GetComponent<PlayerControls>().grounded;
					
					if(grounded) {
						
						if( transform.position.y >= hookLocation.y - 0.1f && transform.position.y <= hookLocation.y + 0.1f ) {
							grapple = true;
							keyPressed = false;
						}
					}
					
					lineRenderer.enabled = true;
					lineRenderer.SetVertexCount(2);
					
				}
				
			}
			
			//TODO: Fix
			// if button was pressed, and is still being pressed
			if( keyPressed && Input.GetKey( inputKey ) ) {
				
				// if key was pressed above the hook
				if( keyPressedPosition >= hookLocation.y ) {
				
					// needs to drop below or be equal to hook pos
					if( transform.position.y <= hookLocation.y ) {
						grapple = true;
						keyPressed = false;
					}
				}
				
				// if key was pressed below the hook
				if( keyPressedPosition < hookLocation.y ) {
					// needs to pass  above or be equal to hook pos
					if( transform.position.y >= hookLocation.y ) {
						grapple = true;
						keyPressed = false;
					}
				}
				
			}
			
			//TODO: Disable movement controls
			if( grapple ) {
			
				if( Input.GetKey( inputKey ) ) {
					
					if(!soundPlayed){
						audio.PlayOneShot(soundEffect);
						soundPlayed = true;
					}
					
					lineRenderer.SetPosition(0, transform.position + (transform.up * 0.05f) );
					lineRenderer.SetPosition(1, hookLocation);
					
					// TODO: move this to fixedupdate
					// grapple is initiated, but movement not actually started yet
					if( this.rigidbody.useGravity ) {
						
						this.rigidbody.useGravity = false;
						isGrappling( true );
						trail.GetComponent<TrailRenderer>().time = 0.0f;
						
						if( transform.position.x < hookLocation.x ) {
							grappleDir = Vector3.right;
						} else if( transform.position.x > hookLocation.x ) {
							grappleDir = Vector3.left;	
						}
						
						this.rigidbody.velocity = grappleDir * 7;
						//this.rigidbody.velocity = Vector3.zero;
						//this.rigidbody.AddForce(grappleDir * grappleSpeed);
					}
					
					// if we are currently grappling:
					if( !this.rigidbody.useGravity ) {
						
						//stop when we reach the hook
						if( transform.position.x >= hookLocation.x - stopDistance && transform.position.x <= hookLocation.x + stopDistance ) {
							this.rigidbody.velocity = Vector3.zero;
							wallGrabbing( true );
						}
						
						//break grapple if rigidbody's vertical velocity is non-zero
						if( rigidbody.velocity.y != 0 ) {
							breakGrapple();	
						}
					}
					
					
				} else if( Input.GetKeyUp( inputKey ) ) {
					breakGrapple();
				}
				
			}
			
			
		}
	}
	
	void breakGrapple() {
		soundPlayed = false;
		
		grapple = false;
		this.rigidbody.useGravity = true;
		
		trail.GetComponent<TrailRenderer>().time = 1.0f;
		
		lineRenderer.SetVertexCount(0);
		lineRenderer.enabled = false;
		
		isGrappling( false );
		wallGrabbing( false );
	}
	
	void disableMovement( bool disable ) {
		
		if(disable) {
			if(grapple) {
				breakGrapple();
			}
			movementDisabled = true;
		} else {
			movementDisabled = false;
		}
	}
	
	void OnTriggerEnter( Collider obj ) {
		if(obj.tag == "horizontalGrapple") {
			inRange = true;
			keyPressed = false;
			hookLocation = obj.transform.position;
		}
	}
	
	void OnTriggerStay( Collider obj ) {
		if(obj.tag == "horizontalGrapple") {
			inRange = true;
		}
	}
	
	void OnTriggerExit( Collider obj ) {
		if(obj.tag == "horizontalGrapple") {
		
			inRange = false;
			hookLocation = Vector3.zero;
		}
	}
}
