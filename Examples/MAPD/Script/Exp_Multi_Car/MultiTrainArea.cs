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

    void Start()
    {
        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var item in AgentsList)
        {
            item.Rb = item.Agent.GetComponentInChildren<Rigidbody>();
            m_AgentGroup.RegisterAgent(item.Agent);
        }
        ResetObject();
    }

    public void ResetObject()
    {
        int len_trucks = TruckList.Length;
        int len_agents = GoalList.Length;
        var enumerable = Enumerable.Range(0, len_trucks).OrderBy(x => Guid.NewGuid()).Take(len_agents);
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
        foreach (var item in AgentsList)
        {
            item.Agent.gameObject.SetActive(true);
            int delta = AgentsList.IndexOf(item) * 10;
            var xRange = 5;
            var zRange = 5;
            item.Rb.transform.rotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(-30, 30), 0f));
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        foreach (var item in AgentsList)
        {
            item.Agent.IsEndEpisode();
        }
        // m_AgentGroup.AddGroupReward(-100 / MaxStep);
    }

    public void AddGroupRewardAsValue(float reward)
    {
        m_AgentGroup.AddGroupReward(reward);
    }

    public void AgentIsSucceed(int index)
    {
        AgentsList[index].Agent.gameObject.SetActive(false);
    }

}
