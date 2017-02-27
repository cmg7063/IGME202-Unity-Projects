/// <summary>
/// HumanManager Script.
/// This script is attached to the Human prefab. It controls the steering forces of
/// the human which includes avoiding obstacles, staying in bounds, evading zombies, 
/// and wandering. This script also draws the OpenGL lines OnPostRender.
/// </summary>

using UnityEngine;
using System.Collections;

public class HumanManager : VehicleMovement 
{
	// Materials for OpenGL
	public Material green; // Forward vector
	public Material blue; // Right vector
	public Material purple; // Future position

	// Use this to toggle on/off OpenGL lines
	public bool useGUI = true;

	public float wanderWeight = 10f;
	public float fleeWeight = 200f;
    public float boundWeight = 100f;
    public float avoidWeight = 200f;
	public float maxForce = 8f;

	public float safeDistance = 20f;
	
	// Use this for initialization
	public override void Start () 
	{
		base.Start ();
	}

	/// <summary>
	/// Calculates the steering forces.
	/// Overrides the abstract method in VehicleMovement().
	/// </summary>
	public override void CalcSteeringForces()
	{
		// Create a new ultimate force that's zeroed out
		Vector3 ultForce = Vector3.zero;

		// If a zombie is too close, add the Flee force to ultimate force 
		// which is weighted by the Flee Weight
		for(int i = 0; i < manager.numZombies; i++)
		{
			float distance = Mathf.Abs(Vector3.Distance (transform.position, manager.zombies[i].transform.position));
			if(distance < safeDistance)
			//if(manager.zombies[i])
			{
				ultForce += Evade(manager.zombies[i].transform.position,manager.zombies[i].GetComponent<VehicleMovement>().Velocity)* fleeWeight;
			}
			else
			{
				ultForce += Wander() * wanderWeight;
			}
		}
        // Avoid obstacles
		for(int i = 0; i < manager.numObs; i++)
		{
			ultForce += avoidObstacle((GameObject)manager.obstacles[i], safeDistance) * avoidWeight;
		}

		// This stuff handles the horizontal running/models moving all weird and stuff :(
		if(transform.position.y < 1 || transform.position.y > 1)
		{
			transform.position = new Vector3 (transform.position.x, 1, transform.position.z);
		}
		if(transform.rotation.x > 0 || transform.rotation.y > 0 || transform.rotation.z > 0)
		{
			transform.rotation = Quaternion.identity;
		}
		else if(transform.rotation.x < 0 || transform.rotation.y < 0 || transform.rotation.z < 0)
		{
			transform.rotation = Quaternion.identity;
		}

        // Stay in bounds of park
		//ultForce += Boundaries(15, new Vector3(Random.Range (10, 90), Terrain.activeTerrain.SampleHeight(this.position), Random.Range (10, 90))) * boundWeight;
		ultForce += Boundaries () * boundWeight;

		// Clamp the ultimate force to Maximum Force
		ultForce = Vector3.ClampMagnitude (ultForce, maxForce);

		// Apply the ultimate force to the Human's acceleration
		ApplyForce (ultForce);
	}

	/// <summary>
	/// Raises the render object event.
	/// Draws the OpenGL lines in Game view by default.
	/// Toggle on/off with boolean useGUI
	/// </summary>
	public void OnRenderObject()
	{
		if (useGUI) 
		{
			// Set the material to be used for the first line
			green.SetPass (0);
			// Forward vector line
			GL.Begin (GL.LINES);
			GL.Vertex (transform.position);
			GL.Vertex (transform.position + transform.forward * 5f);
			//GL.Vertex(new Vector3(1f, transform.position.y, transform.position.z));
			GL.End ();
		
			// Right vector line
			blue.SetPass (0);
			GL.Begin (GL.LINES);
			GL.Vertex (transform.position);
			GL.Vertex (transform.position + transform.right * 5f);
			GL.End ();
		
			// Future position vector line
			purple.SetPass (0);
			GL.Begin (GL.LINES);
			GL.Vertex (transform.position); // try direction not vel
			GL.Vertex (transform.position + this.velocity * 5f);
			GL.End ();
		}
	}
}
