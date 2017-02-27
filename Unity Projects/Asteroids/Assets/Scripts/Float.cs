using UnityEngine;
using System.Collections;

/// <summary>
/// Float Script
/// This script is to be attached to any gameObject that should float around
/// A script that randomizes the position of Extra GameObject and wraps it
/// </summary>

public class Float : MonoBehaviour 
{
	private float speed;
	private float timer;
	private Vector3 randomPos;

	private float objRadius;

	// Use this for initialization
	void Start () 
	{
		speed = .05f;
		objRadius = -.5f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Wrap ();

		if(timer < 1)
		{
			timer = 15;
			randomPos = new Vector3(Random.Range (0, 100), Random.Range (0, 100), 0);
		}
		transform.position = Vector3.MoveTowards (transform.position, randomPos, speed);
	
	}

	/// <summary>
	/// Wrap this instance.
	/// Keep object on screen at all times
	/// Recalculate the position of the ship based upon edges of screen
	/// </summary>
	void Wrap()
	{
		Vector3 pos = transform.position;
		// If the object exceeds the positive y axis
		if(pos.y + objRadius > Camera.main.orthographicSize)
		{
			pos.y = -(Camera.main.orthographicSize - objRadius);
		}
		// If the object exceeds the negative y axis
		if(pos.y - objRadius < -Camera.main.orthographicSize)
		{
			pos.y = -(-Camera.main.orthographicSize + objRadius);
		}
		
		float screenRatio = (float)Screen.width / (float)Screen.height;
		float widthOrtho = Camera.main.orthographicSize * screenRatio;
		
		// If the object exceeds the positive x axis
		if(pos.x + objRadius > widthOrtho)
		{
			pos.x = -(widthOrtho - objRadius);
		}
		// If the object exceeds the negative x axis
		if(pos.x - objRadius < -widthOrtho)
		{
			pos.x = -(-widthOrtho + objRadius);
		}
		transform.position = pos;
	}
	/// <summary>
	/// Raises the trigger enter2D event
	/// Destroys the GameObject once it has collided with another object
	/// </summary>
	/// <param name="collider">Collider.</param>
	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.tag != "Bullet" && collider.tag != "Large Asteroid" && collider.tag != "Small Asteroid")
		{
			Destroy (gameObject);
		}
	}
}
