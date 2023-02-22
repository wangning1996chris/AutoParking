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
    private Vector3 GraphFlag;
    private Vector3 GoalFlag;
    private float E_metric, D_metric;
    
    public override void Initialize()
    {
        m_CarRb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(m_CarRb.transform.position);
        sensor.AddObservation(Goal.transform.position);
        sensor.AddObservation(GraphFlag);
        sensor.AddObservation(GoalFlag);
    }

    public override void OnEpisodeBegin()
    {
        Goal.SetActive(true);
        m_MyArea.ResetObject();

        t_dist = (m_CarRb.transform.position - Goal.transform.position).magnitude;
        p_dist = (m_CarRb.transform.position - Goal.transform.position).magnitude;
        GraphFlag = new Vector3(0, 1, 0);
        GoalFlag = m_MyArea.GetGoalGraph();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        t_dist = (m_CarRb.transform.position - Goal.transform.position).magnitude;
        MoveAgent(actionBuffers.DiscreteActions);
        GraphFlag = m_MyArea.GetAgentGraph();
        E_metric = m_MyArea.CalcuMetric(GoalFlag, GraphFlag);
        D_metric = t_dist / p_dist;
        
        AddReward(-1 * D_metric);
        AddReward(-1 * E_metric);

        // Debug.Log(E_metric);
        // Debug.Log(D_metric);
        Debug.Log(GraphFlag);
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
        m_CarRb.AddForce(dirToGo * 2, ForceMode.VelocityChange);
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