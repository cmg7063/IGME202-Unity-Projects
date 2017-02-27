using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// GameOverManager Script
/// This script is to be attached to the GameOver Canvas object
/// A script that controls the button UI and scene navigation for the Game Over screen
/// </summary>

public class GameOverManager : MonoBehaviour 
{
	// Attributes
	public Button yes;
	public Button no;

	// Methods
	public void Yes()
	{
		Application.LoadLevel ("Play");
	}

	public void No()
	{
		Application.LoadLevel ("MainMenu");
	}
}
