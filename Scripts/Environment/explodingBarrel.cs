using UnityEngine;
using System.Collections;

public class explodingBarrel : MonoBehaviour {
	
	public float explosiveTimer = 6.0f;
	
	private bool touchedEidolon;
	private float lifetime;
	
	public GameObject explosionType;
	public AudioClip explosionSound;

	// Use this for initialization
	void Start () {
		lifetime = explosiveTimer;
	}
	
	// Update is called once per frame
	void Update () {
		if( touchedEidolon ) {
			lifetime -= Time.deltaTime;
			
			if( lifetime <= 0 ) {
				// play explosion effect
				explode();
			}
		}
	}
	
	void OnCollisionEnter( Collision otherObj ) {
		
		if( otherObj.gameObject.layer == LayerMask.NameToLayer("PathPlatform") ) {
			touchedEidolon = true;	
		}
		
		if( otherObj.gameObject.tag == "explosiveBarrelTarget" ) {
			Destroy( otherObj.gameObject );
			explode();
		}
		
		if( otherObj.gameObject.layer == LayerMask.NameToLayer("Level") && !touchedEidolon ) {
			// play explosion effect
			explode();
		}
	}
	
	void explode() {
		
		//play explision effect
		Vector3 explosionPos = gameObject.transform.position;
			
			GameObject explosion;
			
			explosion = (GameObject)GameObject.Instantiate(explosionType,explosionPos,gameObject.transform.rotation);
			explosion.gameObject.AddComponent<AudioSource>();
			explosion.audio.clip = explosionSound;
			explosion.audio.Play ();
		
		Destroy( this.gameObject );	
	}
}
