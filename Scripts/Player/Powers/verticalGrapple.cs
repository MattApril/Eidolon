using UnityEngine;
using System.Collections;

public class verticalGrapple : MonoBehaviour {
	
	/* config vars */
	public int grappleSpeed = 7;
	public float stopDistance = 1.4f;
	public KeyCode inputKey = KeyCode.A;
	
	/* private vars */
	private bool inRange;
	private bool grappling;
	private bool movementDisabled;
	private Vector3 grappleDir;
	private Vector3 hookLocation;
	private LineRenderer lineRenderer;
	private float keyPressedPosition;
	
	private GameObject trail;
	
	/* event dispatchers */
	public delegate void onGrapple(bool disable);
	public static event onGrapple isGrappling;
	
	public delegate void onHang(bool hanging);
	public static event onHang isHanging;
	
	
	void OnEnable() {
		
		playerLife.disablePlayerMovement += disableMovement;
		
	}
	
	void OnDisable() {
		
		playerLife.disablePlayerMovement -= disableMovement;
		
	}
	
	// Use this for initialization
	void Start () {
		
		lineRenderer = this.gameObject.GetComponent<LineRenderer>();
		lineRenderer.SetWidth(GameConstants.trailWidth, GameConstants.trailWidth);
		lineRenderer.SetVertexCount(2);
		
		trail = GameObject.Find("trail");
	}
	
	// Update is called once per frame
	void Update () {
		
		// if entered collider
		if( inRange && !movementDisabled ) {
			
			if( Input.GetKey( inputKey ) ) {
				
				Vector3 verticalExtent = new Vector3( 0, collider.bounds.extents.y/2, 0 );
				lineRenderer.SetVertexCount(2);
				lineRenderer.SetPosition(0, transform.position +  verticalExtent );
				lineRenderer.SetPosition(1, hookLocation);
				
				// TODO: move this to fixedupdate
				// grapple is initiated, but movement not actually started yet
				if( this.rigidbody.useGravity ) {
					
					isGrappling(true);
					grappling = true;
					
					this.rigidbody.useGravity = false;
					
					trail.GetComponent<TrailRenderer>().time = 0.0f;
					lineRenderer.enabled = true;
					
					hookLocation.x = transform.position.x;
					
					grappleDir = Vector3.up;
					
					this.rigidbody.velocity = grappleDir * grappleSpeed;
					//this.rigidbody.AddForce(grappleDir * grappleSpeed);
				}
				
				// if we are currently grappling:
				if( !this.rigidbody.useGravity ) {
					
					//stop when we reach the hook
					if( transform.position.y >= hookLocation.y - stopDistance ) {
						this.rigidbody.velocity = Vector3.zero;
						isHanging( true );
						
					}
					
					//break grapple if rigidbody's horizontal velocity is non-zero
					if( rigidbody.velocity.x != 0 ) {
						breakGrapple();
					}
				}
				
				
			} else if( Input.GetKeyUp( inputKey ) ) {
				breakGrapple();
			}
			
			
		}
	}
	
	void breakGrapple() {
		this.rigidbody.useGravity = true;
		grappling = false;
		
		lineRenderer.SetVertexCount(0);
		lineRenderer.enabled = false;
		isGrappling(false);
		isHanging( false );
		
		trail.GetComponent<TrailRenderer>().time = 1.0f;
	}
	
	void disableMovement( bool disable ) {
		
		if(disable) {
			movementDisabled = true;
		} else {
			movementDisabled = false;
		}
	}
	
	void OnTriggerEnter( Collider obj ) {
		if(obj.tag == "verticalGrapple") {
			inRange = true;
			hookLocation = obj.transform.position;
		}
	}
	
	void OnTriggerStay( Collider obj ) {
		if(obj.tag == "verticalGrapple") {
			inRange = true;
		}
	}
	
	void OnTriggerExit( Collider obj ) {
		if(obj.tag == "verticalGrapple") {
		
			inRange = false;
			hookLocation = Vector3.zero;
		}
	}
}
