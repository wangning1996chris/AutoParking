using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityStandardAssets.Vehicles.Car;
using MBaske.MLUtil;
using MBaske.Sensors.Grid;


public class SingleCarAgent : Agent
{
    public GameObject Goal;
    public SingleCarArea m_MyArea; 
    private CarController m_Car;
    private Rigidbody m_CarRb;
    private float t_dist, p_dist;
    private const int RayNum=18;
    
    public override void Initialize()
    {
        m_Car = GetComponentInChildren<CarController>();
        m_Car.Initialize();
        m_CarRb = GetComponentInChildren<Rigidbody>();
        t_dist = (m_Car.transform.position - Goal.transform.position).magnitude;
        p_dist = (m_Car.transform.position - Goal.transform.position).magnitude;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(m_Car.NormSteer);
        sensor.AddObservation(Normalization.Sigmoid(m_Car.LocalSpin));
        sensor.AddObservation(Normalization.Sigmoid(m_Car.LocalVelocity));
        sensor.AddObservation(m_Car.transform.position);
        sensor.AddObservation(Goal.transform.position);
    }

    public override void OnEpisodeBegin()
    {
        Goal.SetActive(true);
        m_Car.gameObject.SetActive(true);
        m_MyArea.ResetObject();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        t_dist = (m_Car.transform.position - Goal.transform.position).magnitude;
        var actions = actionBuffers.ContinuousActions;
        m_Car.Move(actions[0], actions[1]*500, actions[1], actions[2]); 
        
        AddReward((p_dist - t_dist) * 10);
        Debug.Log((p_dist - t_dist) * 10);
        p_dist = t_dist;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
        actions[1] = Mathf.RoundToInt(Input.GetAxis("Vertical"));
        actions[2] = Mathf.RoundToInt(Input.GetAxis("Jump"));
    }

    public void IsEndEpisode()
    {
        float t_distance = (m_Car.transform.position - Goal.transform.position).magnitude;
        float t_y_pos = m_Car.transform.position[1];

        // Success
        if (t_distance < 5)
        {
            Debug.Log("success");
            AddReward(1000);
            EndEpisode();
        }

        // // Failed: out of space
        // if (!Physics.Raycast(m_Car.transform.position, Vector3.down, 3f)  || t_y_pos < 1)
        // {
        //     Debug.Log("out");
        //     m_MyArea.ResetObject();
        // }

        // Failed: Collision
        Vector3 RayPos = m_Car.transform.position  + new Vector3(0, 1, 0);
        for (int i = 0; i < RayNum-1; i ++)
        {
            float subAngle = -(360 / RayNum) * i;
            Quaternion q = Quaternion.AngleAxis(subAngle, Vector3.up);
            Vector3 forward = q * m_Car.transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(RayPos, forward * 3f, Color.green);
            if (Physics.Raycast(RayPos, forward, 3f))
            {
                Debug.Log("collision");
                AddReward(-1000);
                EndEpisode();
            }
        }
    }
}
