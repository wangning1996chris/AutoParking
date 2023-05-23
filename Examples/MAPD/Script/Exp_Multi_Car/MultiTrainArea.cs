using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgentsExamples;
using UnityEngine;
using Unity.MLAgentsExamples;
using MBaske.Sensors.Grid;
using System.Linq;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

// Parking
public class MultiTrainArea : Area
{
    public GameObject[] TruckList;
    public GameObject[] GoalList;

    [System.Serializable]
    public class PlayerInfo
    {
        public MultiCarAgent Agent;
        [HideInInspector]
        public Rigidbody Rb;
    }
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();
    private SimpleMultiAgentGroup m_AgentGroup;
    private int parkCarNum;

    void Start()
    {
        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var item in AgentsList)
        {
            item.Rb = item.Agent.GetComponentInChildren<Rigidbody>();
            m_AgentGroup.RegisterAgent(item.Agent);
        }
        // ResetObject();
    }

    public void ResetObject()
    {
        parkCarNum = 0; 
        int len_trucks = TruckList.Length;
        int len_agents = GoalList.Length;
        var enumerable = Enumerable.Range(1, len_trucks-2).OrderBy(x => Guid.NewGuid()).Take(len_agents);
        var items = enumerable.ToArray();

        // reset trcuk
        for (int i = 0; i < len_trucks; i = i + 1)
        {
            TruckList[i].SetActive(true);
        }
        // reset goal
        for (int i = 0; i < items.Length; i = i + 1)
        {
            int truckIndex = items[i];
            int goalIndex = i;
            GoalList[goalIndex].transform.position = TruckList[truckIndex].transform.position + new Vector3(0, -0.2f, 0);
            TruckList[truckIndex].SetActive(false);
        }
        // reset agent
        // TODO position is not right
        int ID = Random.Range(0, 5);
        var PosList = Enumerable.Range(0, len_agents).Take(len_agents).ToArray();
        float[,] z_pos = new float [6,3] {{-14, 0, 14}, {0, -14, 14}, {-14, 14, 0}, {14, 0, -14}, {0, 14, -14}, {14, -14, 0}};
        foreach (var item in AgentsList)
        {
            item.Agent.gameObject.SetActive(true);
            int index = AgentsList.IndexOf(item);
            var xRange = 5;
            var zRange = 5;
            item.Rb.transform.position = new Vector3(-6f, 1f, z_pos[ID,index]);
            item.Rb.transform.rotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(-30, 30), 0f));
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        foreach (var item in AgentsList)
        {
            bool endFlag = item.Agent.IsEndEpisode();
            if (parkCarNum == GoalList.Length || endFlag)
            {
                m_AgentGroup.EndGroupEpisode();
            }
        }
    }

    public void AddGroupRewardAsValue(float reward)
    {
        m_AgentGroup.AddGroupReward(reward);
    }

    public void AgentIsSucceed(int index)
    {
        AgentsList[index].Agent.gameObject.SetActive(false);
        parkCarNum += 1;
    }

}
