/// <summary>
/// Leader Script.
/// This script is attached to Harry, the leader of his flocking group.
/// This script handles the steering behaviors of each friend. 
/// </summary>

using UnityEngine;
using System.Collections;

public class Leader: VehicleMovement
{
    // Attributes
    public GameObject target;

    private Vector3 ultForce;

    // Use this for initialization
    override public void Start()
    {
        base.Start();

        ultForce = Vector3.zero;
    }

    protected override void CalcSteeringForces()
	{
		Vector3 fleeForce = Vector3.zero;

		if ((fleeForce = Flee ()) != Vector3.zero)
        {	
			ultForce += fleeForce * fleeWeight;	
		} 
		else
        {
            if (forceYes == true)
            {
                ultForce += CalculatePath() * seekWeight;
            }
		}
		ultForce += Boundaries () * inBoundWeight;

        // Limit to max force
        ultForce = Vector3.ClampMagnitude(ultForce, maxForce);

        // Apply force
        ApplyForce(ultForce);
    }
}
