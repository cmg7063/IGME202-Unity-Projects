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
    public Button notes;
    public Button menu;
    public Button quit;
    public Button play;

    // Methods
    public void Play()
    {
        Application.LoadLevel("Build_01");
    }

    public void Notes()
    {
        Application.LoadLevel("Notes");
    }

    public void Menu()
    {
        Application.LoadLevel("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
