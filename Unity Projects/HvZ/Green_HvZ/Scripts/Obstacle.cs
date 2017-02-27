/// <summary>
/// Obstacle Script.
/// This script is attached to obstacle prefabs. 
/// It simply adds a radius to the obstacle and allows you to access that value.
/// </summary>
 
using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour 
{
	private float radius;

	// Use this for initialization
	void Start () 
	{
		radius = 5f;
	}

	public float Radius
	{
		get{ return radius;}
	}
}
