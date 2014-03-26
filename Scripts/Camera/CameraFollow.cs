using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	
	/* public vars */
	public Transform player;
	public Transform net;
	public int stopFallDistance = 14;
	
	
	// Use this for initialization
	void Start () {
		iTween.CameraFadeAdd();
		
		Hashtable fade = new Hashtable();
			fade.Add("amount",1f);
			fade.Add("time",1f);
		
		iTween.CameraFadeFrom(fade);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if( player.position.y > net.position.y + stopFallDistance  ) {
			//transform.position = Vector3.Lerp (transform.position,new Vector3( player.position.x, player.position.y, 0 ),Time.deltaTime * 4);
			transform.position = new Vector3( player.position.x, player.position.y, 0 );
			
			
			/*Hashtable ht = new Hashtable();
				ht.Add ("z",0f);	
				ht.Add ("position",player.position);
				ht.Add ("time",1f);
			
			iTween.MoveUpdate(gameObject,ht);*/
			
		}
	}
	
	
}
