/// <summary>
/// MenuManager Script.
/// This script is attached to the Menu Canvas item.
/// This script controls the button UI and scene navigation for the Menu screen.
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour 
{
	public Button start;
	public Button instructions;

	public void Play()
	{
		Application.LoadLevel ("Build01");
	}
	public void Exit()
	{
		Application.Quit ();
	}
}
