using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgentsExamples;
using MBaske.Sensors.Grid;
using System;
using Random = UnityEngine.Random;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityStandardAssets.Vehicles.Car;

public class SingleCarArea : MonoBehaviour
{
    public GameObject[] AreaList;
    public GameObject m_Goal;
    public SingleCarAgent m_Agent;
    private Rigidbody m_AgentRb;
    private List<(int, int)> PosDeltaList;
    private Vector3 GoalFlag;

    void Start()
    {
        m_AgentRb = m_Agent.GetComponentInChildren<Rigidbody>();
        PosDeltaList = new List<(int x, int y)> {(45, 25), (-45, 25), (-45, -25), (45, -25)};
        ResetObject();
    }

    public void ResetObject()
    {
        // Reset Goal
        int index = Random.Range(0, 2);
        var spawnTransform = AreaList[index].transform;
        float xRange = spawnTransform.localScale.x / 3.5f;
        float zRange = spawnTransform.localScale.z / 3.5f;

        m_Goal.transform.position = new Vector3(Random.Range(-xRange, xRange), 1f, Random.Range(-zRange, zRange))
            + spawnTransform.position;
        
        if (index == 0)
        {
            GoalFlag = new Vector3(1, 0, 0);
        }
        else
        {
            GoalFlag = new Vector3(0, 0, 1);
        }

        // Reset Agent
        m_AgentRb.transform.position = new Vector3(0, 1f, 0);
        m_AgentRb.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f));
        m_AgentRb.velocity = Vector3.zero;
        m_AgentRb.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        m_Agent.IsEndEpisode();
    }

    public void SetGoalPos(GameObject goal)
    {
        int index = Random.Range(0, AreaList.Length);
        var spawnTransform = AreaList[index].transform;
        var xRange = spawnTransform.localScale.x / 3.5f;
        var zRange = spawnTransform.localScale.z / 3.5f;

        goal.transform.position = new Vector3(Random.Range(-xRange, xRange), 1f, Random.Range(-zRange, zRange))
            + spawnTransform.position;
    }

    public Vector3 GetGoalGraph()
    {
        return GoalFlag;
    }

    public Vector3 GetAgentGraph()
    {
        Vector2 agent_pos = new Vector2(m_AgentRb.transform.position[0],  m_AgentRb.transform.position[2]);
        Vector2 area0_pos = new Vector2(AreaList[0].transform.position[0],  AreaList[0].transform.position[2]);
        Vector2 area1_pos = new Vector2(AreaList[1].transform.position[0],  AreaList[1].transform.position[2]);
        bool IsInsideBox_0 = IsPointInRectangle(agent_pos, 
                                                area0_pos + GetVector2(PosDeltaList,0), area0_pos + GetVector2(PosDeltaList,1), 
                                                area0_pos + GetVector2(PosDeltaList,2), area0_pos + GetVector2(PosDeltaList,3));
        bool IsInsideBox_1 = IsPointInRectangle(agent_pos,
                                                area1_pos + GetVector2(PosDeltaList,0), area1_pos + GetVector2(PosDeltaList,1), 
                                                area1_pos + GetVector2(PosDeltaList,2), area1_pos + GetVector2(PosDeltaList,3));
        if (IsInsideBox_0)
        {
            return new Vector3(1, 0, 0);
        }
        else if (IsInsideBox_1)
        {
            return new Vector3(0, 0, 1);
        }
        else
        {
            return new Vector3(0, 1, 0);
        }
    }

    public float Cross(Vector2 a, Vector2 b)
    {
        return a[0] * b[1] - b[0] * a[1];
    }

    public Vector2 GetVector2(List<(int x, int y)> pos, int index)
    {
        var point = pos[index];
        return new Vector2(point.x, point.y);
    }

    public bool IsPointInRectangle(Vector2 P, Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        Vector2 AB = A - B;
        Vector2 AP = A - P;
        Vector2 CD = C - D;
        Vector2 CP = C - P;

        Vector2 DA = D - A;
        Vector2 DP = D - P;
        Vector2 BC = B - C;
        Vector2 BP = B - P;

        bool isBetweenAB_CD = (Cross(AB, AP) * Cross(CD, CP)) > 0;
        bool isBetweenDA_BC = (Cross(DA, DP) * Cross(BC, BP)) > 0;
        return isBetweenAB_CD && isBetweenDA_BC;
    }

    public float CalcuMetric(Vector3 x, Vector3 y)
    {
        int index_x = 0, index_y = 0;
        for (int i = 0; i < 3; i++)
        {
            if (x[i] == 1)
            {
                index_x = i;
            }
            if (y[i] == 1)
            {
                index_y = i;
            }
        }
        return Math.Abs(index_x - index_y);
    }

    public float CalcuGraphFlag(Vector3 x, Vector3 y)
    {
        if (x == y)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
