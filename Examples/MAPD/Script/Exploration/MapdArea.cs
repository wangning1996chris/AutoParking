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

public class MapdArea : MonoBehaviour
{
    public GameObject[] AreaList;
    public GameObject[] m_GoalList;

    [System.Serializable]
    public class PlayerInfo
    {
        public MapdAgent Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();

    private SimpleMultiAgentGroup m_AgentGroup;
    private const float MaxStep = 6000f;

    void Start()
    {
        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponentInChildren<Rigidbody>();
            m_AgentGroup.RegisterAgent(item.Agent);
        }
        ResetObject();
    }

    public void ResetObject()
    {
        var enumerable = Enumerable.Range(0, 9).OrderBy(x => Guid.NewGuid()).Take(m_GoalList.Length * 2);
        var items = enumerable.ToArray();

        // Reset Goal and Agent
        for (int i = 0; i < m_GoalList.Length; i++)
        {
            int index = items[i];
            var spawnTransform = AreaList[index].transform;
            var xRange = spawnTransform.localScale.x / 3.5f;
            var zRange = spawnTransform.localScale.z / 3.5f;

            m_GoalList[i].transform.position = new Vector3(Random.Range(-xRange, xRange), 1f, Random.Range(-zRange, zRange))
                + spawnTransform.position;
        }

        // Reset Agent
        foreach (var item in AgentsList)
        {
            int index = items[AgentsList.IndexOf(item) + m_GoalList.Length];
            var spawnTransform = AreaList[index].transform;
            var xRange = spawnTransform.localScale.x / 3.5f;
            var zRange = spawnTransform.localScale.z / 3.5f;
            
            item.Rb.transform.position = new Vector3(Random.Range(-xRange, xRange), 1f, Random.Range(-zRange, zRange))
                + spawnTransform.position;
            item.Rb.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f));
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

    public void SetGoalPos(GameObject goal)
    {
        int index = Random.Range(0, AreaList.Length);
        var spawnTransform = AreaList[index].transform;
        var xRange = spawnTransform.localScale.x / 3.5f;
        var zRange = spawnTransform.localScale.z / 3.5f;

        goal.transform.position = new Vector3(Random.Range(-xRange, xRange), 1f, Random.Range(-zRange, zRange))
            + spawnTransform.position;
    }
}
