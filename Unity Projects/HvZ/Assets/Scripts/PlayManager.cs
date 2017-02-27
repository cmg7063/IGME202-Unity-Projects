/// <summary>
/// PlayManager Script.
/// This script is attached to the Info Canvas item.
/// This script controls the button UI and scene navigation for the Play screen.
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour 
{
	public Button menu;

	public void Restart()
	{
		Application.LoadLevel ("Menu");
	}

}
