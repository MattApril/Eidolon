using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	
	private delegate void GUIMethod();
	private GUIMethod currentGUIMethod;
    //private GUIStyle blankStyle = new GUIStyle(); //an "empty" style to avoid any of Unity's default settings
	
    // GUI Textures:
	public GUISkin mySkin;
	public Vector2 bBox, bSpace;
    public GameObject logoTitle;
	
	private float brightness;
	private float volume;

	// Use this for initialization
	void Start () {
		
		iTween.FadeTo(logoTitle,iTween.Hash("alpha", 0.2f, "time", 1));
		
		if( PlayerPrefs.HasKey("brightness") ) {
			brightness = PlayerPrefs.GetFloat("brightness");
		}
		if( PlayerPrefs.HasKey("volume") ) {
			brightness = PlayerPrefs.GetFloat("volume");
		}
		
		// start at main menu
		this.currentGUIMethod = MainMenu;
	}
	
	void OnGUI() {
		
		GUI.skin = mySkin;
		
		// run current GUI
		this.currentGUIMethod();
	}
	
	/* Main Menu GUI*/
	void MainMenu() {
		
		float temp = Screen.height/2 + 100;
			
		if( GUI.Button( new Rect(bSpace.x, temp, bBox.x, bBox.y), "Start" )  ) {
			Application.LoadLevel("Cutscene1");
		}
		
		if(  GUI.Button( new Rect(bSpace.x, temp+= bSpace.y , bBox.x, bBox.y), "Options" )  ) {
			this.currentGUIMethod = OptionsMenu;
		}
		
		if(  GUI.Button( new Rect(bSpace.x, temp+= bSpace.y, bBox.x, bBox.y), "Credits" )  ) {
			this.currentGUIMethod = Credits;
		}
		
		if(  GUI.Button( new Rect(bSpace.x, temp+= bSpace.y, bBox.x, bBox.y), "Exit" )  ) {
			// add Exit popup to current GUI
			this.currentGUIMethod += ExitConfirmationPopup;
		}
	}
	
	/* Level Selection GUI */
	void LevelSelectMenu() {
		
		float temp = Screen.height/2 + 100;
		
		if(  GUI.Button( new Rect(bSpace.x, temp, bBox.x, bBox.y), "Tutorial" )  ) {
			Application.LoadLevel("Tutorial");
		}
		if(  GUI.Button( new Rect(bSpace.x, temp+= bSpace.y , bBox.x, bBox.y), "Level 1" )  ) {
			Application.LoadLevel("LevelOne");
		}
		if(  GUI.Button( new Rect(bSpace.x, temp+= bSpace.y , bBox.x, bBox.y), "Main Menu" )  ) {
			this.currentGUIMethod = MainMenu;
		}
	}
	
	void OptionsMenu() {
		
		float temp = Screen.height/2 + 100;
		
		if(  GUI.Button( new Rect(bSpace.x, temp, bBox.x, bBox.y), "Controls" )  ) {
			// Re-load current level
			this.currentGUIMethod = ControlsDisplay;
		}
		
		GUI.Label( new Rect(bSpace.x, temp+= bSpace.y, bBox.x, bBox.y), "Brightness :" );
		
		brightness = GUI.HorizontalSlider( new Rect(bSpace.x + 135, temp + 18, bBox.x, bBox.y), brightness, 0f, 1f );
		PlayerPrefs.SetFloat( "brightness", brightness );
		
		
		GUI.Label( new Rect(bSpace.x, temp+= bSpace.y, bBox.x, bBox.y), "Audio Volume :" );
		
		volume = GUI.HorizontalSlider( new Rect(bSpace.x + 135, temp + 18, bBox.x, bBox.y), volume, 0f, 1f );
		PlayerPrefs.SetFloat( "volume", brightness );
		AudioListener.volume = volume;
		
		if(  GUI.Button( new Rect(bSpace.x, temp+= bSpace.y, bBox.x, bBox.y), "< Back" )  ) {
			// Re-load current level
			this.currentGUIMethod = MainMenu;
		}
	}
	
	/*
	void OptionsMenu() {
		
		float temp = Screen.height/2 -50;
		
		if(  GUI.Button( new Rect(bSpace.x, temp, bBox.x, bBox.y), "Controls" )  ) {
			// Re-load current level
			this.currentGUIMethod = ControlsDisplay;
		}
		
		GUI.Label( new Rect(bSpace.x, temp+= bSpace.y, bBox.x, bBox.y), "Brightness :" );
		
		brightness = GUI.HorizontalSlider( new Rect(bSpace.x + 135, temp + 18, bBox.x, bBox.y), brightness, 0f, 1f );
		PlayerPrefs.SetFloat( "brightness", brightness );
		GameObject.Find("Directional light").light.intensity = brightness * 0.25f;
		
		
		
		GUI.Label( new Rect(bSpace.x, temp+= bSpace.y, bBox.x, bBox.y), "Audio Volume :" );
		
		volume = GUI.HorizontalSlider( new Rect(bSpace.x + 135, temp + 18, bBox.x, bBox.y), volume, 0f, 1f );
		PlayerPrefs.SetFloat( "volume", brightness );
		AudioListener.volume = volume;
		
		if(  GUI.Button( new Rect(bSpace.x, temp+= bSpace.y, bBox.x, bBox.y), "< Back" )  ) {
			// Re-load current level
			this.currentGUIMethod = MainMenu;
		}
	}*/
	
	/** OptionsMenu -> Controls **/
	void ControlsDisplay() {
		if(  GUI.Button( new Rect(10, 25, 100, 20), "< Back" )  ) {
			// Re-load current level
			this.currentGUIMethod = OptionsMenu;
		}
	}
	
	/* Credits */
	void Credits() {
		
		float temp = Screen.height/2 + 100;
		
		if(  GUI.Button( new Rect(bSpace.x, temp, bBox.x, bBox.y), "< Back" )  ) {
			this.currentGUIMethod = MainMenu;
		}
	}
	
	void ExitConfirmationPopup() {
		GUI.Window( 0, new Rect(Screen.width/2 -100, Screen.height/2 - 100, 200, 150), ConfirmWindow, "Are you sure?" );
	}
	
	/** QuitConfirmationPopup() WindowFunction **/
	void ConfirmWindow( int id ) {
		
		if(  GUI.Button( new Rect(10 + 30, 75, 30, 30), "Yes" )  ) {
			// Load main menu
			Application.Quit();
		}
		
		if(  GUI.Button( new Rect(90 + 30, 75, 30, 30), "No" )  ) {
			// Close popup
			this.currentGUIMethod -= ExitConfirmationPopup;
		}
	}
	
}
