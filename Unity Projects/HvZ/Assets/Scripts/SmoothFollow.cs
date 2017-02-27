/// <summary>
/// SmoothFollow Script.
/// This script is attached to the MainCamera and allows you to set the target of the camera.
/// I DID NOT WRITE THIS SCRIPT; IT WAS OFFERED TO US BY OUR INSTRUCTOR.
/// </summary>

using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour 
{
	public Transform target;
	public float distance = 3.0f;
	public float height = 1.50f;
	public float heightDamping = 2.0f;
	public float positionDamping =2.0f;
	public float rotationDamping = 2.0f;
	
	// Update is called once per frame
	void LateUpdate ()
	{
		// Early out if we don't have a target
		if (!target)
			return;
		
		float wantedHeight = target.position.y + height;
		float currentHeight = transform.position.y;
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		// Set the position of the camera 
		Vector3 wantedPosition = target.position - target.forward * distance;
		transform.position = Vector3.Lerp (transform.position, wantedPosition, Time.deltaTime * positionDamping);
		
		// adjust the height of the camera
		transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);
		
		
		//transform.forward = Vector3.Lerp (transform.forward, target.position - transform.position, Time.deltaTime * rotationDamping);
		transform.forward = Vector3.Lerp (transform.forward, target.forward , Time.deltaTime * rotationDamping);
		
	}
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
