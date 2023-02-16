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

    void Start()
    {
        m_AgentRb = m_Agent.GetComponentInChildren<Rigidbody>();
        ResetObject();
    }

    public void ResetObject()
    {
        var enumerable = Enumerable.Range(0, 2).OrderBy(x => Guid.NewGuid()).Take(2);
        var items = enumerable.ToArray();
        float xRange, zRange;

        // Reset Goal
        var spawnTransform = AreaList[items[0]].transform;
        xRange = spawnTransform.localScale.x / 3.5f;
        zRange = spawnTransform.localScale.z / 3.5f;

        m_Goal.transform.position = new Vector3(Random.Range(-xRange, xRange), 1f, Random.Range(-zRange, zRange))
            + spawnTransform.position;

        // Reset Agent
        spawnTransform = AreaList[items[1]].transform;
        xRange = spawnTransform.localScale.x / 3.5f;
        zRange = spawnTransform.localScale.z / 3.5f;

        m_AgentRb.transform.position = new Vector3(Random.Range(-xRange, xRange), 1f, Random.Range(-zRange, zRange))
            + spawnTransform.position;
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
}
