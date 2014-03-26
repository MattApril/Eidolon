using UnityEngine;
using System.Collections;

public class playerLife : MonoBehaviour {
	
	GameObject playerModel;
	
	/* public vars */
	public int startingHealth = 50;
	public const int maxHealth = 100;
	public float freezeTime = 1.0f;
	public float invincibleTime = 2.0f;
	private Vector2 healthbarPos = new Vector2( 10, 10 );
	
	/* private vars */
	private int health;
	private Vector3 respawnPoint;
	private bool invincible;
	
    private Texture2D healthTexture;
    private Texture2D healthContainerTexture;
    private GUIStyle healthStyle;
    private GUIStyle healthContainerStyle;
	
	private TrailRenderer trailRender;
	private float trailDisableTime;
	
	public delegate void onDeath();
	public static event onDeath dead;
	
	bool flashed;
	bool flashing;
	float flashTimer;
	private float flashTime;
	float cycle;
	
	Material playerMat;
	Material invisible;
	
	public delegate void disableMovement(bool disable);
	public static event disableMovement disablePlayerMovement;
	
	public delegate void OnDamage(bool damaged);
	public static event OnDamage damageAnim;
	
	
	void OnEnable() {
		// add damage event handler
		damagePlayer.onPlayerCollision += onPlayerDamage;
		
		// add checkpoint event handler
		checkpoint.onNewCheckpoint += onNewCheckpoint;
	}
	
	void OnDisable() {
		// remove damage event handler
		damagePlayer.onPlayerCollision -= onPlayerDamage;
		
		// remove checkpoint event handler
		checkpoint.onNewCheckpoint -= onNewCheckpoint;
	}
	
	// Use this for initialization
	void Start () {
		
		health = startingHealth;
		respawnPoint = transform.position;
		
		trailDisableTime = 0.0f;
		
		GameObject trailObj = GameObject.Find("trail");
		trailRender = trailObj.GetComponent<TrailRenderer>();
		
		playerModel = gameObject.transform.FindChild("LoganBody").gameObject;
		playerMat = playerModel.renderer.material;
		invisible = (Material) Resources.Load ("Materials/invisible");
		cycle = 0.10f;
		flashTime = invincibleTime - (freezeTime/2.0f);
		
        healthTexture = new Texture2D(1, 1);
        healthTexture.SetPixel(0, 0, new Color(0f, 0.58f, 1.0f, 0.7f) );
        healthTexture.wrapMode = TextureWrapMode.Repeat;
        healthTexture.Apply();

        healthContainerTexture = new Texture2D(1, 1);
        healthContainerTexture.SetPixel(0, 0, new Color(1f, 1f, 1f, 0.8f) );
        healthContainerTexture.wrapMode = TextureWrapMode.Repeat;
        healthContainerTexture.Apply();

        healthStyle = new GUIStyle();
        healthStyle.normal.background = healthTexture;

        healthContainerStyle = new GUIStyle();
        healthContainerStyle.normal.background = healthContainerTexture;
		
	}
	
	void Update() {
		
		// reset trail to prevent line being drawn after respawn.
		if( trailDisableTime > 0.0f ) {
			trailDisableTime -= Time.deltaTime;
			
			if( trailDisableTime <= 0.0f ) {
				trailRender.enabled = true;
			}
			
		}
		
		if(flashing)
		{
			
			flashTimer -= Time.deltaTime;
			cycle -= Time.deltaTime;
			
			if(cycle <= 0)
			{
				cycle = 0.10f;
				Flash();
			}
			
			if(flashTimer <= 0)
			{
				StopFlashing();
			}
		}
	}
	
	// player damage event handler
	void onPlayerDamage( bool playerMadeContact, int dmg, bool respawn ) {
		if(playerMadeContact) {
			
			// set health
			health += dmg;			
			health = Mathf.Clamp( health, 0, maxHealth );
			
			// if negative damage (i.e. not health pickup)
			if( dmg < 0 ) {
				
				
				// respawn if not dead
				if( health > 0 ) {
					
					if( respawn ) {
						//disable trail
						trailRender.enabled = false;
						trailDisableTime = 1.0f;
						
						transform.position = respawnPoint;
						rigidbody.velocity = new Vector3(0, 0, 0);
					} else {
						damageAnim( true );
						
						// set invincibility
						StartCoroutine( makeInvincible( invincibleTime ) );
						
						// disable movement
						StartCoroutine( freezePlayer( freezeTime ) );
						
					}
					
				} else if( health <= 0 ) {
					//Debug.Log("player dead, restart level");
					dead();
					
					// disable input
					disablePlayerMovement( true );
					
					// no need to take any more damage
					invincible = true;
					
					// remove eidolon
					GameObject.Find("trail").GetComponent<TrailRenderer>().time = 0f;
					Destroy( GameObject.Find("trail").GetComponent<ParticleSystem>() );
				}
			}
			
		}
		
	}
	
	public bool isInvincible() {
		return invincible;
	}
	
	void onNewCheckpoint( bool playerMadeContact, Vector3 pos ) {
		if( playerMadeContact ) {
			respawnPoint = pos;
		}
	}
	
	// draw health bar
	void OnGUI() {
		if( !GameObject.Find("Main Camera").GetComponent<ingameMenu>().isPaused() && health > 0  ) {
        	GUI.Box( new Rect(healthbarPos.x, healthbarPos.y, 208, 16), "", healthContainerStyle);
        	GUI.Box( new Rect(healthbarPos.x + 4, healthbarPos.y + 3, 200 * health / 100.0f, 10), "", healthStyle);
		}
	}
	
	void Flashing()
	{
		cycle = 0.10f;
		flashing = true;
		flashTimer = flashTime;
	}
	
	void StopFlashing()
	{
		flashing = false;
		playerModel.renderer.material = playerMat;
	}
	
	void Flash()
	{
		flashed = !flashed;
		
		if(flashed) { 
			playerModel.renderer.material = invisible;
		}
		else {
			playerModel.renderer.material = playerMat;	
		}
	}
	
	private IEnumerator freezePlayer( float freezeTime ) {
		
		disablePlayerMovement( true );
		Debug.Log("disableMovement");
		
		yield return new WaitForSeconds( freezeTime * 0.4f );
		
		Flashing();
		
		yield return new WaitForSeconds( freezeTime * 0.6f );
		
		Debug.Log("enableMovement");
		disablePlayerMovement( false );
	
		
	}
	
	private IEnumerator makeInvincible( float invincibleTime ) {
		
		invincible = true;

		yield return new WaitForSeconds( invincibleTime );
		
		invincible = false;
		
	}
}
