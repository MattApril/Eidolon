using UnityEngine;
using System.Collections;

public class Patroller : MonoBehaviour {
	
	
	public AudioClip explosionSound;
	public GameObject explosionType;
	
	public AudioClip moveSound;
	public AudioClip openSound;
	
	/* public vars */
	public float patrolSpeed = 0.5f;
	public float viewRadius = 2.0f;
	public float enageAnimationTime = 1.5f;
	public float chaseSpeed = 1.0f;
	public float selfDestructTime = 3.0f;
	
	
	public Transform[] pathPoints;
	
	/* private vars */
	private bool inRange;
	private float currentSpeed;
	private bool seeking;
	private bool engaging;
	private Vector3 moveDir;
	private int currentPoint;
	private int pointIncrement;
	private float animationTimer;
	private float selfDestructTimer;
	
	private LayerMask playerMask;
	
	private Animator animator;

	// Use this for initialization
	void Start () {
		
		animator = this.GetComponent<Animator>();
		
		inRange = false;
		
		patrol();
		pointIncrement = 1;
		
		currentPoint = 0;
		moveDir = getMoveDir( currentPoint, currentPoint + pointIncrement );
		
		playerMask = 1 << 10;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		Collider[] colliders = Physics.OverlapSphere( transform.position, viewRadius, playerMask );
		
		if( colliders.Length > 0 ) {
			foreach( Collider collider in colliders ) {
				
				if( collider.tag == "Player" ) {
					
					// enage player
					if( inRange == false ) {
						StartCoroutine( engagePlayer() );
						inRange = true;
					}
					
					// TODO: implement smarter seeking by checking paths 
					// 		 ahead/behind that may lead closer to the target.
					if( seeking ) {
						
						selfDestructTimer += Time.deltaTime;
						
						if( selfDestructTimer >= selfDestructTime ) {
							// explode
							Explosion ();
							
							Object.Destroy( this.transform.parent.gameObject );
							
							
						}
						
						Vector3 newMoveDir = moveDir;
						
						// check where player is and move in that direction
						// check if moving on x-axis or y-axis
						if( moveDir == Vector3.right || moveDir == Vector3.left ) {
							if( collider.transform.position.x > transform.position.x ) {
								// set move dir as right	
								newMoveDir = Vector3.right;
							} else {
								// set move dir as left	
								newMoveDir = Vector3.left;
							}
						} else if( moveDir == Vector3.up || moveDir == Vector3.down ) {
							if( collider.transform.position.y > transform.position.y ) {
								// set move dir as up
								newMoveDir = Vector3.up;
							} else {
								// set move dir as down
								newMoveDir = Vector3.down;
							}
						}
						
						if( newMoveDir != moveDir ) {
							currentPoint += pointIncrement;
							pointIncrement *= -1;
							moveDir = newMoveDir;
						}
					}
					
				}
			}
			
		} else {
			if( inRange ) {
				inRange = false;	
			}
			
			if( seeking ) {
				animator.SetBool( "Chasing", false );
				patrol();
			}
		}
		
		if( !engaging ) {
			transform.Translate( moveDir * currentSpeed * Time.deltaTime );
		}
		
		// check boundaries
		if( moveDir == Vector3.right ) {
			
			// if patroller is passing the next path point
			if( transform.position.x >= pathPoints[currentPoint + pointIncrement].transform.position.x ) {
				transform.position = pathPoints[currentPoint + pointIncrement].transform.position;
				changePath();
				
			}
			
		} else if( moveDir == Vector3.left ) {
			// if patroller is passing the next path point
			if( transform.position.x <= pathPoints[currentPoint + pointIncrement].transform.position.x ) {
				transform.position = pathPoints[currentPoint + pointIncrement].transform.position;
				changePath();
				
			}
		} else if( moveDir == Vector3.up ) {
			// if patroller is passing the next path point
			if( transform.position.y >= pathPoints[currentPoint + pointIncrement].transform.position.y ) {
				transform.position = pathPoints[currentPoint + pointIncrement].transform.position;
				changePath();
				
			}
		} else if( moveDir == Vector3.down ) {
			// if patroller is passing the next path point
			if( transform.position.y <= pathPoints[currentPoint + pointIncrement].transform.position.y ) {
				transform.position = pathPoints[currentPoint + pointIncrement].transform.position;
				changePath();
				
			}
		}
		
	}
	
	Vector3 getMoveDir( int pointFrom, int pointTo ) {
		Vector3 dir = pathPoints[pointTo].position - pathPoints[pointFrom].position;
		return dir.normalized;
	}
	
	void changePath() {
		// increment/decrement the current point
		currentPoint += pointIncrement;
		
		// change increment direction if end reached
		if( currentPoint == pathPoints.Length-1 ) {
			// end reached (right)
			pointIncrement = -1;
		} else if( currentPoint == 0 ) {
			// reached start (left)
			pointIncrement = 1;
		}
		
		// update the move direction
		moveDir = getMoveDir( currentPoint, currentPoint + pointIncrement );
		
	}
	
	IEnumerator engagePlayer() {
		//seeking = true;
		//currentSpeed = chaseSpeed;
		
		// stop moving
		// play animation
		// if still in range after the end of the animtion, enable seeking
		animator.SetBool("Patrolling", false );
		engaging = true;
		animator.SetBool( "Engaging", true );
		
		audio.PlayOneShot(openSound);
		// wait
		yield return new WaitForSeconds( enageAnimationTime );
		
		
		engaging = false;
		animator.SetBool( "Engaging", false );
		
		if( inRange ) {
			audio.clip = moveSound;
			audio.loop = true;
			
			audio.Play ();
			
			seeking = true;
			currentSpeed = chaseSpeed;
			selfDestructTimer = 0f;
			animator.SetBool( "Chasing", true );
		}
	}
	
	void patrol() {
		audio.Stop ();
		
		
		
		engaging = false;
		seeking = false;
		currentSpeed = patrolSpeed;
		animator.SetBool("Patrolling", true );
	}
	
	void OnCollisionEnter( Collision obj ) {
		
		if( obj.gameObject.tag == "Player" ) {
			damagePlayer.doDamage( -10, false );
			
			Explosion ();
			
			Object.Destroy( this.transform.parent.gameObject );
		}
		
	}
	
	void Explosion()
	{
		Vector3 explosionPos = gameObject.transform.position;
			
			GameObject explosion;
			
			explosion = (GameObject)GameObject.Instantiate(explosionType,explosionPos,gameObject.transform.rotation);
			explosion.gameObject.AddComponent<AudioSource>();
			explosion.audio.clip = explosionSound;
			explosion.audio.Play ();
	}
}