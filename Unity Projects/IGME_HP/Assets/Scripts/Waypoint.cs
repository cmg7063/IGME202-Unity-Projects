/// Waypoint Script. 
/// This script is attached to waypoint prefabs. 
/// This script calculates the midpoint between the start and end point of the path section.

using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{
    // Attributes
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Vector3 result;

	// Use this for initialization
	void Start ()
    {
        Vector3 midPoint = startPoint + endPoint;
        midPoint /= 2;

        transform.position = midPoint;      
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.DrawLine(startPoint, endPoint, Color.black);
        result = endPoint - startPoint;
	}

}
