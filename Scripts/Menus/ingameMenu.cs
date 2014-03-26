using UnityEngine;
using System.Collections;

public class ingameMenu : MonoBehaviour {
	
	public KeyCode pauseKey = KeyCode.Escape;
	public GUISkin mySkin;
	public Vector2 bBox, bSpace;
	
	private bool paused;
	private float brightness;
	private float volume;
	private bool dead;
	
	private delegate void GUIMethod();
	private GUIMethod currentGUIMethod;

	// Use this for initialization
	void Start () {
		// no initial GUI
		this.currentGUIMethod = null;
		
		//set default volume
		volume = 1f;
		
		// set brightness
		if( PlayerPrefs.HasKey("brightness") ) {
			brightness = PlayerPrefs.GetFloat("brightness");
		}
		GameObject.Find("Directional light").light.intensity = brightness * 0.25f;
		
		// Set volume
		if( PlayerPrefs.HasKey("volume") ) {
			brightness = PlayerPrefs.GetFloat("volume");
		}
		AudioListener.volume = volume;
	}
	
	void OnEnable() {
		// add death event handler
		playerLife.dead += onPlayerDeath;
	}
	
	void OnDisable() {
		// remove death event handler
		playerLife.dead -= onPlayerDeath;
	}
	
	void Update () {
		
		// pause game, but not during death menu
		if( Input.GetKeyDown(pauseKey) && !dead ) {
			togglePause();
		}
		
	}
	
	// Toggle pause menu
	void togglePause() {
		
		paused = !paused; // toggle paused state
		
		if( paused ) {
			fadeBG();
			this.currentGUIMethod = PauseMenu;
			Time.timeScale = 0.0f;
		} else {
			this.currentGUIMethod = null;
			Time.timeScale = 1.0f;
			unfadeBG();
		}
		
	}
	
	
	void OnGUI() {
		
		GUI.skin = mySkin;
		
		// run current GUI
		if( this.currentGUIMethod != null ) {
			this.currentGUIMethod();
		}
		
	}
	
	/** Death Menu **/
	void DeathMenu() {
		
		float temp = Screen.height/2 -50;
		
		if(  GUI.Button( new Rect(bSpace.x, temp, bBox.x, bBox.y), "Restart" )  ) {
			// Re-load current level
			Application.LoadLevel( Application.loadedLevelName );
		}
		
		if(  GUI.Button( new Rect(bSpace.x, temp+= bSpace.y, bBox.x, bBox.y), "Main Menu" )  ) {
			this.currentGUIMethod += QuitConfirmationPopup;
		}
	}
	/** Pause Menu **/
	void PauseMenu() {
		
		float temp = Screen.height/2 -50;
		
		if(  GUI.Button( new Rect(bSpace.x, temp, bBox.x, bBox.y), "Resume" )  ) {
			togglePause();
		}
		
		if(  GUI.Button( new Rect(bSpace.x, temp+= bSpace.y , bBox.x, bBox.y), "Options" )  ) {
			this.currentGUIMethod = OptionsMenu;
		}
		
		if(  GUI.Button( new Rect(bSpace.x, temp+= bSpace.y , bBox.x, bBox.y), "Main Menu" )  ) {
			this.currentGUIMethod += QuitConfirmationPopup;
		}
	}
	
	/** OptionsMenu **/
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
			this.currentGUIMethod = PauseMenu;
		}
	}
	
	/** OptionsMenu -> Controls **/
	void ControlsDisplay() {
		if(  GUI.Button( new Rect(10, 25, 100, 20), "< Back" )  ) {
			// Re-load current level
			this.currentGUIMethod = OptionsMenu;
		}
	}
	
	/** OptionsMenu -> MainMenu **/
	/** DeathMenu   -> MainMenu **/
	void QuitConfirmationPopup() {
		GUI.Window( 0, new Rect(Screen.width/2 -100, Screen.height/2 - 50, 200, 150), ConfirmWindow, "Are you sure?" );
	}
	
	/** QuitConfirmationPopup() WindowFunction **/
	void ConfirmWindow( int id ) {
		
		GUI.Label( new Rect(Screen.width/2, Screen.height/2, 200, 200), "You will lose all progress\\: ");
		
		if(  GUI.Button( new Rect(10 + 30, 75, 30, 30), "Yes" )  ) {
			// Load main menu
            Time.timeScale = 1f;
			Application.LoadLevel( "Menu" );
		}
		
		if(  GUI.Button( new Rect(90 + 30, 75, 30, 30), "No" )  ) {
			// Close popup
			this.currentGUIMethod -= QuitConfirmationPopup;
		}
	}
	
	void onPlayerDeath() {
		dead = true;
		deathFade();
		this.currentGUIMethod = DeathMenu;
	}
	
	void fadeBG() {
		Hashtable fade = new Hashtable();
			fade.Add("amount", 0.25f);
			fade.Add("time", 1.5f);
			fade.Add("easetype", "easeOutQuart" );
			fade.Add("ignoretimescale", true);
			
		iTween.CameraFadeTo(fade);
		
	}
	
	void unfadeBG() {
		Hashtable fade = new Hashtable();
			fade.Add("amount", 0f);
			fade.Add("time", 0.25f);
			
		iTween.CameraFadeTo(fade);
	}
	
	void deathFade() {
		Hashtable fade = new Hashtable();
			fade.Add("amount", 0.4f);
			fade.Add("time", 4f);
			fade.Add("easetype", "linear" );
			
		iTween.CameraFadeTo(fade);
	}

    public bool isPaused()
    {
        return paused;
    }
	
}
