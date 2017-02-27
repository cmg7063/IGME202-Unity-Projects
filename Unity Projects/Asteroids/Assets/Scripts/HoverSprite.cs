using UnityEngine;
using System.Collections;

/// <summary>
/// HoverSprite Script
/// This script is to be attached to the character sprite selectors
/// A script that simply changes the sprite image on hover
/// </summary>

public class HoverSprite : MonoBehaviour 
{
	public GameObject hoverSprite;
	private Vector3 currentPos;
	
	void OnMouseOver()
	{
		currentPos = transform.position;

		gameObject.GetComponent<SpriteRenderer> ().enabled = false;

		hoverSprite.transform.position = currentPos;

		hoverSprite.GetComponent<SpriteRenderer> ().enabled = true;
	}

	void OnMouseExit()
	{
		gameObject.GetComponent<SpriteRenderer> ().enabled = true;
		hoverSprite.GetComponent<SpriteRenderer> ().enabled = false;
	}

	void Start()
	{
		hoverSprite.GetComponent<SpriteRenderer> ().enabled = false;
	}
}
