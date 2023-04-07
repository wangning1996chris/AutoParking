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

public class SingleAbsAgentArea : MonoBehaviour
{
    public GameObject[] AreaList;
    public GameObject m_Goal;
    public SingleAbsAgent m_Agent;
    private Rigidbody m_AgentRb;
    private List<(int, int)> PosDeltaList;
    private float[] GoalFlag, AgentFlag;
    private GraphMatrixStructure GrapghInfo;
    private int g_index, a_index;

    void Start()
    {
        m_AgentRb = m_Agent.GetComponent<Rigidbody>();
        PosDeltaList = new List<(int x, int y)> {(45, 25), (-45, 25), (-45, -25), (45, -25)};
        GrapghInfo = new GraphMatrixStructure();
        g_index = -1;
        a_index = -1;
    }

    public void ResetObject()
    {
        float xRange, zRange;
        var enumerable = Enumerable.Range(0, 4).OrderBy(x => Guid.NewGuid()).Take(2);
        var items = enumerable.ToArray();
        // Reset Goal
        g_index = items[0];
        var g_spawnTransform = AreaList[g_index].transform;
        xRange = g_spawnTransform.localScale.x / 3.5f;
        zRange = g_spawnTransform.localScale.z / 3.5f;

        m_Goal.transform.position = new Vector3(Random.Range(-xRange, xRange), 1f, Random.Range(-zRange, zRange))
            + g_spawnTransform.position;
        
        GoalFlag = GrapghInfo.GetEncoder(g_index);

        // Reset Agent
        a_index = items[1];
        var a_spawnTransform = AreaList[a_index].transform;
        xRange = a_spawnTransform.localScale.x / 3.5f;
        zRange = a_spawnTransform.localScale.z / 3.5f;

        m_AgentRb.transform.position = new Vector3(Random.Range(-xRange, xRange), 1.8f, Random.Range(-zRange, zRange))
            + a_spawnTransform.position;
        m_AgentRb.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f));
        m_AgentRb.velocity = Vector3.zero;
        m_AgentRb.angularVelocity = Vector3.zero;

        AgentFlag = GrapghInfo.GetEncoder(a_index);
    }

    void FixedUpdate()
    {
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

    public float[] InitGoalGraph()
    {
        return GoalFlag;
    }

    public float[] InitAgentGraph()
    {
        return AgentFlag;
    }

    public float[] GetAgentGraph()
    {
        Vector2 agent_pos = new Vector2(m_AgentRb.transform.position[0],  m_AgentRb.transform.position[2]);
        for (int i =0 ; i < AreaList.Length; i++)
        {
            Vector2 area_i_pos = new Vector2(AreaList[i].transform.position[0],  AreaList[i].transform.position[2]);
            bool IsInsideBox_i = IsPointInRectangle(agent_pos, 
                                            area_i_pos + GetVector2(PosDeltaList,0), area_i_pos + GetVector2(PosDeltaList,1), 
                                            area_i_pos + GetVector2(PosDeltaList,2), area_i_pos + GetVector2(PosDeltaList,3));
            if (IsInsideBox_i)
            {
                a_index = i;
            }
        }
        return GrapghInfo.GetEncoder(a_index);

        // Vector2 area0_pos = new Vector2(AreaList[0].transform.position[0],  AreaList[0].transform.position[2]);
        // Vector2 area1_pos = new Vector2(AreaList[1].transform.position[0],  AreaList[1].transform.position[2]);
        // bool IsInsideBox_0 = IsPointInRectangle(agent_pos, 
        //                                         area0_pos + GetVector2(PosDeltaList,0), area0_pos + GetVector2(PosDeltaList,1), 
        //                                         area0_pos + GetVector2(PosDeltaList,2), area0_pos + GetVector2(PosDeltaList,3));
        // bool IsInsideBox_1 = IsPointInRectangle(agent_pos,
        //                                         area1_pos + GetVector2(PosDeltaList,0), area1_pos + GetVector2(PosDeltaList,1), 
        //                                         area1_pos + GetVector2(PosDeltaList,2), area1_pos + GetVector2(PosDeltaList,3));
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

    public float CalcuMetric(float[] encoder_1, float[] encoder_2)
    {
        int x = -1, y = -1;
        for (int i = 0; i < encoder_1.Length; i++)
        {
            if (encoder_1[i] == 1)
            {
                x = i;
            }
            if (encoder_2[i] == 1)
            {
                y = i;
            }
        }
        return GrapghInfo.GetDistance(x, y);
    }
}


public class GraphMatrixStructure
{
    private int Graph_Len = 9;
    private float [][] GraphMatrix = {
            new float [9] {0, 1, int.MaxValue, 1, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue},
            new float [9] {1, 0, 1, int.MaxValue, 1, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue},
            new float [9] {int.MaxValue, 1, 0, int.MaxValue, int.MaxValue, 1, int.MaxValue, int.MaxValue, int.MaxValue},
            new float [9] {1, int.MaxValue, int.MaxValue, 0, 1, int.MaxValue, 1, int.MaxValue, int.MaxValue},
            new float [9] {int.MaxValue, 1, int.MaxValue, 1, 0, 1, int.MaxValue, 1, int.MaxValue},
            new float [9] {int.MaxValue, int.MaxValue, 1, int.MaxValue, 1, 0, int.MaxValue, int.MaxValue, 1},
            new float [9] {int.MaxValue, int.MaxValue, int.MaxValue, 1, int.MaxValue, int.MaxValue, 0, 1, int.MaxValue},
            new float [9] {int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, 1, int.MaxValue, 1, 0, 1},
            new float [9] {int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, 1, int.MaxValue, 1, 0}
        };

    // private int Graph_Len = 4;
    // private float [][] GraphMatrix = {
    //         new float [4] {0, 1, 1, int.MaxValue},
    //         new float [4] {1, 0, int.MaxValue, 1},
    //         new float [4] {1, int.MaxValue, 0, 1},
    //         new float [4] {int.MaxValue, 1, 1, 0}
    //     };
    
    public float[] GetEncoder(int index)
    {
        float [] encoder = new float [Graph_Len];
        for (int i = 0; i < Graph_Len; i++)
        {
            if (i == index){
                encoder[i] = 1;
            }
            else{
                encoder[i] = 0;
            }
        }
        return encoder;
    }

    public float GetDistance(int x, int y)
    {
        float[] distList  = Dijkstra(GraphMatrix, x);
        return distList[y];
    }

    public float[] Dijkstra(float[][] mgrap, int v)
    {
        int len = Graph_Len;
        float[] dist = new float[len];
        int[] path = new int[len];
        int[] s = new int[len];   
        float mindis;
        int i, j, u;
        u = 0;

        for (i = 0; i < len; i++)
        {
            dist[i] = mgrap[v][i];       
            s[i] = 0;                        
            if (mgrap[v][i]< int.MaxValue)        
                path[i] = v;
            else
                path[i] = -1;
        }
        s[v] = 1;                  
        path[v] = 0;
        for (i = 0; i < len; i++)                
        {
            mindis = int.MaxValue;                   
            for (j = 0; j < len; j++)        
                if (s[j] == 0 && dist[j] < mindis)
                {
                    u = j;
                    mindis = dist[j];
                }
            s[u] = 1;                       
            for (j = 0; j < len; j++)         
                if (s[j] == 0)
                    if (mgrap[u][j] < int.MaxValue && dist[u] + mgrap[u][j] < dist[j])
                    {
                        dist[j] = dist[u] + mgrap[u][j];
                        path[j] = u;
                    }
        }
        return dist;
    }
}