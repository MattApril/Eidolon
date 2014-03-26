using UnityEngine;
using System.Collections;

// player damage event dispatcher
public class damagePlayer : MonoBehaviour {
	
	/* public vars */
	public bool respawn = false;
	public bool net = false;
	
	public int playerDamage = -20;
	
	public delegate void playerCollision( bool playerMadeContact, int dmg, bool respawn );
	public static event playerCollision onPlayerCollision;
	
	// Use this for initialization
	void Start () {
		
	}
	
	public static void doDamage( int dmg, bool respawn ) {
			onPlayerCollision( true, dmg, respawn );
	}
	
//	void OnTriggerEnter(Collider obj) {
//		// if player collides
//		if( obj.tag == "Player" ) {
//			onPlayerCollision( true, playerDamage, respawn );
//		}
//	
//	}
	
	void OnTriggerEnter( Collider obj ) {
		// if player collides
		if( obj.tag == "Player" ) {
			
			// only damage if not invincible
			if( net ) {
				
				onPlayerCollision( true, playerDamage, respawn );
				
			} else if( obj.GetComponent<playerLife>().isInvincible() == false ) {
				onPlayerCollision( true, playerDamage, respawn );
			}
		}
	
	}
	
	void OnTriggerStay( Collider obj ) {
		// if player collides
		if( obj.tag == "Player" ) {
			
			// only damage if not invincible
			if( obj.GetComponent<playerLife>().isInvincible() == false ) {
				onPlayerCollision( true, playerDamage, respawn );
			}
		}
	
	}
	
}
