using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathPlatform : MonoBehaviour {
	
	public AudioClip pathSound;
	public AudioClip pathFinish;
	
	/* config vars */
	public KeyCode inputKey = KeyCode.E;
	public float pathWidth = 0.5f;
	public float maxPathDistance = 10.0f;
	
	/* private vars */
	private List<Vector3> posList;
	private int oneWayContactCount;
	private Vector3 previousPathPoint;
	private Vector3 startPathPoint;
	private bool isDrawing;
	private bool pathComplete;
	private bool movementDisabled;
	
	/* gameobj's */
	private List<GameObject> cylList;
	public GameObject cylinder;
	public GameObject cylinder_end;
	private GameObject trail;
	
	/* components */
	private LineRenderer lineRenderer;
	private PlayerControls controls;
	
	public delegate void pathDraw( bool isDrawing );
	public static event pathDraw onPathDraw;
	
	void OnEnable() {
		// add platform event handler
		oneWayPlatform.onPlayerCollision += setOneWayCollision;
		playerLife.disablePlayerMovement += disableMovement;
		PlayerControls._grounded += grounded;
	}
	
	void OnDisable() {
		// remove platform event handler
		oneWayPlatform.onPlayerCollision -= setOneWayCollision;
		playerLife.disablePlayerMovement -= disableMovement;
		PlayerControls._grounded -= grounded;
	}
	
	// Use this for initialization
	void Start () {
		
		posList = new List<Vector3>();
		oneWayContactCount = 0;
		pathComplete = false;
		
		cylList = new List<GameObject>();
		
		// access line renderer
		lineRenderer = GameObject.Find("path").GetComponent<LineRenderer>();
		lineRenderer.SetWidth( GameConstants.trailWidth, GameConstants.trailWidth );
		
		trail = GameObject.Find("trail");
		
		controls = GetComponent<PlayerControls>();
	}
	
	// Update is called once per frame
	void Update () {
		
		// don't create path while on ladder
		if( !controls.isClimbing() && !movementDisabled ) {
			
			if( Input.GetKeyDown( inputKey ) ) {
				
				isDrawing = true;
				
				if(!audio.isPlaying)
				{
					audio.clip = pathSound;
					audio.loop = true;
					
					audio.Play ();
				}
				
				// clear any previous list and delete old path
				posList.Clear();
				deletePath();
				
				// set starting point for range detection
				startPathPoint = transform.position;
				
				posList.Add(trail.transform.position);
				
				trail.GetComponent<TrailRenderer>().time = 1.0f;
				
				onPathDraw( true );
				
			} else if( Input.GetKey(inputKey) ) {
				
				// record points, or draw path when out of range
				if( (startPathPoint - transform.position).magnitude < 5.0f ) {
					
					float minDistance = 0.01f;
					
					// check for minDist between current point and last recorded point
					if( (posList[ posList.Count-1 ] - trail.transform.position).magnitude > minDistance ) {
					
						// record
						posList.Add(trail.transform.position);
					}
					
					
				} else {
					if( !pathComplete ) {
						
						onPathDraw( false );
						
						if( posList.Count > 8 ) {
							createPath( posList );
	
						}
					
					}
				}
			}
			
			//TODO: Draw in realtime (prevent choke/freeze?)
			// complete path when key is let go
			if( Input.GetKeyUp(inputKey) ) {
				
				if( !pathComplete ) {
					
					onPathDraw( false );
					
					audio.Stop();
					
					if( posList.Count > 8 ) {
						createPath( posList );

					}
					
				}
			}
			
		}
		
		// destroy path when player moves far enough away from it
		if( (startPathPoint - transform.position).magnitude >= maxPathDistance ) {
			posList.Clear();
			deletePath();
		}
		
	}
	
	
	//Options:
	// - fade-in (delay draw)***
	void createPath( List<Vector3> pointList ) {
		
		audio.Stop ();
		audio.PlayOneShot(pathFinish,0.5f);
		
		Debug.Log("Draw Path");
		
		// point counter
		int count = 0;
		int cutoff = 4;
		
		//create colliders
		foreach( Vector3 pos in pointList ) {
			
			GameObject cyl;
			
			// instantiate path colliders
			if( count == 0 || count == pointList.Count - cutoff ) {
				cyl = Instantiate(cylinder_end) as GameObject; // GO for start and end of path
				cyl.transform.position = pos;
				cylList.Add( cyl ); // track GO so it can be deleted
				previousPathPoint = pos;
			} else {
				if( (pos - previousPathPoint).magnitude >= 0.1f ) {
					cyl = Instantiate(cylinder) as GameObject; // GO for all point inbetween start and end
					cyl.transform.position = pos;
					cylList.Add( cyl ); // track GO so it can be deleted
					previousPathPoint = pos;
				}
			}
			
			count++;
			
			// skip the last 6 points, to avoid intersection with player
			if( count > pointList.Count - cutoff ) break;
		
		}
		
		//draw line path
		trail.GetComponent<TrailRenderer>().time = 0.0f;
		
		lineRenderer.enabled = true;
		lineRenderer.SetVertexCount( pointList.Count - cutoff );
		
		for( int m = 0; m < pointList.Count - cutoff; m++ ) {
			lineRenderer.SetPosition(m, pointList[m] );
		}
		
		isDrawing = false;
		pathComplete = true;
		
	}
	
	// remove path colliders and visuals
	void deletePath() {
		foreach( GameObject cyl in cylList ) {
			Destroy(cyl);
		}
		cylList.Clear();
		pathComplete = false;
		
		// remove line
		lineRenderer.SetVertexCount( 0 );
		oneWayContactCount = 0;
		trail.GetComponent<TrailRenderer>().time = 1.0f;
	}
	
	void disableMovement( bool disable ) {
		
		if(disable) {
			movementDisabled = true;
		} else {
			movementDisabled = false;
		}
	}
	
	// track platform intersections
	void setOneWayCollision( bool playerMadeContact ) {
		
		// track number of intersection with path colliders
		if( playerMadeContact ) {
			oneWayContactCount++;
		} else {
			oneWayContactCount--;	
		}
		
		// if intersecting, ignore collisions
		if( oneWayContactCount > 0 ) {
			Physics.IgnoreLayerCollision( 10, 11, true );
		} else if( oneWayContactCount <= 0) {
			Physics.IgnoreLayerCollision( 10, 11, false );
		}
	}
	
	// Grounding event handler
	void grounded( bool isGrounded ) {
		if( isDrawing ) {
			if( isGrounded ) {
				
				onPathDraw( false );
					
				if( posList.Count > 8 ) {
					createPath( posList );

				}
			}
		}
	}
	
}
