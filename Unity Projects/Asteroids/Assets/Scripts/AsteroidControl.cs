using UnityEngine;
using System.Collections;

/// <summary>
/// AsteroidMovement Script
/// This script is to be attached to Asteroid prefabs
/// A script that controls the movements of the Asteroids. Wraps asteroids around the scene
/// Checks for collisions between bullet and Asteroid
/// </summary>

public class AsteroidControl : MonoBehaviour 
{
	public AudioClip destroy;

	// Attributes
	public GameObject smallAsteroid;
	public GameObject bullet;

	private SceneManager manager;

	private float speed;
	private float timer;
	private Vector3 randomPos;

	private float astRadius; // Controls how far the asteroid can be from the edge of the screen

	//Methods
	// Use this for initialization
	void Start () 
	{
		speed = .02f;
		astRadius = -.5f;

		GameObject sceneManagerObj = GameObject.FindWithTag ("GameController");
		manager = sceneManagerObj.GetComponent<SceneManager> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//CheckCollision ();
		Wrap ();

		// Move asteroids
		if(timer < 1)
		{
			timer = 15;
			randomPos = new Vector3(Random.Range (0, 50), Random.Range (0, 50), 0);
		}
		transform.position = Vector3.MoveTowards (transform.position, randomPos, speed);
	}

	/// <summary>
	/// Wrap this instance.
	/// Keep asteroids on screen at all times
	/// Recalculate the position of the ship based upon edges of screen
	/// </summary>

	void Wrap()
	{
		Vector3 pos = transform.position;
		// If the asteroid exceeds the positive y axis
		if(pos.y + astRadius > Camera.main.orthographicSize)
		{
			pos.y = -(Camera.main.orthographicSize - astRadius);
		}
		// If the asteroid exceeds the negative y axis
		if(pos.y - astRadius < -Camera.main.orthographicSize)
		{
			pos.y = -(-Camera.main.orthographicSize + astRadius);
		}
		
		float screenRatio = (float)Screen.width / (float)Screen.height;
		float widthOrtho = Camera.main.orthographicSize * screenRatio;
		
		// If the asteroid exceeds the positive x axis
		if(pos.x + astRadius > widthOrtho)
		{
			pos.x = -(widthOrtho - astRadius);
		}
		// If the asteroid exceeds the negative x axis
		if(pos.x - astRadius < -widthOrtho)
		{
			pos.x = -(-widthOrtho + astRadius);
		}
		transform.position = pos;
	}

	bool CircleCollision(GameObject gameObj1, GameObject gameObj2)
	{
		CircleCollider2D obj1Collider = gameObj1.GetComponent<CircleCollider2D> ();
		CircleCollider2D obj2Collider = gameObj2.GetComponent<CircleCollider2D> ();
		
		float radius1 = obj1Collider.radius;
		float radius2 = obj2Collider.radius;
		
		float dx = obj1Collider.transform.position.x - obj2Collider.transform.position.x;
		float dy = obj1Collider.transform.position.y - obj2Collider.transform.position.y;
		
		float distance = Mathf.Sqrt (dx * dx + dy * dy);
		
		if (distance < radius1 + radius2) 
		{
			return true;
		}
		return false;
	}

	/*
	void CheckCollision ()
	{
		if (CircleCollision (gameObject, bullet)) 
		{
			//Destroy (bullet);
			if (tag.Equals ("Large Asteroid")) 
			{
				// Spawn small asteroids
				Instantiate (smallAsteroid, new Vector3 (transform.position.x - .5f, transform.position.y - .5f, 0), Quaternion.Euler (0, 0, 90));

				// Spawn small asteroids
				Instantiate (smallAsteroid, new Vector3 (transform.position.x + .5f, transform.position.y + .0f, 0), Quaternion.Euler (0, 0, 0));
				
				// Spawn small asteroids
				Instantiate (smallAsteroid, new Vector3 (transform.position.x + .5f, transform.position.y - .5f, 0), Quaternion.Euler (0, 0, 270));

				manager.Split ();
			} 
			else 
			{
				manager.DestroyAsteroid ();
				Destroy (gameObject);
			}
			manager.IncrementScore ();
			Destroy (gameObject);
			//Debug.Log ("I hit the player!");
		}
	}
	*/

	/// <summary>
	/// Raises the trigger enter2D event
	/// Destroys the GameObject once it has collided with another object
	/// </summary>
	/// <param name="collider">Collider.</param>
	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag.Equals ("Bullet")) 
		{
			AudioSource.PlayClipAtPoint(destroy, Camera.main.transform.position);
			//manager.IncrementScore();
			Destroy(collider.gameObject);
			Destroy (gameObject);

			// This will only happen upon collision with Large Asteroid
			if (smallAsteroid != null) 
			{
				manager.Score += 20;
				manager.IncrementScore();

				// Spawn small asteroids
				Instantiate (smallAsteroid, new Vector3(transform.position.x - .5f,
				                                        transform.position.y - .5f, 0), Quaternion.Euler (0, 0, 90));
			
				// Spawn small asteroids
				Instantiate (smallAsteroid, new Vector3(transform.position.x + .5f,
				                                        transform.position.y, 0), Quaternion.Euler (0, 0, 0));
			
				// Spawn small asteroids
				Instantiate (smallAsteroid, new Vector3 (transform.position.x + .5f, transform.position.y - .5f, 0), Quaternion.Euler (0, 0, 270));
				manager.Split ();
			} 
			else 
			{
				manager.Score += 50;
				manager.IncrementScore();

				manager.DestroyAsteroid ();
			}
		}
	}
}
