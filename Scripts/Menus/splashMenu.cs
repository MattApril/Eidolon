using UnityEngine;
using System.Collections;

public class splashMenu : MonoBehaviour {
	
	private delegate void GUIMethod();
	private GUIMethod currentGUIMethod;
	
    // GUI Textures:
    public GameObject logoTitle;
	public GameObject pressEnter;
	public GameObject teamName;
	
	
	// Use this for initialization
	void Start () {
		
		teamName.SetActive(true);
		iTween.FadeFrom(teamName,iTween.Hash("alpha", 0.0f, "time", 0.8f, "delay", 0.4f));
		iTween.FadeTo(teamName,iTween.Hash("alpha", 0.0f, "time", 1.5f, "delay", 2.0f));
		
		logoTitle.SetActive(true);
		iTween.FadeFrom(logoTitle,iTween.Hash("alpha", 0.0f, "time", 0.8f, "delay", 4.5f));
		
		pressEnter.SetActive(true);
		iTween.FadeFrom(pressEnter,iTween.Hash("alpha", 0.0f, "time", 0.8f,"delay", 6.0f));
	
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown(KeyCode.KeypadEnter)== true || Input.GetKeyDown(KeyCode.Return) == true )
		{
			Application.LoadLevel("Menu");
		}
	}
}
