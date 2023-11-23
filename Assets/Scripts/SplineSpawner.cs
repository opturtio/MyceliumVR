using System.Collections.Generic;
using UnityEngine;

namespace PathCreation.Examples
{
    // Example of creating a path at runtime from a set of points.

    public class SplineSpawner : MonoBehaviour
    {

        public int initialBranches = 1;
        public int stepSize = 3;
        public int stepCount = 100;
        public float maxTurnPitchAngle = 20f;
        public float maxTurnYawAngle = 20f;
        public float maxBranchPitchAngle= 20f;
        public float maxBranchYawAngle = 20f;
        public float minBranchYawAngle = 15f;
        public float branchChance= 0.05f;
        public int stepsBetweenBranches = 2;

        public List<Agent> agents;
        public List<Agent> startingAgents;
        public List<GameObject> branches;

        public WaypointManager waypointManager;

        [Header("Materials")]
        public Material material;

        private bool needsToUpdate = false;

        void Start()
        {

            agents = new List<Agent>();
            startingAgents = new List<Agent> ();

            for (int i = 0; i < initialBranches; i++) {
                float angle = i * 360 / initialBranches;
                Agent agent = new Agent(
                        null,
                        0,
                        new Vector3(0f, 0f, 0f),
                        Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0f, 1f),
                        stepCount,
                        this
                    );
                agents.Add(agent);
                startingAgents.Add(agent);
            }
            needsToUpdate = true; // Because new branches were added

            while (needsToUpdate)
            {
                needsToUpdate = false;
                for (int i = agents.Count; i > 0; i--)
                {
                    if (agents[i - 1].move())
                    {
                        needsToUpdate = true;
                    }
                }
            }

            foreach (Agent a in agents)
            {
                if(a.points.Count < 2)
                {
                    continue;
                }

                GameObject branch = new GameObject("Branch");
                branch.AddComponent<PathCreator>();

                BezierPath bezierPath = new BezierPath(a.points, false, PathSpace.xyz);

                branch.GetComponent<PathCreator>().bezierPath = bezierPath;
                branch.GetComponent<PathCreator>().EditorData.vertexPathMaxAngleError = 3f;
                branch.GetComponent<PathCreator>().EditorData.vertexPathMinVertexSpacing = 1f;

                branch.AddComponent<RoadMeshCreator>();
                RoadMeshCreator road = branch.GetComponent<RoadMeshCreator>();

                road.autoUpdate = true;
                road.roadWidth = 0.5f;
                road.thickness = 1f;
                road.flattenSurface = true;

                road.roadMaterial = material;
                road.undersideMaterial = material;

                road.pathCreator = branch.GetComponent<PathCreator>();

                road.TriggerUpdate();


                branch.transform.parent = transform;
                branches.Add(branch);
            }
            foreach (GameObject b in branches)
            {
                Destroy(b);
            }
            branches.Clear();
            waypointManager.InitalizeAgents(startingAgents);
        }

    }

    public class Agent
    {
        Vector3 pos;
        Vector3 dir;
        int initialStepsLeft;
        int stepsLeft;

        int initialStepsUntilBranch;
        int stepsUntilBranch;

        public List<(int, Agent)> branches;

        public List<Vector3> points;

        SplineSpawner splineSpawner;

        public Agent(Agent parent_, int connectIndex_, Vector3 pos_, Vector3 dir_, int stepsLeft_, SplineSpawner splineSpawner_)
        {
            pos = pos_;
            dir = dir_;
            stepsLeft = stepsLeft_;
            initialStepsLeft = stepsLeft;
            splineSpawner = splineSpawner_;
            stepsUntilBranch = splineSpawner.stepsBetweenBranches;
            initialStepsUntilBranch = stepsUntilBranch;
            branches = new List<(int, Agent)> ();
            points = new List<Vector3>

            {
                pos
            };
        }

        public bool move()
        {
            if(stepsLeft > 0)
            {
                float pitch = Random.Range(-splineSpawner.maxTurnPitchAngle, splineSpawner.maxTurnPitchAngle);
                float yaw = Random.Range(-splineSpawner.maxTurnYawAngle, splineSpawner.maxTurnYawAngle);
                dir = Quaternion.Euler(pitch, yaw, 0f) * dir;
                pos = pos + dir * splineSpawner.stepSize;
                stepsLeft -= 1;

                points.Add(pos);

                stepsUntilBranch -= 1;

                if (stepsLeft > 3 && stepsUntilBranch <= 0)
                {
                    if (Random.Range(0f, 1f) < splineSpawner.branchChance)
                    {
                        float branchPitch = Random.Range(-splineSpawner.maxBranchPitchAngle, splineSpawner.maxBranchPitchAngle);
                        float branchYaw = Random.Range(splineSpawner.minBranchYawAngle, splineSpawner.maxBranchYawAngle);
                        branchYaw = Random.Range(0f,1f) > 0.5f ? branchYaw : -branchYaw;
                        Agent newAgent = new Agent(
                            this,
                            initialStepsLeft - stepsLeft,
                            pos,
                            Quaternion.Euler(branchPitch, branchYaw, 0f) * dir,
                            stepsLeft,
                            splineSpawner
                            );
                        splineSpawner.agents.Add(newAgent);
                        branches.Add((initialStepsLeft - stepsLeft, newAgent));

                        stepsUntilBranch = initialStepsUntilBranch;
                    }
                }
                return true;
            } else { return false; }
        }
    }
}