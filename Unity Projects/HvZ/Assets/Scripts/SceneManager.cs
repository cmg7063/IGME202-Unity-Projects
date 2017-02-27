/// <summary>
/// SceneManager Script.
/// This script is attached to the SceneManager GameObject, it controls the Instigation of
/// the obstacles, zombies, and humans. This script adds all instantiated GameObjects to 
/// a List<GameObject>. This script also allows the user to flip throughout the camera views
/// and toggle on/off the debugLines with the 'D' key.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour 
{
	// GameObject prefabs
	public GameObject tree1;
	public GameObject tree2;
	public GameObject rock1;
	public GameObject rock2;
	public GameObject wagon;
	public GameObject tomb1;

	public Text humanText;
	public Text zombieText;

	public GameObject zombiePref;
	public GameObject humanPref;

	private bool debugLines; // Toggle on/off openGL lines

	private int camView = 0; // Flip throughout the camera views

	// List of GameObjects
	public List<GameObject> obstacles = new List<GameObject>();
    public List<GameObject> zombies = new List<GameObject>();
	public List<GameObject> humans = new List<GameObject>();

	public int numHumans = 10;
	public int numZombies = 4;
	public int numObs = 50;
	
	// Use this for initialization
	void Start () 
	{
		Vector3 position = Vector3.zero;

		// Instantiate and add humans to humans List<GameObject>
        for(int i = 0; i < numHumans; i++)
        {
			position = new Vector3(Random.Range(10, 90), 1, Random.Range(10, 90));
			humans.Add((GameObject)(Instantiate(humanPref, position, Quaternion.identity)));

        }

		// Instantiate and add zombies to zombies List<GameObject>
        for (int i = 0; i < numZombies; i++)
        {
			position = new Vector3(Random.Range(10, 50), 1, Random.Range(10, 50));
			zombies.Add((GameObject)(Instantiate(zombiePref, position, Quaternion.identity)));
        }

		// Instantiate and add tree1 to obstacle List<GameObject>
        for (int i = 0; i < 10; i++)
        {
			position = new Vector3(Random.Range(10, 90), 0, Random.Range(10, 90));
			obstacles.Add((GameObject)(Instantiate(tree1, position, Quaternion.identity)));
		}

		// Instantiate and add tree2 to obstacle List<GameObject>
		for (int i = 10; i < 15; i++)
		{
			position = new Vector3(Random.Range(10, 90), 0, Random.Range(10, 90));
			obstacles.Add((GameObject)(Instantiate(tree2, position, Quaternion.identity)));
		}

		// Instantiate and add rock1 to obstacle List<GameObject>
		for (int i = 15; i < 25; i++)
		{
			position = new Vector3(Random.Range(10, 90), 0, Random.Range(10, 90));
			obstacles.Add((GameObject)(Instantiate(rock1, position, Quaternion.identity)));
		}

		// Instantiate and add rock2 to obstacle List<GameObject>
		for (int i = 25; i < 30; i++)
		{
			position = new Vector3(Random.Range(10, 90), 0, Random.Range(10, 90));
			obstacles.Add((GameObject)(Instantiate(rock2, position, Quaternion.identity)));
		}

		// Instantiate and add wagon to obstacle List<GameObject>
		for (int i = 30; i < 35; i++) 
		{
			position = new Vector3(Random.Range(10, 90), 0, Random.Range(10, 90));
			obstacles.Add((GameObject)(Instantiate(wagon, position, Quaternion.identity)));
		}
		for (int i = 35; i < numObs; i++) 
		{
			position = new Vector3(Random.Range(10, 90), 0, Random.Range(10, 90));
			obstacles.Add((GameObject)(Instantiate(tomb1, position, Quaternion.identity)));
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		humanText.text = "Humans: " + numHumans;
		zombieText.text = "Zombies: " + numZombies;

		if (Input.GetKeyDown(KeyCode.D)) 
		{
			if (debugLines) 
			{
				debugLines = !debugLines;
				foreach (GameObject human in humans) 
				{
					human.GetComponent<HumanManager> ().useGUI = false;
				}
				foreach (GameObject zombie in zombies) 
				{
					zombie.GetComponent<ZombieManager> ().useGUI = false;
				}
			}
			else
			{
				debugLines = !debugLines;

				foreach (GameObject human in humans) 
				{
					human.GetComponent<HumanManager> ().useGUI = true;
				}
				foreach (GameObject zombie in zombies) 
				{
					zombie.GetComponent<ZombieManager> ().useGUI = true;
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(camView == 0)
			{
				camView = 1;
				for(int i = 0; i < numZombies; i++)
				{
					Camera.main.GetComponent<SmoothFollow>().target = zombies[i].transform;
				}
			}
			else if(camView == 1)
			{
				camView = 2;
				for(int i = 0; i < numHumans; i++)
				{
					Camera.main.GetComponent<SmoothFollow>().target = humans[i].transform;
				}
			}
			else if(camView == 2)
			{
				camView = 0;
				// Since the main camera can't be transformed to itself... or maybe it can I don't know,
				// we assign it to the other bird's eye view camera in the scene
				Camera.main.GetComponent<SmoothFollow>().target = GameObject.FindWithTag("Birb").transform; 
			}
		}
	}
}
