/// <summary>
/// Enemy Script.
/// This script is attached to Monstrous Book of Monsters that seeks/chases after Harry's group of friends.
/// This script handles the steering behaviors of each friend. 
/// </summary>

using UnityEngine;
using System.Collections;

public class Enemy: VehicleMovement
{
	
	// Attributes
	public GameObject seekerTarget;
	
	private Vector3 ultForce;

    // Use this for initialization
    override public void Start ()
    {
		base.Start();

		ultForce = Vector3.zero;
	}

	protected override void CalcSteeringForces()
    {
		Vector3 seekForce = Vector3.zero;
        if ((seekForce = SeekClosest()) != Vector3.zero)
        {
            ultForce += seekForce;
        }
        else
        {
            ultForce += Wander() * seekWeight;
        }

		ultForce += Boundaries () * inBoundWeight;

        // Limit to max force
        ultForce = Vector3.ClampMagnitude(ultForce, maxForce);

        // Apply force
        ApplyForce(ultForce);
    }
}
