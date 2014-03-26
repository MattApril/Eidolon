using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {
	//Audio
	public AudioClip jumpSound;
	
	
	/* Key States */
	private int currentDirKey;
	private const int NONE = 0;
	private const int LEFT = 1;
	private const int RIGHT = 2;
	
	/* public config vars */
	public float moveSpeed = 3.0f;
	public float groundedRayLength = 0.55f;
	public float wallStopDistance = 0.35f;
	public float wallJumpRange = 0.5f;
	public float wallJumpForce = 6.0f;
	public float maxWallStickTime = 1.5f;
	public float stopJumpMoveTime = 0.45f;
	public int ladderJumpForce = 50;
	public Vector3 ladderJumpDir = new Vector3(2, 1, 0);
	
	/* private vars */
	public bool grounded { get; private set; }
	private float defaultMoveSpeed;
	private bool left;
	private bool right;
	private bool stopMove;
	private bool jump;
	private bool rightWalljump;
	private bool leftWalljump;
	private bool climbing;
	private bool upLadder;
	private bool downLadder;
	private float ladderTop;
	private float ladderCenter;
	private float colliderHeight;
	private GameObject trail;
	private Vector3 trailDefaultPos;
	private bool crouch;
	private bool inCrouchZone;
	private bool dead;
	private float doorCooldown;
	
	////////////////////////////ADAM CHANGES TO CODE//////////////////////////////
	//This allows the user to have some leeway when jumping
	/////////////////////////////////////////////////////////////////////////////
	private float jumpTimer;
	private bool bIsJumping=false;
	private bool bForgivenessJump=false;
	
	/* private walljump vars */
	private bool R;
	private bool L;
	private bool TR; // top right ray
	private bool BR; // bottom right ray
	private bool TL;
	private bool BL;
	private bool rightWallContact; // enable when touching a wall, to prevent sticking (delay enabling for creating the wall grab)
	private bool leftWallContact;
	private bool rightWalljumpContact;
	private bool leftWalljumpContact;
	private int wallID;
	private int previousWallID;
	private int wallGrabID;
	private int previousWallGrabID;
	private RaycastHit leftHit = new RaycastHit();
	private RaycastHit rightHit = new RaycastHit();
	private float stickTime;
	private bool disableLeft; // prevent left key input, mainly used for wall jump
	private bool disableRight; // prevent right key input, mainly used for wall jump
	private float disableTimer;
	private bool timedDisable;
	
	private bool movementDisabled;
	
	LayerMask groundLayerMask;
	LayerMask wallLayerMask;
	
	protected Animator animator;
	
	// Events
	public delegate void onGrounded( bool isGrounded );
	public static event onGrounded _grounded;
	
	public delegate void onMove( bool isGrounded );
	public static event onMove moving;
	
	public delegate void onJump();
	public static event onJump jumping;
	
	public delegate void onCrouch( bool isCrouching );
	public static event onCrouch crouching;
	
	public delegate void onWalljump();
	public static event onWalljump wallJumping;
	
	public delegate void onClimb( bool isClimbing );
	public static event onClimb ladder;
	
	public delegate void onLadderMove( bool isClimbing );
	public static event onLadderMove climbingLadder;
	
	public delegate void onLadderJump( bool isJumping );
	public static event onLadderJump ladderJump;
	
	public delegate void onWallGrab( bool isGrabbing );
	public static event onWallGrab wallGrabbing;
	
	public delegate void onTurn( bool turning );
	public static event onTurn isTurning;
	
	void OnEnable() {
		
		// add power event handlers
		verticalGrapple.isGrappling += disableMovement;
		Grapple.isGrappling += disableMovement;
		Swing.swingState += disableMovement;
		
		crouchZone.onCrouchZone += handleCrouchZone;
		
		playerLife.disablePlayerMovement += disableMovement;
		playerLife.dead += onPlayerDeath;
		
	}
	
	void OnDisable() {
		
		// remove event handlers
		verticalGrapple.isGrappling -= disableMovement;
		Grapple.isGrappling -= disableMovement;
		Swing.swingState -= disableMovement;
		
		crouchZone.onCrouchZone -= handleCrouchZone;
		
		playerLife.disablePlayerMovement -= disableMovement;
		playerLife.dead -= onPlayerDeath;
		
	}

	// initialization
	void Start () {
		
		initVars();
		
		ladderJumpDir.Normalize();
		
		colliderHeight = GetComponent<CapsuleCollider>().height;
		
		trail = GameObject.Find("trail");
		trailDefaultPos = trail.transform.localPosition;
		defaultMoveSpeed = moveSpeed;
		
		//layermasks
		LayerMask ground = 1 << 8;
		LayerMask path =  1 << 11;
		groundLayerMask = ground | path; //'level' layer
		wallLayerMask = 1 << 8;
		
		animator = GetComponent<Animator>();
		
		dead = false;
		
		doorCooldown = 0f;
	}
	
	void initVars() {
		
		this.rigidbody.freezeRotation = true;
		
		currentDirKey = NONE;
		left = false;
		right = false;
		stopMove = false;
		jump = false;
		climbing = false;
		upLadder = false;
		downLadder = false;
		
		movementDisabled = false;
		
		disableTimer = stopJumpMoveTime;
	}
	
	
	
	// Update is called once per frame
	void Update() {
		
		// Lock Z-axis
		Vector3 forceZ = transform.localPosition;
		forceZ.z = 0f;
		transform.localPosition = forceZ;
		
		if( dead ) {
			return;	
		}
		
		doorCooldown -= Time.deltaTime;
		
		Debug.DrawRay(transform.position, Vector3.down * groundedRayLength );
		
		
		//////////////////////////// START ADAM CHANGES TO CODE//////////////////////////////
		//Added this code to allow split edge jumping///////////////////////////////////////
		
		
		
		if(bIsJumping){
			
			jumpTimer+=Time.deltaTime;
			
			// The 0.5344666f is the amount of time the player is forgiven to jump 
			if(jumpTimer < 0.5344666f ){
				bForgivenessJump=true;
			}else{
				bForgivenessJump=false;
			}
			
		}
	
		
		
		Vector3 rightV,leftV;
		bool bRightV,bLeftV;
		bRightV=bLeftV=false;
		rightV = Vector3.down;
		leftV = Vector3.down;
		//if(transform.rotation.
		if(transform.rotation.w==1.0f){
			bRightV = Physics.Raycast(transform.position,new Vector3(rightV.x- 0.4f,rightV.y,rightV.z),groundedRayLength,groundLayerMask);
			Debug.DrawRay( transform.position, new Vector3(leftV.x- 0.4f,leftV.y,leftV.z)* groundedRayLength,new Color(124,256,0)  );
		}else if(transform.rotation.y==1.0f){
			bLeftV = Physics.Raycast(transform.position,new Vector3(leftV.x+ 0.4f,leftV.y,leftV.z),groundedRayLength,groundLayerMask);
			Debug.DrawRay( transform.position, new Vector3(leftV.x+ 0.4f,leftV.y,leftV.z)* groundedRayLength,new Color(124,256,0)  );
		}
		
		////////////////////////////END ADAM CHANGES TO CODE//////////////////////////////
		
		
		
		/**Grounding**/
		if( Physics.Raycast( transform.position, Vector3.down, groundedRayLength, groundLayerMask ) ) {
			
			// handle crouching with jumping/falling
			if( !grounded ) {
				if( Input.GetKey(KeyCode.DownArrow) ) {
					doCrouch( true );
				}else{
					bIsJumping=true;	
				}
			}
			
			// clear former walljump/wallgrab ID's
			if( wallID != 0 ) {
				previousWallID = 0;
			}
			
			if( wallGrabID != 0 ) {
				previousWallGrabID = 0;
			}
			
			if( disableLeft ) {
				disableLeft = false;	
			}
			
			if( disableRight ) {
				disableRight = false;	
			}
			
			grounded = true;
			
			if( animator.GetBool("Grounded") == false ) {
				_grounded( true );
			}
			
		} else {
			
			// handle crouching with jumping/falling
			if( grounded ) {
				doCrouch( false );
			}
			
			grounded = false;
			
			if( animator.GetBool("Grounded") == true ) {
				_grounded( false );
			}
		}
		
		if( !crouch ) {
			
			Debug.DrawRay( transform.position, ( (Vector3.right * 0.6f) + Vector3.up).normalized * wallStopDistance );
			Debug.DrawRay( transform.position, ( (Vector3.right * 0.6f) + Vector3.down).normalized * wallStopDistance );
			
			
			Debug.DrawRay( transform.position, ( (Vector3.left * 0.6f) + Vector3.up).normalized * wallStopDistance );
			Debug.DrawRay( transform.position, ( (Vector3.left * 0.6f) + Vector3.down).normalized * wallStopDistance );
			
			// wall ray casts
			
			TR = Physics.Raycast( transform.position, ( (Vector3.right * 0.6f) + Vector3.up ).normalized, out rightHit, wallStopDistance, wallLayerMask );
			BR = Physics.Raycast( transform.position, ( (Vector3.right * 0.6f) + Vector3.down ).normalized, wallStopDistance, wallLayerMask );
			TL = Physics.Raycast( transform.position, ( (Vector3.left * 0.6f) + Vector3.up ).normalized, out leftHit, wallStopDistance, wallLayerMask );
			BL = Physics.Raycast( transform.position, ( (Vector3.left * 0.6f) + Vector3.down ).normalized, wallStopDistance, wallLayerMask );
			
			/** Right Wall Stop **/
			if( TR && BR ) {
				
				wallGrabID = rightHit.collider.gameObject.GetInstanceID();
				
				// if player is holding key, stick to wall for X seconds
				if( rightWallContact == false && stickTime < maxWallStickTime && Input.GetKey(KeyCode.RightArrow) && wallGrabID != previousWallGrabID && !grounded ) {
					
					if( !animator.GetBool("WallGrabbing") ) {
						//Debug.Log("Right Wall Grab ON");
						wallGrabbing( true );
					}
					
					rightWallContact = false;
					
					stickTime += Time.deltaTime;
					
				} else {
					
					if( animator.GetBool("WallGrabbing") ) {
						//Debug.Log("Right Wall Grab OFF");
						wallGrabbing( false );
					}
					rightWallContact = true;
					
					previousWallGrabID = wallGrabID;
					stickTime = 0.0f;
				}
				
			} else if( TR || BR ) {
				// only one ray hitting, do not wall grab (disable input)
				
				if( animator.GetBool("WallGrabbing") ) {
					//Debug.Log("Right Wall Grab OFF");
					wallGrabbing( false );
				}
				
				// disable right key
				rightWallContact = true;
			} else {
				
				// do nothing
				rightWallContact = false;
			}
			
			/** Left Wall Stop **/
			if( TL && BL ) {
				
				wallGrabID = leftHit.collider.gameObject.GetInstanceID();
				
				// if player is holding key, stick to wall for X seconds
				if( leftWallContact == false && stickTime < maxWallStickTime && Input.GetKey(KeyCode.LeftArrow) && wallGrabID != previousWallGrabID && !grounded ) {
					
					if( !animator.GetBool("WallGrabbing") ) {
						//Debug.Log("Left Wall Grab ON");
						wallGrabbing( true );
					}
					
					leftWallContact = false;
					
					stickTime += Time.deltaTime;
				} else {
					
					if( animator.GetBool("WallGrabbing") ) {
						//Debug.Log("Left Wall Grab OFF");
						wallGrabbing( false );
					}
					
					leftWallContact = true;
					
					previousWallGrabID = wallGrabID;
					stickTime = 0.0f;
				}
			} else if ( TL || BL ) {
				// only one ray hitting, do not wall grab (disable input)
				
				if( animator.GetBool("WallGrabbing") ) {
					//Debug.Log("Left Wall Grab OFF");
					wallGrabbing( false );
				}
				// disable left key
				leftWallContact = true;
			} else {
				// do nothing
				leftWallContact = false;
	 		}
			
			
//			Debug.DrawRay( transform.position, Vector3.right * wallJumpRange );
//			Debug.DrawRay( transform.position, Vector3.left * wallJumpRange );
			
			RaycastHit hit;
			/**Wall Jump**/
			if( Physics.Raycast( transform.position, Vector3.right, out hit, wallJumpRange, wallLayerMask ) ) {
				
				// if entering a wall collider, set current wallID
				if(rightWalljumpContact == false) {
					wallID = hit.collider.gameObject.GetInstanceID();
				}
				
				rightWalljumpContact = true;
				
				
			} else {
				
				rightWalljumpContact = false;
			}
			
			if( Physics.Raycast( transform.position, Vector3.left, out hit, wallJumpRange, wallLayerMask ) ) {
				
				// if entering a wall collider, set current wallID
				if(leftWalljumpContact == false) {
					wallID = hit.collider.gameObject.GetInstanceID();
				}
				
				leftWalljumpContact = true;
				
			} else {
				leftWalljumpContact = false;
			}
			
		}
		
		// check input for use during FixedUpdate()
		
		// CLIMBING JUMP
		if( climbing ) {
			
			if( Input.GetKeyDown( KeyCode.Space ) ) {
				stopClimbing( true );
			}
			
			if(pollButtons.jump) {
				stopClimbing( true );
			}
		
		// JUMP
			
			
		////////////////////////////ADAM CHANGES TO CODE//////////////////////////////
		//Added the bRightV, and bLeftV to allow split edge jumping
		////////////////////////////ADAM CHANGES TO CODE//////////////////////////////
		
		// conditions:
		// --------------
		// grounded == true;
		// forgiving rays == true, and wall grabbing rays == false
		// forgivness timer == true
		} else if( grounded || ( ( bRightV && (!TL || !BL) ) || (bLeftV && (!TR || !BR) ) || bForgivenessJump  ) ) {
			
			if( Input.GetKeyDown( KeyCode.Space ) && !crouch ) {
				audio.PlayOneShot(jumpSound);
				
				jump = true;
				bIsJumping=false;
				jumpTimer = 0;
				bForgivenessJump=false;
			}
			
			if(pollButtons.jump) {
				jump = true;
			}
		
		// WALL JUMP
		} else if( rightWalljumpContact ) {
			
			if( Input.GetKeyDown( KeyCode.Space ) && transform.forward == Vector3.forward ) {
				if( wallID != previousWallID ) {
					
					wallJumping();
					
					audio.PlayOneShot(jumpSound);
					
					previousWallGrabID = wallGrabID;
					
					// disable animation
					if( animator.GetBool("WallGrabbing") ) {
						wallGrabbing( false );
					}
					
					rightWalljump = true;
					disableRight = true;
					timedDisable = true;
					
					previousWallID = wallID;
		}
			}
			if(pollButtons.jump) {
				//TODO: Fix for controller
				rightWalljump = true;
			}
		
		// WALL JUMP
		} else if( leftWalljumpContact ) {
			
			
			// if spacebar pressed, while facing right direction
			if( Input.GetKeyDown( KeyCode.Space ) && transform.forward == Vector3.forward * -1 ) {
				if( wallID != previousWallID ) {
					
					wallJumping();
					
					audio.PlayOneShot(jumpSound);
					
					previousWallGrabID = wallGrabID;
					
					// disable animation
					if( animator.GetBool("WallGrabbing") ) {
						wallGrabbing( false );
					}
					
					leftWalljump = true;
					disableLeft = true;
					timedDisable = true;
					
					previousWallID = wallID;
				}
			}
		}
		
		/* Fancy Wall Jump delay */
		if( disableLeft ) {
			
			if( timedDisable ) {
				disableTimer -= Time.deltaTime;
				// re-enable left key after timer runs out of user lets go of key
				if( disableTimer <= 0 ) {
					disableLeft = false;
					timedDisable = false;
					disableTimer = stopJumpMoveTime;
				}
			}
			
			if( Input.GetKeyDown( KeyCode.LeftArrow ) ) {
				disableLeft = false;
			}
			
			
		}
		
		/* Fancy Wall Jump delay */
		if( disableRight ) {
			
			if( timedDisable ) {
				disableTimer -= Time.deltaTime;
				// re-enable left key after timer runs out of user lets go of key
				if( disableTimer <= 0 ) {
					disableRight = false;
					timedDisable = false;
					disableTimer = stopJumpMoveTime;
				}
			}
			
			if( Input.GetKeyDown( KeyCode.RightArrow ) ) {
				disableRight = false;
			}
			
			
		}
		
		// LEFT
		if( Input.GetKey( KeyCode.LeftArrow ) ) {
			
			if( currentDirKey == NONE ) {
				currentDirKey = LEFT;
			}
			
			if( !(currentDirKey == LEFT) || leftWallContact || disableLeft ) {
				left = false;
			} else {
				left = true;
			}
			
		} else {
			left = false;
		}
		
		if( Input.GetKeyUp( KeyCode.LeftArrow ) ) {
			stopMove = true;
			
			if( currentDirKey == LEFT ) {
				currentDirKey = NONE;	
			}
		}
		
		// RIGHT
		if( Input.GetKey( KeyCode.RightArrow )  ) {
			
			if( currentDirKey == NONE ) {
				currentDirKey = RIGHT;
			}
			
			if( !(currentDirKey == RIGHT) || rightWallContact || disableRight ) {
				right = false;
			} else {
				right = true;
			}
			
		} else {
			right = false;
		}
		if( Input.GetKeyUp( KeyCode.RightArrow )  ) {
			stopMove = true;
			
			if( currentDirKey == RIGHT ) {
				currentDirKey = NONE;	
			}
		}
		
		// LADDER MOVEMENT
		if( climbing ) {
			if( Input.GetKey( KeyCode.UpArrow ) ) {
				upLadder = true;
				
			} else {
				upLadder = false;
			}
	
			if( Input.GetKey( KeyCode.DownArrow ) ) {
				downLadder = true;
	
			} else {
				downLadder = false;
			}
		}
		/* Crouching */
		// Still need to make the controller do it
		if( Input.GetKeyDown( KeyCode.DownArrow ) ) {
		
			if( grounded && !crouch ) {
				doCrouch( true );
			}
			
		} else if(Input.GetKeyUp(KeyCode.DownArrow)) {
			if( crouch && !inCrouchZone ) {
				doCrouch( false );
			
			}
		}
		
	}
	
	
	// Toggle crouch
	void doCrouch( bool isCrouching ) {
		
		CapsuleCollider collider = GetComponent<CapsuleCollider>();
			if( isCrouching ) {
				
				collider.height = colliderHeight * 0.5f;
				collider.center = new Vector3( 0, -0.08f, 0 );
				trail.transform.Translate(0f, -0.25f, 0f );
				moveSpeed = defaultMoveSpeed * 0.5f;
				
				rightWallContact = false;
				leftWallContact = false;
				
				crouching( true );
				crouch = true;
				
			} else {
				
				collider.height = colliderHeight;
				collider.center = Vector3.zero;
				trail.transform.localPosition = trailDefaultPos;
				moveSpeed = defaultMoveSpeed;
				crouching( false );
				crouch = false;
				
			}
		
	}
		
	// event handler
	// enable/disable movement input
	void disableMovement( bool disable ) {
		
		Debug.Log(disable);
		
		if(disable) {
			movementDisabled = true;
		} else {
			movementDisabled = false;
		}
	}
	
	void handleCrouchZone( bool inZone ) {
		inCrouchZone = inZone;
		
		// if exiting crouchZone and user is not holding crouch,crouching = false
		if( inZone == false && !Input.GetKey( KeyCode.DownArrow ) ) {
			doCrouch( false );
		}
	}
	
	void startClimbing() {
		
		Vector3 centerOffset = new Vector3();
		centerOffset.x = ( transform.position.x - ladderCenter ) * -1.0f;
		
		Debug.Log( centerOffset );
		
		Hashtable transition = new Hashtable();
			transition.Add("amount", centerOffset );
			transition.Add("time", 0.5f);
			transition.Add("easetype", "easeOutQuart" );
			
		iTween.MoveBy(this.gameObject.transform.parent.gameObject, transition );
		
		climbing = true;
		ladder( true );
		this.rigidbody.velocity = Vector3.zero;
		rigidbody.isKinematic = true;
		
		StartCoroutine( getOnLadder() );
		
	}
	
	void stopClimbing( bool jumping ) {
		climbing = false;
		ladder( false );
		
		getOffLadder();
		
		this.rigidbody.isKinematic = false;
		
		// lader jump
		if( jumping ) {
			
			if( Input.GetKey( KeyCode.RightArrow ) ) {
				// add right force, disable right key
				this.rigidbody.AddForce( ladderJumpDir * ladderJumpForce );
				disableRight = true;
				
				ladderJump( true );
			} else if ( Input.GetKey(KeyCode.LeftArrow) ) {
				// add left force, disable left key
				Vector3 leftDir = ladderJumpDir;
				leftDir.x = ladderJumpDir.x * -1.0f;
				this.rigidbody.AddForce( leftDir * ladderJumpForce );
				disableLeft = true;
				
				ladderJump( true );
			} else {
				// add small force to initiate movement
				this.rigidbody.AddForce(Vector3.up * 0.1f);
			}
		
		
		}
		
	}
	
	public bool isClimbing() {
		return climbing;
	}
	
	public bool isGrounded() {
		return grounded;
	}
	
	IEnumerator getOnLadder() {
		
		float initYRotation = transform.rotation.eulerAngles.y;
		bool targetRotation = false;
		
		// while not reached target rotation
		while( climbing && !targetRotation	) {
			
			// apply rotation
			transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.Euler( 0, 270, 0 ), Time.deltaTime * 10.0f );
			
			// have we reach target rotation yet?
			Debug.Log (initYRotation );
			
			if( initYRotation < 270.0f ) {
				if( transform.rotation.eulerAngles.y == 270.0f ) {
					targetRotation = true;
					Debug.Log("Ladder Rotation Complete");
				}
			} else if( initYRotation < 270.0f || initYRotation == 0f ) {
				if( transform.rotation.eulerAngles.y == 270.0f ) {
					targetRotation = true;
					Debug.Log("Ladder Rotation Complete");
				}
			}
			
			yield return null;
		}
		
	}
	
	void getOffLadder() {
		
		// set orientation
		if( Input.GetKey( KeyCode.RightArrow ) ) {
			transform.rotation = Quaternion.Euler( 0, 0, 0 );
		} else if( Input.GetKey( KeyCode.LeftArrow ) ) {
			transform.rotation = Quaternion.Euler( 0, 180, 0 );	
		}
	}
	
	// physics loop
	void FixedUpdate() {
		
		if( !movementDisabled ) {
			
			// JUMP
			if( jump ) {
				
				jumping();
				
				rigidbody.velocity = Vector3.up*6;
				
				pollButtons.jump = false;
				jump = false;
				pollButtons.jumping = true;
			}
			
			if( rightWalljump ) {
				rigidbody.velocity = ( ( Vector3.up * 2.5f ) + Vector3.left ).normalized * wallJumpForce;
				rightWalljump = false;
			}
			
			if( leftWalljump ) {
				rigidbody.velocity = ( ( Vector3.up * 2.5f ) + Vector3.right ).normalized * wallJumpForce;
				leftWalljump = false;
			}
			
			
			// Left & Right movement, make sure we are not on a ladder
			if( !climbing ) {
				
				// senstive input for joysticks, but causes a poor/rough wall jump (only tested with keyboard)
				// // direction changes before the smoothed input has fully changed
				// // i.e. left = true, but Input.Horizontal is still a positive value
//				if( left ) {
//					rigidbody.velocity = new Vector3( Input.GetAxis("Horizontal") * 3, rigidbody.velocity.y, 0 );
//				}
//				if( right ) {
//					rigidbody.velocity = new Vector3( Input.GetAxis("Horizontal") * 3, rigidbody.velocity.y, 0 );
//				}
				
				if( left ) {
					
					// animation event
					if( animator.GetBool("Moving") == false ) {
						moving( true );
					}
					
					// Get input direction
					Vector3 fwd = Vector3.Normalize(  new Vector3( 0f, 0f, Input.GetAxis("Horizontal") )  );
					
					// if input direction differs from transform dir, change it
					if( transform.forward != fwd ) {
						isTurning( true );
						transform.forward = fwd;
					} else {
						isTurning( false );	
					}
					
					rigidbody.velocity = new Vector3( -moveSpeed, rigidbody.velocity.y, 0 );
				}
				
				if( right ) {
					
					// animation event
					if( animator.GetBool("Moving") == false ) {
						moving( true );
					}
					
					// Get input direction
					Vector3 fwd = Vector3.Normalize(  new Vector3( 0f, 0f, Input.GetAxis("Horizontal") )  );
					
					// if input direction differs from transform dir, change it
					if( transform.forward != fwd ) {
						isTurning( true );
						transform.forward = fwd;
					} else {
						isTurning( false );
					}
					
					rigidbody.velocity = new Vector3( moveSpeed, rigidbody.velocity.y, 0 );
				}
				
				
				/************************/
					
				if( stopMove ) {
					// animation event
					moving( false );
					
					rigidbody.velocity = new Vector3( 0, rigidbody.velocity.y, 0 );
					stopMove = false;
				}
			
			}
			
			if( climbing ) {
				if( upLadder ) {
				
					if( transform.position.y < ladderTop ) {
						climbingLadder( true );
						transform.Translate( Vector3.up * moveSpeed * Time.deltaTime );
					} else {
						climbingLadder( false );
					}
				} else if( downLadder ) {
					if(grounded) {
						climbingLadder( false );
						stopClimbing( false );
					} else {
						climbingLadder( true );
						transform.Translate( Vector3.down * moveSpeed * Time.deltaTime * 0.75f );
					}
				} else {
					climbingLadder( false );
				}
			}
			
//			if( upLadder && climbing ) {
//				
//				if( transform.position.y < ladderTop ) {
//					climbingLadder( true );
//					transform.Translate( Vector3.up * moveSpeed * Time.deltaTime );
//				} else {
//					climbingLadder( false );
//				}
//			}
//		
//			if( downLadder && climbing ) {
//				if(grounded) {
//					climbingLadder( false );
//					stopClimbing( false );
//				} else {
//					climbingLadder( true );
//					transform.Translate( Vector3.down * moveSpeed * Time.deltaTime );
//				}
//					
//			}
			
		}
	}
	
	// latch on to ladders
	void OnTriggerStay( Collider colliderObj ) {
		
		float ladderCenterLeeway = 0.15f;
		
		// laders
		if( colliderObj.tag == "Ladder" ) {
			if( !climbing ) {
				
				if(  Mathf.Abs( transform.position.x - colliderObj.transform.position.x ) <= ladderCenterLeeway  ) {
					
					ladderCenter = colliderObj.transform.position.x;
					
					if( Input.GetKey( KeyCode.UpArrow ) ) {
						ladderTop = colliderObj.collider.bounds.center.y + colliderObj.collider.bounds.extents.y - (colliderHeight * transform.localScale.y)/2.0f;
						startClimbing();
					} else if ( Input.GetKey( KeyCode.DownArrow ) && !grounded ) {
						ladderTop = colliderObj.collider.bounds.center.y + colliderObj.collider.bounds.extents.y - (colliderHeight * transform.localScale.y)/2.0f;
						startClimbing();
					}
					
				}
				
			}
		}
		
		//Open Doors
		if(colliderObj.tag == "Door")
		{
			if(doorCooldown <= 0 )
			{
				if( Input.GetKey( KeyCode.UpArrow ) ){
					colliderObj.gameObject.GetComponent<Door>().Open(gameObject);
					doorCooldown = 3f;
				}
			}
		}
		
		//Push Levers
		if(colliderObj.tag == "Lever")
		{
			if( Input.GetKey( KeyCode.UpArrow ) ){
				colliderObj.gameObject.GetComponent<Lever>().Pull();
			}			
		}
	}
	
	// OLD
	void OnTriggerExit( Collider colliderObj ) {
		if( colliderObj.tag == "Ladder" ) {
			if( climbing ) {
				
			}
		}
	}
	
	public bool IsMovingRight()
	{
		return right;	
	}
	
	public bool IsMovingLeft()
	{
		return left;	
	}
	
	void onPlayerDeath() {
		initVars();
		dead = true;
	}
}
