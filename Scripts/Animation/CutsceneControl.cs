using UnityEngine;
using System.Collections;

public class CutsceneControl : MonoBehaviour {
	
	public string NextSceneName;
	
	private Animator animator;
	private AnimatorStateInfo currentState;
	private float animationLength;
	private float elapsedTime;
	private bool animationComplete;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
		animationLength = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if( animationLength == 0f ) {
			currentState = animator.GetCurrentAnimatorStateInfo(0);
			animationLength = currentState.length;
			//Debug.Log( animationLength );
		} else {
			
			elapsedTime += Time.deltaTime;
			
			if( elapsedTime >= animationLength && !animationComplete ) {
				//fade and go to next scene
				Application.LoadLevel( NextSceneName );
				animationComplete = true;
			}
			
		}
		
		if( Input.GetKeyDown(KeyCode.Space) ) {
			Application.LoadLevel( NextSceneName );
		}
	}
}
