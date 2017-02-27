using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// CharSelManager Script
/// This script is to be attached to the Menu Canvas object
/// A script that controls the button UI and scene navigation for the Character Selection screen
/// Upon the selection of a button, method is called and respective scene is loaded
/// </summary>


public class CharSelManager : MonoBehaviour 
{
	// Attributes
	public Button menu;
	public Button lucas;
	public Button eleven;
	public Button dustin;

	private int selectedChar = 0; // Save Player character selection choice

	// Methods
	public void Menu()
	{
		Application.LoadLevel ("MainMenu");
	}

	public void Lucas()
	{
		selectedChar = 1;
		PlayerPrefs.SetInt ("selectedChar", (selectedChar));
		Application.LoadLevel ("Play");

	}

	public void Eleven()
	{
		selectedChar = 2;
		PlayerPrefs.SetInt ("selectedChar", (selectedChar));
		Application.LoadLevel ("Play");
	}

	public void Dustin()
	{
		selectedChar = 3;
		PlayerPrefs.SetInt ("selectedChar", (selectedChar));
		Application.LoadLevel ("Play");

	}

}
