/// <summary>
/// ZombieManager Script.
/// This script is attached to the Zombie prefab. It controls the steering forces of
/// the zombie which includes avoiding obstacles, staying in bounds, pursuing humans, 
/// and wandering. This script also draws the OpenGL lines OnPostRender.
/// </summary>

using UnityEngine;
using System.Collections;

public class ZombieManager : VehicleMovement 
{
	// Materials for OpenGL
	public Material green; // Forward vector
	public Material black; // Human target
	public Material blue; // Right vector
	public Material red; // Future position vector

	// Use this to toggle on/off OpenGL lines
	public bool useGUI = true;
	
    public float seekWeight = 175f;
	public float wanderWeight = 8f;
	public float maxForce = 9f;
    public float boundWeight = 100f;
    public float avoidWeight = 200f;

    public float safeDistance = 10f;

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

		for(int i = 0; i < manager.numHumans; i++)
		{
			float distance = Vector3.Distance(transform.position, manager.humans[i].transform.position);
			if (distance <= safeDistance)
			//if(manager.humans[i])
			{
				maxSpeed = 10f; // Go faster when human is within "safeDistance"
				//maxForce = 10f;
				ultForce += Pursue(manager.humans[i], safeDistance) * seekWeight;

				if(distance <= 3)
				{
					GameObject zombie = (GameObject)Instantiate(manager.zombiePref, manager.humans[i].transform.position, Quaternion.identity);
					manager.numZombies++;
					manager.zombies.Add(zombie);
					Destroy (manager.humans[i]);
					manager.humans.RemoveAt(i);
					manager.numHumans--;
				}
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

        // Clamp the ultimate force by the maximum force
        ultForce = Vector3.ClampMagnitude (ultForce, maxForce);

		// Apply the ultimate force to the Zombie's acceleration
		ApplyForce(ultForce);
	}
	public void OnRenderObject()
	{
		if(useGUI)
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
			blue.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex (transform.position);
			GL.Vertex (transform.position + transform.right * 5f);
			GL.End ();

			// Human target vector line
			black.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex (transform.position);
			GL.End ();

			// Future position vector line
			red.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex (transform.position);
			GL.Vertex (transform.position + this.velocity * 5f);
			GL.End ();
		}
	}
}
