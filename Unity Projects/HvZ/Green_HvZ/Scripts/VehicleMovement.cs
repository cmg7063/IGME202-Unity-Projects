/// <summary>
/// VehicleMovement Script.
/// This script is attached to the human and zombie prefabs. This script controls the movement of the
/// GameObject based on incoming forces and handles the methods involving Wander(), Boundaries(), 
/// and AvoidObstacles().
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class VehicleMovement : MonoBehaviour 
{
	// Vectors for force movement
	public Vector3 velocity;
	public Vector3 acceleration;
	public Vector3 direction;
	public Vector3 position;

	// Floats for force movement
	public float mass;
	public float maxSpeed;
	
	// Radius around vehicle
	private float radius = 1f;

	public float wanderOffset = 1.5f;
	public float wanderRadius = 4;
	public float wanderRate = 0.4f;
	private float wanderOr = 0;

    CharacterController charCtrl;

    protected SceneManager manager;

    public Vector3 Velocity
    {
        get { return velocity; }
    }

	// Use this for initialization
	virtual public void Start () 
	{
		manager = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
        charCtrl = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	protected void Update () 
	{
		charCtrl.Move (velocity * Time.deltaTime);
		CalcSteeringForces ();
		UpdatePosition ();
		SetTransform ();
	}

	// CalcSteeringForces() handled in HumanManager Script and ZombieManager Script
    abstract public void CalcSteeringForces();

    /// <summary>
    /// UpdatePosition
    /// Calculate the velocity and resulting position of a vehicle
    /// based on any forces
    /// </summary>
    protected void UpdatePosition()
	{
		// Step 0: Start my local position with the same position as the transform component
		position = transform.position;

		// Step 1: Add acceleration vector  * time to velocity
		velocity += acceleration * Time.deltaTime;

		// Step 2: Add velocity to position
		position += velocity * Time.deltaTime;

		// Step 3: Derive direction from velocity
		direction = velocity.normalized;

		// Step 4: Reset acceleration
		acceleration = Vector3.zero;  // (new Vector3(0, 0, 0)  *0
	}

	/// <summary>
	/// Applies any Vector3 force to the acceleration vector
	/// </summary>
	/// <param name="force">Force.</param>
	protected void ApplyForce(Vector3 force)
	{
		acceleration += force / mass;
	}

	/// <summary>
	/// Applies a friction force to this vehicle
	/// </summary>
	protected void ApplyFriction(float coeff)
	{
		// Step 1: Get the negative velocity
		Vector3 friction = velocity * -1;

		// Step 2: Normalize so its independent of velocity's magnitude
		friction.Normalize ();

		// Step 3: Multiple by coefficient of friction
		friction = friction * coeff;

		// Step 4: Apply to acceleration
		acceleration += friction;
	}

	/// <summary>
	/// Applies any force like it would gravity:
	/// It's added to acceleration independent of the mass
	/// </summary>
	/// <param name="force">Force.</param>
	protected void ApplyGravity(Vector3 force)
	{
		acceleration += force;
	}

	/// <summary>
	/// Sets the transform component to the local positon
	/// </summary>
	protected void SetTransform()
	{
		// Set the "up" vector to this vehicle's direction
		transform.forward = direction;
		transform.position = position;
	}
	/// <summary>
	/// Seek the specified targetPosition.
	/// Calculates the sterring force toward a target's position.
	/// </summary>
	/// <param name="targetPosition">Target position.</param>
	protected Vector3 Seek(Vector3 targetPosition)
	{
		// Step 1: Calculate desired velocity
		// This is a vector from myself to my target
		Vector3 desiredVelocity = targetPosition - position;
		
		// Step 2: Scale to maxSpeed
		desiredVelocity.Normalize ();
		desiredVelocity *= maxSpeed;
		
		// Step 3: Calculate the steering force
		// This is a vector from the head of current ot the head of velocity
		Vector3 steeringForce = desiredVelocity - velocity;
		
		// Step 4: Return the steering force
		return steeringForce;
	}
	
	/// <summary>
	/// Pursue the specified vehicle.
	/// Calculates the steering force toward a target's estimated future position.
	/// </summary>
	/// <param name="vehicle">Vehicle.</param>
	public Vector3 Pursue(GameObject target, float predictDist)
	{
		Vector3 predictedPos = target.transform.position + target.transform.forward * predictDist;
		return Seek (predictedPos);
	}
	
	/// <summary>
	/// Flee this instance.
	/// </summary>
	protected Vector3 Flee(Vector3 targetPosition)
	{
		Vector3 desiredVelocity = position - targetPosition;
		desiredVelocity.Normalize();
		desiredVelocity*= maxSpeed;
		
		Vector3 steeringForce = desiredVelocity - velocity;
		
		return steeringForce;
		//transform.position = Vector3.Lerp (transform.position, transform.position - target.transform.position, Time.deltaTime * maxSpeed);
	}
	
	/// <summary>
	/// Evade the specified vehicle.
	/// Calculates steering force away from a target's estimated future position.
	/// </summary>
	/// <param name="vehicle">Vehicle.</param>
	protected Vector3 Evade(Vector3 targetPosition, Vector3 targetVelocity)
	{
		Vector3 distance = targetPosition - transform.position;
		float lookAhead = distance.magnitude / maxSpeed;
		Vector3 futurePos = targetPosition + targetVelocity * lookAhead;
		
		return Flee (futurePos);
	}
	
	/// <summary>
	/// Wander this instance.
	/// Calculate a sterring force to a random point.
	/// </summary>
	public Vector3 Wander()
	{
		float charOr = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

		wanderOr += (Random.value - Random.value) * wanderRate;

		float targetOr = wanderOr + charOr;

		Vector3 targetPos = transform.position + (orientToVector (targetOr) * wanderRadius);
		return Seek (targetPos);
	}

	/// <summary>
	/// Orientations to vector.
	/// Return orientation of target as a Vector3.
	/// </summary>
	/// <returns>The to vector.</returns>
	/// <param name="orientation">Orientation.</param>
	public Vector3 orientToVector(float orientation) 
	{
		return new Vector3(Mathf.Cos(orientation), Mathf.Sin(orientation), 0);
	}

    /// <summary>
    /// Boundaries this instance.
    /// Returns the corresponding steering force to go back to the center is obj is too far.
    /// </summary>
	protected Vector3 Boundaries()
	{
		if(transform.position.x > 90 || transform.position.x < 10)
		{
			return Seek (new Vector3(50, 0, 50));
		}
		else if(transform.position.z > 90 || transform.position.z < 10)
		{
			return Seek (new Vector3(50, 0, 50));
		}
		return new Vector3 ();
	}

	/// <summary>
	/// Avoids the obstacle.
	/// </summary>
	/// <returns>The obstacle.</returns>
	/// <param name="obst">Obst.</param>
	/// <param name="safeDistance">Safe distance.</param>
	public Vector3 avoidObstacle(GameObject obst, float safeDistance)
	{
		Vector3 steer = new Vector3 (0, 0, 0);

		Obstacle obstacle = obst.GetComponent<Obstacle> ();

		// Create vecToCenter - a vector from the character to the center of the obstacle
		// Find the distance to the obstacle 
		Vector3 vecToCenter = obst.transform.position - transform.position;

		// Return a zero vector if the obstacle is too far to concern us
		// Use safe distance to determine how large the "safe zone" is
		if(vecToCenter.magnitude > safeDistance)
		{
			return Vector3.zero;
		}
		// Return a zero vector if the obstacle is behind us
		// (dot product of vecToCenter and forward is negative)
		float dotB = Vector3.Dot (vecToCenter, transform.forward);
		if(dotB < 0)
		{
			return Vector3.zero;
		}

		// Use the dot product of the vector to obstacle center (vecToCenter) and the unit vector
		// to the right (right) of the vehicle to find the projected distance between the centers
		// of the vehicle and the obstacle
		// Compare this to the sum of the radii and return a zero vector if we can pass safely
		float dotR = Vector3.Dot (vecToCenter, transform.right);
		if(radius + obstacle.Radius < Mathf.Abs(dotR))
		{
			return Vector3.zero;
		}

		// If we get this far we are on a collision course and must steer away!
		// Use the sign of the dot produdct between the vector to center (vecToCenter) and the
		// vector to the right (right) to determine whether to steer left or right
		// Calculate desired velocity using the right vector or negated right vector and maxSpeed
		if(dotR > 0) // On right, move left
		{
			steer = (transform.right * -1) * maxSpeed; 
		}
		else // On left, move right
		{
			steer = transform.right * maxSpeed;
		}

		return steer;
	}
}
