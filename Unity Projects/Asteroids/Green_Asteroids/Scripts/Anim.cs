using UnityEngine;
using System.Collections;

/// <summary>
/// SpriteAnim Script
/// This script is to be attached to the Bullet prefab
/// A script that animates the bullet by changing between sprites
/// This code was not written by me!! Found on Answers.Unity3D.com
/// </summary>

public class Anim : MonoBehaviour 
{
	public bool loop;
	public float frameSeconds = .3f;

	private SpriteRenderer spr;
	public Sprite [] sprites;
	private int frame = 0;
	private float deltaTime = 0;
	
	// Use this for initialization
	void Start () 
	{
		spr = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Keep track of the time that has passed
		deltaTime += Time.deltaTime;

		// Loop to allow for multiple sprite frame jumps in a single update call if needed. Useful if frameSeconds is very small*/
		while (deltaTime >= frameSeconds) 
		{
			deltaTime -= frameSeconds;
			frame++;
			if(loop)
			{
				frame %= sprites.Length;
			}
			//Max limit
			else if(frame >= sprites.Length)
			{
				frame = sprites.Length - 1;
			}
		}
		//Animate sprite with selected frame
		spr.sprite = sprites [frame];
	}
}
