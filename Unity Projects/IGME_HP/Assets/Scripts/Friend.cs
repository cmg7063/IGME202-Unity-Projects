/// <summary>
/// Friend Script.
/// This script is attached to friends in Harry Potter's flock "crew" (Hermione and Ron). 
/// This script handles the steering behaviors of each friend. 
/// </summary>

using UnityEngine;
using System.Collections;

public class Friend : VehicleMovement
{

    // Attributes
	public GameObject target;

	private Vector3 ultForce;

	override public void Start ()
    {
		base.Start();

		ultForce = Vector3.zero;
	}

	protected override void CalcSteeringForces()
    {
		Vector3 fleeForce = Vector3.zero;

        if ((fleeForce = Flee()) != Vector3.zero)
        {
            ultForce += fleeForce * fleeWeight;
        }
        else
        {
            ultForce += Seek(target.transform.position + target.transform.forward * -followDistance) * seekWeight;

            ultForce += Cohesion(manager.centroidPos) * cohesionWeight;

            ultForce += Alignment() * alignWeight;
        }
		
		ultForce += (Separation (safeDist) * sepWeight);
        
		ultForce += (Boundaries() * inBoundWeight);

        // Limit to max force
        ultForce = Vector3.ClampMagnitude (ultForce, maxForce);

        // Apply force
        ApplyForce(ultForce);
	}

}
