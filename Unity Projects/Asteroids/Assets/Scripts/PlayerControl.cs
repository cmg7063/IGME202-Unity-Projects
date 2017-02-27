using UnityEngine;
using System.Collections;

/// <summary>
/// ShipMovement Script
/// This script is to be attached to the Ship prefab
/// A script that moves the Ship using vector-based movement. Wraps the ship around the screen
/// </summary>

public class PlayerControl : MonoBehaviour 
{
	// Attributes
	// SFX audio files
	public AudioClip shoot;
	public AudioClip hit;
	public AudioClip powerUp;

	private Vector3 position;
	private Vector3 velocity;
	private Vector3 direction;
	private Vector3 acceleration;

	// These two attributes  will prevent bullet rapid fire 
	private float fireDelay;
	private float fireTimer;

	public GameObject bullet;
	public GameObject smallAsteroid;
	public GameObject largeAsteroid;

	private SceneManager manager;

	// Rate of movement for the ship
	private float maxSpeed;

	private float totalRotation;
	private float accelRate;
	
	// Array of renderers for Wrap()
	private Renderer[] renderers;
	private bool isWrappingX;
	private bool isWrappingY;

	// Methods
	// Use this for initialization
	void Start () 
	{
		GameObject sceneManagerObj = GameObject.FindWithTag ("GameController");
		manager = sceneManagerObj.GetComponent<SceneManager> ();

		fireDelay = .25f;

		position = new Vector3 (0, 0, 0);
		velocity = new Vector3 (0, 0, 0);
		direction = new Vector3 (0, 1, 0);
		acceleration = new Vector3 (0, 0, 0);

		maxSpeed = 1f;
		accelRate = .1f;
		
		isWrappingX = false;
		isWrappingY = false;
		renderers = GetComponentsInChildren<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//CheckCollision ();
		Wrap ();
		Drive ();
		SetTransform ();
		RotateShip ();

		fireTimer -= Time.deltaTime;

		if(Input.GetKey(KeyCode.Space) && fireTimer <= 0)
		{
			Shoot();
			fireTimer = fireDelay;
		}
	}

	void Shoot()
	{
		// Spawn a bullet
		Instantiate(bullet, new Vector3(transform.position.x,transform.position.y, 0), transform.rotation);
		AudioSource.PlayClipAtPoint (shoot, Camera.main.transform.position);
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
	/// <summary>
	/// Raises the trigger enter2D event
	/// Destroys the GameObject once it has collided with another object
	/// </summary>
	/// <param name="collider">Collider.</param>
	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag != "Bullet" && collider.gameObject.tag != "Extra")
		{
			// Play SFX
			AudioSource.PlayClipAtPoint(hit, Camera.main.transform.position);
			transform.position = new Vector3 (0, 0, 0); // This SHOULD center the player when they are hit, but I don't think it is doing that...

			manager.MinusLives ();
			//Debug.Log ("I hit an asteroid");
		}
		if(collider.gameObject.tag.Equals("Extra"))
		{
			AudioSource.PlayClipAtPoint(powerUp, Camera.main.transform.position);
			manager.Score += 100;
			manager.IncrementScore();
		}
	}
	/*
	void CheckCollision ()
	{
		if (CircleCollision (this.gameObject, smallAsteroid) || CircleCollision (this.gameObject, largeAsteroid)) 
		{
			transform.position = new Vector3 (0, 0, 0);
			
			manager.MinusLives ();
			//Debug.Log ("I hit an asteroid");
		}
	}
	*/

	/// <summary>
	/// Rotates the ship.
	/// Changes the ship's direction when the keys are pressed
	/// </summary>
	void RotateShip()
	{
		// Press left arrow, rotate 1 degree to left
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			direction = Quaternion.Euler(0, 0, 2) * direction;
			totalRotation += 2f;
		}
		// Press right arrow, rotate 1 degree to right
		if(Input.GetKey(KeyCode.RightArrow))
		{
			direction = Quaternion.Euler(0, 0, -2) * direction;
			totalRotation -= 2f;
		}
	}

	/// <summary>
	/// Drive this instance.
	/// Calculate the velocity based upon acceleration and deceleration
	/// Updates the position of the vehicle based on velocity vector
	/// </summary>
	void Drive()
	{
		// Press up arrow, accelerate
		if (Input.GetKey (KeyCode.UpArrow)) 
		{
			acceleration = accelRate * direction * Time.deltaTime;
			velocity += acceleration;
			velocity = Vector3.ClampMagnitude (velocity, maxSpeed); // Limit velocity
			// Add velocity to ship's position
			position += velocity;
		} 
		else 
		{
			velocity = velocity * .95f;
			if(velocity.magnitude < .001)
			{
				velocity = Vector3.zero;
			}
			position += velocity;
		}
	}

	/// <summary>
	/// Sets the transform.
	/// Update positioin and rotation of vehicle
	/// </summary>
	void SetTransform()
	{
		transform.rotation = Quaternion.Euler (0, 0, totalRotation);

		// Transform object to calcualted position
		transform.position = position;
	}

	/// <summary>
	/// Wrap this instance.
	/// Keep ship on screen at all times
	/// Recalculate the position of the ship based upon edges of screen
	/// </summary>
	void Wrap()
	{
		bool isVisible = CheckRenderers();
		
		if(isVisible)
		{
			isWrappingX = false;
			isWrappingY = false;
			return;
		}
		if(isWrappingX && isWrappingY)
		{
			return;
		}
		
		position = transform.position;
		
		if(position.x > 1 || position.x < 0)
		{
			position.x = -position.x;
			isWrappingX = true;
		}
		if(position.y > 1 || position.y < 0)
		{
			position.y = -position.y;
			isWrappingY = true;
		}
		transform.position = position;
	}

	/// <summary>
	/// Checks the renderers.
	/// </summary>
	/// <returns><c>true</c>, if renderers was checked, <c>false</c> otherwise.</returns>
	bool CheckRenderers()
	{
		foreach (Renderer renderer in renderers) 
		{
			// If at least one render is visible, return true
			if (renderer.isVisible) 
			{
				return true;
			}
		}
		// Otherwise, the object is invisible
		return false;
	}
}
