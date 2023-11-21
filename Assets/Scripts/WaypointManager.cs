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

    private Vector3 targetPoint1;
    private Agent agent1;
    private int agent1Index;
    private Vector3 startingPoint;

    private Vector3 targetPoint2;
    private Agent agent2;

    private GameObject waypointPath1;
    private GameObject waypointPath2;

    private GameObject waypoint1;
    private GameObject waypoint2;

    public void startWaypoints(List<Agent> agents_)
    {
        agents = agents_;

        agent1 = agents[Random.Range(0, agents.Count)];
        startingPoint = agent1.points[0];

        agent1Index = 0;

        targetPoint1 = agent1.branches[agent1Index].Item2.points[0];

        waypoint1 = Instantiate(waypointPrefab, targetPoint1 + new Vector3(0f, waypointHeight, 0f), Quaternion.identity);
        waypoint1.GetComponent<WaypointController>().type = 1;
        waypoint1.GetComponent<WaypointController>().manager = this;

        waypoint1.transform.parent = transform;

        List<Vector3> points1 = new List<Vector3> ();
        for (int i = 0; i <= agent1.branches[0].Item1; i++) {
            points1.Add(agent1.points[i]);
        }

        waypointPath1 = new GameObject("Trail 1");
        waypointPath2 = new GameObject("Trail 2");
        waypointPath1.AddComponent<PathCreator>();
        waypointPath2.AddComponent<PathCreator>();

        GameObject trailFollower1 = Instantiate(waypointTrailPrefab, transform.position, Quaternion.identity);
        GameObject trailFollower2 = Instantiate(waypointTrailPrefab, new Vector3(1000, 1000, 1000), Quaternion.identity);
        trailFollower1.AddComponent<PathFollower>();
        trailFollower2.AddComponent<PathFollower>();

        BezierPath bezierPath = new BezierPath(points1, false, PathSpace.xyz);
        waypointPath1.GetComponent<PathCreator>().bezierPath = bezierPath;

        trailFollower1.GetComponent<PathFollower>().pathCreator = waypointPath1.GetComponent<PathCreator>();
        trailFollower2.GetComponent<PathFollower>().pathCreator = waypointPath2.GetComponent<PathCreator>();
        trailFollower1.GetComponent<PathFollower>().speed = 5f;
        trailFollower2.GetComponent<PathFollower>().speed = 5f;

        waypointPath1.transform.parent = transform;
        waypointPath2.transform.parent = transform;
        trailFollower1.transform.parent = transform;
        trailFollower2.transform.parent = transform;

    }

    private void Trigger()
    {
        Trigger(1);
    }

    public void Trigger(int type)
    {
        Destroy(waypoint1);
        Destroy(waypoint2);

        if (type == 1) {
            // Agent1 stays the same
        }
        else if(type == 2)
        {
            // Agent1 needs to be updated
            agent1 = agent2;
            agent1Index = 0;
        }

        // Index goes up
        agent1Index += 1;

        if (agent1Index > agent1.branches.Count)
        {
            Debug.Log("Reached end");
            return;
        }

        // Agent 2 is updated
        agent2 = agent1.branches[agent1Index - 1].Item2;

        // Starting point moves
        startingPoint = targetPoint1;

        if (agent1Index < agent1.branches.Count)
        {
            // Still branches left for agent1
            targetPoint1 = agent1.branches[agent1Index].Item2.points[0];
        }
        else
        {
            // Put the waypoint at the end of agent1
            targetPoint1 = agent1.points[agent1.points.Count - 1];
        }

        if (agent2.branches.Count > 0)
        {
            // Still branches left for agent1
            targetPoint2 = agent2.points[agent2.branches[0].Item1];
        }
        else
        {
            // Put the waypoint at the end of agent1
            targetPoint2 = agent2.points[agent2.points.Count - 1];
        }


        waypoint1 = Instantiate(waypointPrefab, targetPoint1 + new Vector3(0f, waypointHeight, 0f), Quaternion.identity);
        waypoint1.GetComponent<WaypointController>().type = 1;
        waypoint1.GetComponent<WaypointController>().manager = this;

        waypoint1.transform.parent = transform;

        waypoint2 = Instantiate(waypointPrefab, targetPoint2 + new Vector3(0f, waypointHeight, 0f), Quaternion.identity);
        waypoint2.GetComponent<WaypointController>().type = 2;
        waypoint2.GetComponent<WaypointController>().manager = this;

        waypoint2.transform.parent = transform;

        // Move the trails
        List<Vector3> points1 = new List<Vector3>();
        int endPoint1 = agent1Index < agent1.branches.Count ? agent1.branches[agent1Index].Item1 : agent1.points.Count - 1;
        for (int i = agent1.branches[agent1Index - 1].Item1; i <= endPoint1; i++)
        {
            points1.Add(agent1.points[i]);
        }

        List<Vector3> points2 = new List<Vector3>();
        int endPoint2 = agent2.branches.Count > 0 ? agent2.branches[0].Item1 : agent2.points.Count - 1;
        for (int i = 0; i <= endPoint2; i++)
        {
            points2.Add(agent2.points[i]);
        }

        BezierPath bezierPath1 = new BezierPath(points1, false, PathSpace.xyz);
        waypointPath1.GetComponent<PathCreator>().bezierPath = bezierPath1;

        BezierPath bezierPath2 = new BezierPath(points2, false, PathSpace.xyz);
        waypointPath2.GetComponent<PathCreator>().bezierPath = bezierPath2;

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
