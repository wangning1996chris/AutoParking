using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgentsExamples;

public class MapdArea : Area
{
    /// <summary>
    /// The class of Player and Stone
    /// </summary>
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

    [System.Serializable]
    public class StoneInfo
    {
        public Transform T;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }   

    [Header("Max Environment Steps")] public int MaxEnvironmentSteps = 5000;
    //List of Agents On Platform
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();
    //List of Blocks On Platform
    public List<StoneInfo> StonesList = new List<StoneInfo>();
    // List of Switch On Platform
    public List<MapdSwitch> SwitchsList = new List<MapdSwitch>();
    //Config of Agent and Stone
    public bool UseRandomAgentRotation = false;
    public bool UseRandomAgentPosition = true;
    public bool UseRandomStoneRotation = false;
    public bool UseRandomStonePosition = true;

    // Other Var
    public GameObject[] spawnAreas;
    private SimpleMultiAgentGroup m_AgentGroup;
    private int m_ResetTimer;

    /// <summary>
    /// API for MAPD
    /// </summary>
    void Start()
    {
        // Initialize TeamManager
        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            m_AgentGroup.RegisterAgent(item.Agent);
        }
        
        // Initialize Multi Stone
        foreach (var item in StonesList)
        {
            item.StartingPos = item.T.transform.position;
            item.StartingRot = item.T.transform.rotation;
            item.Rb = item.T.GetComponent<Rigidbody>();
        }

        // reset
        ResetScene();
    }


    public Vector3 GetRandomSpawnPos(int spawnAreaIndex)
    {   
        var spawnTransform = spawnAreas[spawnAreaIndex].transform;
        var xRange = spawnTransform.localScale.x / 3.1f;
        var zRange = spawnTransform.localScale.z / 3.1f;

        Vector3 randomSpawnPos = new Vector3(Random.Range(-xRange, xRange), 0.5f, Random.Range(-zRange, zRange))
            + spawnTransform.position;

        return randomSpawnPos;
    }


    public Quaternion GetRandomRot()
    {
        return Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
    }


    public void ScoredReward(float score)
    {
        // change reward
        m_AgentGroup.AddGroupReward(score);
    }


    public void ResetScene()
    {           
        m_ResetTimer = 0;
        int spawnNum = spawnAreas.Length;
        int takeNum = AgentsList.Count + StonesList.Count;
        var enumerable = Enumerable.Range(0, spawnNum).OrderBy(x => Guid.NewGuid()).Take(takeNum);
        var itemsIndex = enumerable.ToArray();
        //Reset Agents
        foreach (var item in AgentsList)
        {
            int indexItem = AgentsList.IndexOf(item);
            var pos = UseRandomAgentPosition ? GetRandomSpawnPos(itemsIndex[(indexItem + 0) % takeNum]) : item.StartingPos;
            var rot = UseRandomAgentRotation ? GetRandomRot() : item.StartingRot;

            item.Agent.transform.SetPositionAndRotation(pos, rot);
            item.Agent.ResetInfo();
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }

        //Reset Blocks
        foreach (var item in StonesList)
        {
            int indexItem = StonesList.IndexOf(item);
            var pos = UseRandomStonePosition ? GetRandomSpawnPos(itemsIndex[(indexItem + AgentsList.Count) % takeNum]) : item.StartingPos;
            var rot = UseRandomStoneRotation ? GetRandomRot() : item.StartingRot;

            item.T.transform.SetPositionAndRotation(pos, rot);
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
            item.T.gameObject.SetActive(true);
        }

        // Reset Switch
        foreach (var item in SwitchsList)
        {
            item.ResetSwitch();
        }
    }

    void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_AgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }

        //Hurry Up Penalty
        m_AgentGroup.AddGroupReward(-0.1f);
    }
}
