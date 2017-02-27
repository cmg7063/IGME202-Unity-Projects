using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// SceneManager Script
/// This script is to be attached to the SceneManager
/// A script that randomly generates Asteroids in the scene, the sprite image is chosen at random
/// Controls the UI events such as score, lives, level
/// Checks player lives, splits asteroids, and checks player score
/// </summary>

public class SceneManager : MonoBehaviour 
{
	// Attributes 
	// These are the GameObjects for the character selection
	public GameObject eleven;
	public GameObject lucas;
	public GameObject dustin;
	
	public int savedPlayer = 0;

	// This is for the lives Health Bar UI
	public SpriteRenderer lifeSprite;
	public Sprite[] waffles;

	// Powerup GameObjects
	public GameObject pudding;
	public GameObject slingShot;


	// Information pertaining to player stats in game
	private int score;
	private int hiScore;
	private int lives;
	private int level;
	private int levelIncrement;

	public int numAsteroids;
	public List<GameObject> asteroidPrefab;
	public GameObject smallAsteroid;
	public GameObject largeAsteroid;

	// UI text info
	public Text scoreText;
	public Text livesText;
	public Text levelText;
	public Text hiScoreText;

	// Properties
	public int Score
	{
		get{ return score;}
		set{ score = value;}
	}

	// Methods
	// Use this for initialization
	void Start () 
	{
		lucas = GameObject.FindGameObjectWithTag("Player1");
		eleven = GameObject.FindGameObjectWithTag("Player2");
		dustin = GameObject.FindGameObjectWithTag("Player3");
		
		// Grab the saved data for each player and load 
		savedPlayer = PlayerPrefs.GetInt ("selectedChar");
		
		// If player did not choose a character, default as 11
		if(savedPlayer == 0)
		{
			lucas.SetActive(false);
			eleven.SetActive(true);
			dustin.SetActive(false);
		}
		// If they chose Lucas
		if(savedPlayer == 1)
		{
			lucas.SetActive(true);
			eleven.SetActive(false);
			dustin.SetActive(false);
		}
		// If they chose 11
		if(savedPlayer == 2)
		{
			lucas.SetActive(false);
			eleven.SetActive(true);
			dustin.SetActive(false);
		}
		// If they chose Dustin
		if(savedPlayer == 3)
		{
			lucas.SetActive(false);
			eleven.SetActive(false);
			dustin.SetActive(true);
		}

		levelIncrement = 4; // As the levels progress, the number of asteroids will increase by multiples of 4

		// Add the small and large asteroid GameObjects to the list of prefabs
		asteroidPrefab.Add (smallAsteroid);
		asteroidPrefab.Add (largeAsteroid);

		hiScore = PlayerPrefs.GetInt ("hiScore", 0);
		StartGame ();
	}

	//Methods
	/// <summary>
	/// Starts the game
	/// Sets the variables to how they should be at the start of the game
	/// Sets the text objects and properties
	/// </summary>
	void StartGame()
	{
		score = 0;
		lives = 3;
		level = 1;

		scoreText.text = "Score: " + score;
		hiScoreText.text = "HiScore: " + hiScore;
		livesText.text = "Lives: ";
		lifeSprite.sprite = waffles [0];
		levelText.text = "Level: " + level;

		AsteroidGeneration ();
	}

	/// <summary>
	/// Asteroids the generation
	/// This method determines how many asteroids are required per level
	/// Spawns asteroids at random positions using Random.Range
	/// Chooses random asteroid prefab from asteroidPrefab list
	/// </summary>
	void AsteroidGeneration()
	{
		ExistingAsteroid ();
		numAsteroids = (level * levelIncrement);

		for(int i = 0; i < numAsteroids; i++)
		{
			// Instantiate a random asteroid prefab
			Instantiate(asteroidPrefab[Random.Range (0, asteroidPrefab.Count)], new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(-6.0f, 6.0f), 0), Quaternion.Euler(0,0,Random.Range(-0.0f, 359.0f)));
		}
		levelText.text = "Level: " + level;

		if (level > 1) 
		{
			Instantiate(pudding, new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(-6.0f, 6.0f), 0), Quaternion.Euler(0,0,Random.Range(-0.0f, 359.0f)));
		}
	}

	/// <summary>
	/// Score this instance
	/// Score is incremented and the text is updated and checked to see if a new hi-score has been reached
	/// </summary>
	public void IncrementScore()
	{
		//score++;
		scoreText.text = "Score: " + score;

		// If the score is greater than the saved hi-score
		if(score > hiScore)
		{
			hiScore = score; // Update hi-score
			hiScoreText.text = "Hi Score: " + hiScore;

			PlayerPrefs.SetInt("hiScore", hiScore);
		}
	}

	/// <summary>
	/// Minuses the lives
	/// Controls the player's lives and updates sprites accordingly
	/// </summary>
	public void MinusLives()
	{
		lives--;

		livesText.text = "Lives:";

		if(lives == 2)
		{
			lifeSprite.sprite = waffles[1];
		}
		if(lives == 1)
		{
			lifeSprite.sprite = waffles[2];
		}

		// If the Player's lives has reached 0, restart game
		if(lives < 1)
		{
			lifeSprite.sprite = null;
			Application.LoadLevel("GameOver");
		}
	}

	/// <summary>
	/// Split this instance
	/// Adds to the number of asteroids remaining after a large asteroid has been hit
	/// </summary>
	public void Split()
	{
		numAsteroids += 2;
	}

	/// <summary>
	/// Destroys the asteroids
	/// </summary>
	public void DestroyAsteroid()
	{
		numAsteroids--;
	}

	/// <summary>
	/// Existings the asteroid
	/// Gather all the asteroids i nan array and loop through them
	/// Called when a new game is started
	/// </summary>
	void ExistingAsteroid()
	{
		GameObject [] largeAsteroids = GameObject.FindGameObjectsWithTag("Large Asteroid");

		foreach(GameObject obj in largeAsteroids)
		{
			GameObject.Destroy(obj);
		}

		GameObject [] smallAsteroids = GameObject.FindGameObjectsWithTag("Small Asteroid");
		
		foreach(GameObject obj in smallAsteroids)
		{
			GameObject.Destroy(obj);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		Debug.Log (numAsteroids);

		// If the Player has destroyed all the asteroids, go on to next level
		if(numAsteroids < 1)
		{
			level++;
			AsteroidGeneration();
		}
		//Debug.Log ("Lives: " + lives);
	}
}
