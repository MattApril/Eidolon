/** Animation Paramter Controls (Logan) **/

using UnityEngine;
using System.Collections;

public class LoganControl : MonoBehaviour {
	
	// Load components
	protected Animator animator;
	protected PlayerControls player;
	
	private int wallJumpState = Animator.StringToHash("Base Layer.Wall Kick off");
	private int injuredState = Animator.StringToHash("Base Layer.Injured");
	private int injuredAirState = Animator.StringToHash("Base Layer.Injured-Air");
	private int ladderJumpState = Animator.StringToHash("Base Layer.JumpOffLadder");
	
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		player = GetComponent<PlayerControls>();
	}
	
	void OnEnable() {
		// add event handlers
		
		/* Powers */
		Swing.swingState += swinging;
		Grapple.isGrappling += grapple;
		Grapple.wallGrabbing += wallGrabbing;
		verticalGrapple.isGrappling += vertGrapple;
		verticalGrapple.isHanging += hanging;
		PathPlatform.onPathDraw += pathPower;
		
		/* Movement */
		PlayerControls._grounded += grounded;
		PlayerControls.moving += moving;
		PlayerControls.jumping += jumping;
		PlayerControls.wallJumping += wallJumping;
		PlayerControls.ladder += climbing;
		PlayerControls.climbingLadder += climbingLadder;
		PlayerControls.ladderJump += ladderJump;
		PlayerControls.wallGrabbing += wallGrabbing;
		PlayerControls.isTurning += turn;
		PlayerControls.crouching += crouch;
		
		ladderDoor.grounded += grounded;
		
		playerLife.damageAnim += playerDamage;
		playerLife.dead += dead;
		
	}
	
	void OnDisable() {
		// remove event handlers
		
		/* Powers */
		Swing.swingState -= swinging;
		Grapple.isGrappling -= grapple;
		Grapple.wallGrabbing -= wallGrabbing;
		verticalGrapple.isGrappling -= vertGrapple;
		verticalGrapple.isHanging -= hanging;
		PathPlatform.onPathDraw -= pathPower;
		
		/* Movement */
		PlayerControls._grounded -= grounded;
		PlayerControls.moving -= moving;
		PlayerControls.jumping -= jumping;
		PlayerControls.wallJumping -= wallJumping;
		PlayerControls.ladder -= climbing;
		PlayerControls.climbingLadder -= climbingLadder;
		PlayerControls.ladderJump -= ladderJump;
		PlayerControls.wallGrabbing -= wallGrabbing;
		PlayerControls.isTurning -= turn;
		PlayerControls.crouching -= crouch;
		
		ladderDoor.grounded -= grounded;
		
		playerLife.damageAnim -= playerDamage;
		playerLife.dead -= dead;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		// input "speed"
		animator.SetFloat(  "InputSpeed", Mathf.Abs( Input.GetAxis("Horizontal") )  );
		animator.SetFloat(  "InputVelocity", Input.GetAxis("Horizontal")  );
		animator.SetFloat(  "VerticalVelocity", rigidbody.velocity.y  );
		
		AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
		
		// Prevent death animation from looping
		if( animator.GetBool("Dead") ) {
			
			AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0) ; // get next state on layer 0
			
			if( nextState.IsName("Base Layer.Death") ) {
				animator.SetBool( "Dead", false );
			}
		}
		
		if( currentState.nameHash == wallJumpState ) {
			animator.SetBool("WallJumping", false);
		}
		
		// turn off WallJump param after it is set.
		if( currentState.nameHash == injuredState ) {
			animator.SetBool("Damaged", false);
		}
		
		if( currentState.nameHash == injuredAirState ) {
			animator.SetBool("Damaged", false);
		}
		
		if( currentState.nameHash == ladderJumpState ) {
			animator.SetBool("LadderJumping", false);
		}
		
		
		
	}
	
	void grounded( bool isGrounded ) {
		if( isGrounded ) {
			animator.SetBool( "Grounded", true );
			animator.SetBool("Jumping", false);

		} else {
			animator.SetBool("Grounded", false);	
		}
	}
	
	void moving( bool isMoving ) {
		animator.SetBool( "Moving", isMoving );
	}
	
	// Swinging event handler
	void swinging(bool isSwinging) {
		animator.SetBool( "Swinging", isSwinging );
	}
	
	void vertGrapple( bool isGrappling ) {
		animator.SetBool( "VertGrappling", isGrappling );
	}
	
	void hanging( bool isHanging ) {
		animator.SetBool( "Hanging", isHanging );
	}
	
	void grapple( bool isGrappling ) {
		animator.SetBool( "Grappling", isGrappling );
	}
	
	void pathPower( bool isDrawing ) {
		animator.SetBool( "DrawingPath", isDrawing );
	}
	
	void jumping() {
		animator.SetBool( "Jumping", true );
	}
	
	void climbing( bool isClimbing ) {
		animator.SetBool( "OnLadder", isClimbing );
	}
	
	void climbingLadder( bool isMoving ) {
		animator.SetBool( "Climbing", isMoving );
	}
	
	void ladderJump( bool isJumping ) {
		animator.SetBool( "LadderJumping", isJumping );
	}
	
	void wallGrabbing( bool isGrabbing ) {
		animator.SetBool( "WallGrabbing", isGrabbing );
	}
	
	void wallJumping() {
		Debug.Log("WallJump");
		animator.SetBool( "WallJumping", true );
	}
	
	void turn( bool isTurning ) {
		animator.SetBool( "Turn", isTurning );
	}
	
	void crouch( bool isCrouching ) {
		animator.SetBool( "Crouch", isCrouching );
	}
	
	void playerDamage( bool takingDamage ) {
		animator.SetBool( "Damaged", takingDamage );
	}
	
	void dead() {
		animator.SetBool( "Dead", true );
		
	}
	
}
