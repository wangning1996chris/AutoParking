using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityStandardAssets.Vehicles.Car;
using MBaske.MLUtil;
// using MBaske.Sensors.Grid;


public class SingleAbsAgent : Agent
{
    public GameObject Goal;
    public SingleAbsAgentArea m_MyArea; 
    private Rigidbody m_CarRb;
    private float t_dist, p_dist;
    private const int RayNum=18;
    private float[] AgentFlag;
    private float[] GoalFlag;
    private float E_metric, D_metric;
    
    public override void Initialize()
    {
        m_CarRb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // posistion info
        sensor.AddObservation(m_CarRb.transform.position);
        sensor.AddObservation(Goal.transform.position);
        // graph encoder 
        // sensor.AddObservation(AgentFlag);
        // sensor.AddObservation(GoalFlag);
    }

    public override void OnEpisodeBegin()
    {
        Goal.SetActive(true);
        m_MyArea.ResetObject();

        t_dist = (m_CarRb.transform.position - Goal.transform.position).magnitude;
        p_dist = (m_CarRb.transform.position - Goal.transform.position).magnitude;
        AgentFlag = m_MyArea.InitAgentGraph();
        GoalFlag = m_MyArea.InitGoalGraph();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        t_dist = (m_CarRb.transform.position - Goal.transform.position).magnitude;
        MoveAgent(actionBuffers.DiscreteActions);
        AgentFlag = m_MyArea.GetAgentGraph();
        E_metric = m_MyArea.CalcuMetric(AgentFlag, GoalFlag);
        D_metric = t_dist / p_dist;
        
        AddReward(-1 * D_metric);
        AddReward(-1 * E_metric);

        // string DebugInfo = string.Join(",", AgentFlag);
        // Debug.Log(E_metric);
        // Debug.Log(D_metric);
        // Debug.Log(DebugInfo);

        if (!Physics.Raycast(m_CarRb.transform.position, Vector3.down, 3f))
        {
            Debug.Log("out");
            AddReward(-5000);
            EndEpisode();
        }
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_CarRb.AddForce(dirToGo * 4, ForceMode.VelocityChange);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal"))
        {
            Debug.Log("success");
            AddReward(5000);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("wall"))
        {
            Debug.Log("collision");
            AddReward(-10);
        }
    }
}
