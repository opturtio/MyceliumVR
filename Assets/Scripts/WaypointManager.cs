using System.Collections.Generic;
using PathCreation;
using PathCreation.Examples;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public int waypointTargetCount;
    public float waypointHeight;
    public GameObject waypointPrefab;
    public GameObject waypointTrailPrefab;

    private List<Agent> agents;


    public void startWaypoints(List<Agent> agents_)
    {
        agents = agents_;

        Agent randomAgent = agents[Random.Range(0, agents.Count)];
        int randomIndex = Random.Range(0, randomAgent.points.Count);
        List<Vector3> points = randomAgent.getPointsToRootFromIndex(randomIndex);

        foreach (Vector3 pos in points)
        {
            GameObject waypoint = Instantiate(waypointPrefab, pos + new Vector3(0f, waypointHeight, 0f), Quaternion.identity);
            waypoint.transform.parent = transform;
        }

        GameObject waypointPath = new GameObject("Trail");
        waypointPath.AddComponent<PathCreator>();

        GameObject trailFollower = Instantiate(waypointTrailPrefab, transform.position, Quaternion.identity);
        trailFollower.AddComponent<PathFollower>();

        BezierPath bezierPath = new BezierPath(points, false, PathSpace.xyz);
        waypointPath.GetComponent<PathCreator>().bezierPath = bezierPath;

        trailFollower.GetComponent<PathFollower>().pathCreator = waypointPath.GetComponent<PathCreator>();
        trailFollower.GetComponent<PathFollower>().speed = 30f;

        waypointPath.transform.parent = transform;
        trailFollower.transform.parent = transform;


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
