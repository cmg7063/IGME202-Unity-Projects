using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// MenuManager Script
/// This is to be attached to the MainMenu Canvas object
/// A script that controls the button UI and scene navigation for the Main Menu screen
/// </summary>


public class MenuManager : MonoBehaviour 
{
	// Attributes
	public Button start;
	public Button character;

	// Methods
	public void Play()
	{
		Application.LoadLevel ("Play");
	}

	public void CharSel()
	{
		Application.LoadLevel ("Character");
	}

	public void Start()
	{
		PlayerPrefs.DeleteKey ("hiScore"); // Delete previous hi-scores
	}
}
