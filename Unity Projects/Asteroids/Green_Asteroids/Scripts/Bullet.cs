using UnityEngine;
using System.Collections;

/// <summary>
/// Bullet Script
/// This script is to be applied to the Bullet prefab
/// A script that controls the bullet speed and "life"
/// </summary>

public class Bullet : MonoBehaviour 
{
	// Attributes
	public float maxSpeed;
	public float timer;

	//Methods
	// Use this for initialization
	void Start () 
	{
		// Control the speed of the bullet
		maxSpeed = 5f;

		// Controls the life of the bullet
		timer = 2f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Controls the movement of the bullet
		Vector3 position = transform.position;
		
		Vector3 velocity = new Vector3 (0, maxSpeed * Time.deltaTime, 0);
		position += transform.rotation * velocity;
		
		transform.position = position;

		timer -= Time.deltaTime;
		if(timer <= 0)
		{
			Destroy(gameObject); // Destroy bullet
		}
	}
}
