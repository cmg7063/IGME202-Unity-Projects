/// <summary>
/// VehicleMovement Script.
/// This script is attached to enemy and friend prefabs. 
/// This script controls the basic movement of the prefabs based on incoming forces.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class VehicleMovement : MonoBehaviour
{
    // Attributes
    protected Vector3 acceleration;
    protected Vector3 velocity;
	protected Vector3 desired;

    protected Vector3 futurePost;

    public int followDistance = 0;

    public float maxSpeed;
    public float maxForce;
    public float mass;
    public float radius;
    public float safeDist;
    public bool forceYes = true;

    public int friendNum = 0;
	private Vector3 wanderVector = Vector3.zero;
	private int wanderOffset = 0;

    // Properties
    public Vector3 Velocity
    {
        get { return velocity; }
    }

    CharacterController charControl;

    protected SceneManager manager;
    public float safeDistance;
    public float avoidWeight;
    public float sepWeight;
    public float seekWeight;
    public float alignWeight;
	public float cohesionWeight;
	public float inBoundWeight;
	public float fleeWeight;

    // Use this for initialization
    virtual public void Start()
    {
        acceleration = Vector3.zero;
        velocity = transform.forward;
        desired = Vector3.zero;

        charControl = GetComponent<CharacterController>();
        manager = GameObject.Find("SceneManager").GetComponent<SceneManager>();

    }

    // Update is called once per frame
    protected void Update()
    {
        //Calculate all necessary steering forces
        CalcSteeringForces();

        //Add acceleration to velocity
        velocity += acceleration * Time.deltaTime;

        velocity.y = 0;

        //Limit velocity magnitude to max speed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        //Move position by current velocity
        charControl.Move(Velocity * Time.deltaTime);

        //Reset acceleration
        acceleration = Vector3.zero;

        //Set forward vector so that it faces in direction of velocity
        transform.forward = velocity.normalized;
    }

    abstract protected void CalcSteeringForces();

    /// <summary>
    /// Applies any Vector3 force to the acceleration vector
    /// </summary>
    /// <param name="force">Force.</param>
    protected void ApplyForce(Vector3 steeringForce)
    {
        acceleration += steeringForce / mass;
    }

    /// <summary>
    /// Seek the specified targetPosition.
    /// Calculates the sterring force toward a target's position.
    /// </summary>
    /// <param name="targetPosition">Target position.</param>
    protected Vector3 Seek(Vector3 targetPosition)
    {
        // Step 1: Calculate desired velocity
        desired = targetPosition - transform.position;
        desired.Normalize();

        // Step 2: Scale to maxSpeed
        desired *= maxSpeed;
        Vector3 steeringForce = desired - velocity;

        // Step 4: Return the steering force to seek target position
        return steeringForce;
    }

    /// <summary>
    /// Flee this instance.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    protected Vector3 Flee(Vector3 targetPosition)
    {
        Vector3 fleeForce = Seek(targetPosition);
        fleeForce.x *= -1;
        fleeForce.z *= -1;

        return (fleeForce);
    }

    /// <summary>
    /// Flee this instance.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    protected Vector3 Flee()
	{
		Vector3 fleeForce = Vector3.zero;
		
		float distance = Vector3.Distance(manager.mbom.transform.position, transform.position);
		if (distance < 30)
        {
			fleeForce = Seek (manager.mbom.transform.position);
			fleeForce.x *= -1;
			fleeForce.z *= -1;
		}	
		return (fleeForce);
	}

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected Vector3 PathFollow(Waypoint waypoint, Vector3 normalPoint)
    {
        Vector3 follow = Vector3.zero;
        float distance = Vector3.Distance(futurePost, normalPoint);

        if (distance > manager.pathRadius)
        {
            Vector3 B = waypoint.result.normalized;
            B *= 25;
            normalPoint += B;
            follow = Seek(normalPoint);
        }
        return follow;
    }

	/// <summary>
    /// Find the closest node in the path.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="waypoint"></param>
    /// <returns></returns>
    public Vector3 ClosestNode(Vector3 point, Waypoint waypoint)
    {
        Vector3 A = point - waypoint.startPoint;

        Vector3 C = waypoint.endPoint - waypoint.startPoint;
        C.Normalize();

        C *= (Vector3.Dot(A, C));

        Vector3 normalPoint = waypoint.startPoint + C;

        return normalPoint;
    }

	/// <summary>
    /// Calculate closest path and seek it.
    /// </summary>
    /// <returns></returns>
    public Vector3 CalculatePath()
    {
        Vector3 pathF = Vector3.zero;

        futurePost = transform.position + (transform.forward * 2);

        int j = 0;
        float shortestDist = 100;
        Vector3 normalPoint = Vector3.zero;

        //Find closest path
        for (int i = 0; i < manager.Paths.Count; i++) {
            Waypoint waypoint = ((GameObject)manager.Paths[i]).GetComponent<Waypoint>();

            Vector3 norm = ClosestNode(futurePost, waypoint);

            if ((waypoint.startPoint.x < norm.x && norm.x < waypoint.endPoint.x) || (waypoint.endPoint.x < norm.x && norm.x < waypoint.startPoint.x))
            { }
            else {
                float distA = Vector3.Distance(futurePost, waypoint.startPoint);
                float distB = Vector3.Distance(futurePost, waypoint.endPoint);

                if (distA < distB)
                {
                    norm = waypoint.startPoint;
                }
                else
                {
                    norm = waypoint.endPoint;
                }
            }

            float distance = Vector3.Distance(futurePost, norm);

            Debug.DrawLine(futurePost, norm, Color.white);

            if (distance < shortestDist)
            {
                shortestDist = distance;
                j = i;
                normalPoint = norm;
            }
        }
        pathF = PathFollow(((GameObject)manager.Paths[j]).GetComponent<Waypoint>(), normalPoint);

        return pathF;
    }

    /// <summary>
    /// Separates this instance.
    /// Keeps the flock a specific distance away from each other; each flocker has a "personal bubble".
    /// </summary>
    /// <returns>Desired velocity.</returns>
    public Vector3 Separation(float separationDistance)
    {
        Vector3 totalVelocity = new Vector3(0, 0, 0);
        float distance;

        if (friendNum == 3)
        {
            for (int i = 0; i < manager.Friends.Count; i++)
            {
                distance = Vector3.Distance(((GameObject)manager.Friends[i]).transform.position, transform.position);
                distance = Mathf.Abs(distance);

                //If distance is close, but not identical
                if ((distance < separationDistance) && (distance != 0))
                {
                    Vector3 tempVelocity = (Flee(((GameObject)manager.Friends[i]).transform.position)).normalized;
                    tempVelocity.y = 0;
                    tempVelocity *= (1 / distance);
                    totalVelocity += tempVelocity;
                }
            }
        }
        return ((totalVelocity.normalized * maxSpeed) - velocity);
    }

    /// <summary>
    /// Aligns flocker with flocks average direction.
    /// </summary>
    /// <param name="alignVector"></param>
    /// <returns>Desired velocity.</returns>
    public Vector3 Alignment()
    {
        Vector3 totalForward = new Vector3(0, 0, 0);

        if (friendNum == 3)
        {
            for (int i = 0; i < manager.Friends.Count; i++)
            {
                totalForward += ((GameObject)manager.Friends[i]).transform.forward;
            }
        }
        totalForward.Normalize();
        totalForward *= maxSpeed;

        return (totalForward - velocity);
    }

    /// <summary>
    /// Keeps the flock close together.
    /// </summary>
    /// <param name="cohesionVector"></param>
    /// <returns></returns>
    public Vector3 Cohesion(Vector3 cohesionVector)
    {
        Vector3 vecToCenter = Seek(cohesionVector);

        return vecToCenter;
    }

    /// <summary>
    /// Applies force to object to stay in bounds and avoid collisions with terrain.
    /// </summary>
    /// <returns>Force to apply to object.</returns>
	public Vector3 Boundaries()
    {
		if (transform.position.x > 290 || transform.position.x < 10)
        {
			return Seek (new Vector3(50, 1, 50));
		}
		else if (transform.position.z > 290 || transform.position.z < 10)
        {
			return Seek (new Vector3(50, 1, 50));
		}
        return new Vector3();
    }

	/// <summary>
    /// Monstrous Book of Monsters seeks the closest friend near it.
    /// </summary>
    /// <returns></returns>
    public Vector3 SeekClosest()
    {
		int friend = 0;
		int num = -1;
		float currentDist = 30;
		float distance = 0;

		//Cycle through list of friends to see if any friends are close
		for (int i = 0; i < manager.Friends.Count; i++)
        {
			if(((distance = Vector3.Distance(((GameObject)manager.Friends[i]).transform.position, transform.position)) < currentDist))
            {
				friend = 3;
				num = i;
				currentDist = distance;
			}
		}
		Vector3 returnForce = Vector3.zero;

		//If friends are close, seek them
		if(num != -1)
        {
			if(currentDist < 4)
            {
				if(friend == 3)
                {
					returnForce = Seek (((GameObject)manager.Friends[num]).transform.position);
					return returnForce;
				}
			}
			//Seek the proper friend from the list
			else
            {
                if (friend == 3)
                {
                    return Seek(((GameObject)manager.Friends[num]).transform.position);
                }
			}
		}
        return Vector3.zero;
    }

	/// <summary>
    /// Find a random position to wander to.
    /// </summary>
    /// <returns></returns>
    public Vector3 Wander()
    {
		if (wanderOffset > 100)
        {
			Vector3 forward = transform.forward.normalized;
			Vector3 right = transform.right.normalized;
			float rng = Random.Range(-20.0f, 20.0f);

			wanderVector = ((forward * 5) + (right * rng));
			wanderOffset = 0;
		}
		wanderOffset++;

        return Seek(wanderVector) ;
	}
}
