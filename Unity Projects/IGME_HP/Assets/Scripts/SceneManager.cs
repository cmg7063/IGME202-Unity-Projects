/// <summary>
/// SceneManager Script.
/// This script is attached to the SceneManager empty gameobject. 
/// This script handles the generation of Harry (the leader) and Harry's crew. 
/// This script also calculates the average direction and position of Harry's crew.
/// This script also handles the camera view transitions.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour
{
    // Attributes
    public GameObject pathPrefab;
    public GameObject ronPrefab;
    public GameObject hermionePrefab;
    public GameObject harryPrefab;

    public GameObject mbomPrefab;
    public GameObject centroidPrefab;

    public Terrain terrain;

    private int camSet;
    public Camera cinematicCam;
    public Camera followCam;
    public Camera overviewCam;

    private ArrayList paths = new ArrayList();
    private ArrayList friends = new ArrayList();

    public float pathRadius = 1f;
    public Vector3 centroidPos;

    private GameObject leader;
    public GameObject mbom;

    // Properties
    public ArrayList Paths
    {
        get { return paths; }
    }
    public ArrayList Friends
    {
        get { return friends; }
    }

    // Use this for initialization
    void Start()
    {
        cinematicCam.enabled = true;
        overviewCam.enabled = false;
        followCam.enabled = false;

        StartCoroutine(Cinematic());

        Vector3 position = new Vector3(0, 0, 0);
        Vector3 startPoint = new Vector3(0, .5f, 0);
        Vector3 endPoint = new Vector3(0, .5f, 0);

        // Generate path nodes
        startPoint = new Vector3(40, 1, 20);
        endPoint = new Vector3(200, 1, 20);
        GameObject path0 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        path0.GetComponent<Waypoint>().startPoint = startPoint;
        path0.GetComponent<Waypoint>().endPoint = endPoint;
        paths.Add(path0);

        startPoint = new Vector3(200, 1, 20);
        endPoint = new Vector3(240, 1, 72);
        GameObject path1 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        path1.GetComponent<Waypoint>().startPoint = startPoint;
        path1.GetComponent<Waypoint>().endPoint = endPoint;
        paths.Add(path1);
        /*
                startPoint = new Vector3(240, 1, 40);
                endPoint = new Vector3(250, 1, 72);
                GameObject path2 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
                path2.GetComponent<Waypoint>().startPoint = startPoint;
                path2.GetComponent<Waypoint>().endPoint = endPoint;
                paths.Add(path2);

                startPoint = new Vector3(250, 1, 72);
                endPoint = new Vector3(240, 1, 72);
                GameObject path3 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
                path3.GetComponent<Waypoint>().startPoint = startPoint;
                path3.GetComponent<Waypoint>().endPoint = endPoint;
                paths.Add(path3);
         */

        startPoint = new Vector3(240, 1, 72);
        endPoint = new Vector3(240, 1, 200);
        GameObject path4 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        path4.GetComponent<Waypoint>().startPoint = startPoint;
        path4.GetComponent<Waypoint>().endPoint = endPoint;
        paths.Add(path4);

        startPoint = new Vector3(240, 1, 200);
        endPoint = new Vector3(258, 1, 200);
        GameObject path5 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        path5.GetComponent<Waypoint>().startPoint = startPoint;
        path5.GetComponent<Waypoint>().endPoint = endPoint;
        paths.Add(path5);

        startPoint = new Vector3(258, 1, 200);
        endPoint = new Vector3(258, 1, 270);
        GameObject path6 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        path6.GetComponent<Waypoint>().startPoint = startPoint;
        path6.GetComponent<Waypoint>().endPoint = endPoint;
        paths.Add(path6);

        startPoint = new Vector3(258, 1, 270);
        endPoint = new Vector3(201, 1, 270);
        GameObject path7 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        path7.GetComponent<Waypoint>().startPoint = startPoint;
        path7.GetComponent<Waypoint>().endPoint = endPoint;
        paths.Add(path7);

        startPoint = new Vector3(201, 1, 270);
        endPoint = new Vector3(201, 1, 237);
        GameObject path8 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        path8.GetComponent<Waypoint>().startPoint = startPoint;
        path8.GetComponent<Waypoint>().endPoint = endPoint;
        paths.Add(path8);

        startPoint = new Vector3(201, 1, 237);
        endPoint = new Vector3(30, 1, 237);
        GameObject path9 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        path9.GetComponent<Waypoint>().startPoint = startPoint;
        path9.GetComponent<Waypoint>().endPoint = endPoint;
        paths.Add(path9);

        startPoint = new Vector3(30, 1, 237);
        endPoint = new Vector3(30, 1, 65);
        GameObject path10 = (GameObject)Instantiate(pathPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        path10.GetComponent<Waypoint>().startPoint = startPoint;
        path10.GetComponent<Waypoint>().endPoint = endPoint;
        paths.Add(path10);

        position = new Vector3(Random.Range(35, 40), 0, Random.Range(35, 40));
        leader = (GameObject)Instantiate(harryPrefab, position, Quaternion.identity);
        leader.GetComponent<VehicleMovement>().maxSpeed--;

        position = new Vector3(Random.Range(150, 150), 0, Random.Range(150, 150));
        mbom = (GameObject)Instantiate(mbomPrefab, position, Quaternion.identity);

        position = new Vector3(Random.Range(35, 50), 0, Random.Range(35, 50));
        GameObject ron = (GameObject)Instantiate(ronPrefab, position, Quaternion.identity);
        ron.GetComponent<Friend>().target = leader;
        Friends.Add(ron);

        position = new Vector3(Random.Range(35, 50), 0, Random.Range(35, 50));
        GameObject hermione = (GameObject)Instantiate(hermionePrefab, position, Quaternion.identity);
        hermione.GetComponent<Friend>().target = leader;
        Friends.Add(hermione);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (camSet == 0)
            {
                camSet = 1;
                overviewCam.GetComponent<SmoothFollow>().target = GameObject.FindWithTag("Friend").transform;
            }
            else if (camSet == 1)
            {
                camSet = 0;
                overviewCam.GetComponent<SmoothFollow>().target = GameObject.FindWithTag("Follow").transform;
            }
        }
        CalcCentroids();

        centroidPrefab.transform.position = (centroidPos + new Vector3(0, 1, 0));
        centroidPrefab.transform.forward = CalcFlockDirection(friends);
    }

    /// <summary>
    /// Calculate the average position of the flock.
    /// </summary>
    private void CalcCentroids()
    {
        Vector3 center = new Vector3();

        //Adds the positions
        for (int i = 0; i < Friends.Count; i++)
        {
            center += ((GameObject)Friends[i]).transform.position;
        }
        centroidPos = (center / Friends.Count);
    }

    /// <summary>
    /// Calculate the average direction of the flock.
    /// </summary>
    private Vector3 CalcFlockDirection(ArrayList crew)
    {
        Vector3 totalForward = new Vector3(0, 0, 0);

        for (int i = 0; i < crew.Count; i++)
        {
            totalForward += ((GameObject)crew[i]).transform.forward;
        }

        totalForward.Normalize();

        return totalForward;
    }

    /// <summary>
    /// Wait a few seconds before allowing the user to toggle between camera views.
    /// </summary>
    /// <returns></returns>
    IEnumerator Cinematic()
    {
        // Wait until animation ends before enabling other cameras
        yield return new WaitForSeconds(22);
        cinematicCam.enabled = false;
        overviewCam.enabled = true;
    }
}
