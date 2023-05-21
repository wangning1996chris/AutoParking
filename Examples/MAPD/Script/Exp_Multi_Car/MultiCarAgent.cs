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

// Parking
public class MultiCarAgent : Agent
{
    public MultiTrainArea m_MyArea; 
    public GameObject Goal;
    public int index;
    
    private Vector3 Destination;
    private CarController m_Car;
    private Rigidbody m_CarRb;

    private float t_distance, t_angle;
    private float MaxDistance, MaxAngle;
    private const int RayNum = 36;



    public override void Initialize()
    {
        m_Car = GetComponentInChildren<CarController>();
        m_Car.Initialize();
        m_CarRb = GetComponentInChildren<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        m_MyArea.ResetObject();
        Destination = Goal.transform.position;
        // reset var
        MaxDistance = (m_Car.transform.position - Destination).magnitude;
        t_distance = (m_Car.transform.position - Destination).magnitude;
        MaxAngle = Math.Abs(m_Car.transform.rotation.eulerAngles[1] - 270);
        t_angle = Math.Abs(m_Car.transform.rotation.eulerAngles[1] - 270);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(m_Car.NormSteer);
        sensor.AddObservation(Normalization.Sigmoid(m_Car.LocalSpin));
        sensor.AddObservation(Normalization.Sigmoid(m_Car.LocalVelocity));
        sensor.AddObservation(m_Car.transform.position);
        sensor.AddObservation(Destination);
    }
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actions = actionBuffers.ContinuousActions;
        m_Car.Move(actions[0], actions[1]*200, actions[1], actions[2]);

        t_distance = (m_Car.transform.position - Destination).magnitude;
        t_angle = Math.Abs(m_Car.transform.rotation.eulerAngles[1] - 270);
        
        float distance_reward = -t_distance / MaxDistance;
        float angle_reward = -t_angle / MaxAngle;
        
        m_MyArea.AddGroupRewardAsValue(distance_reward * 2 + angle_reward);
        // Debug.Log(distance_reward + angle_reward);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
        actions[1] = Mathf.RoundToInt(Input.GetAxis("Vertical"));
        actions[2] = Mathf.RoundToInt(Input.GetAxis("Jump"));
    }

    public bool IsEndEpisode()
    {
        // Success
        bool flag = false;
        Vector3 CarRota = m_Car.transform.rotation.eulerAngles;
        float speed = Math.Abs(m_Car.ForwardSpeed);
        float angle = Math.Abs(CarRota[1] - 270);
        if (t_distance < 1.8 && speed < 0.5 && angle < 8)
        {
            Debug.Log("success");
            m_MyArea.AgentIsSucceed(index);
        }

        // Failed: out of space
        if (!Physics.Raycast(m_Car.transform.position, Vector3.down, 3f))
        {
            Debug.Log("out");
            m_MyArea.AddGroupRewardAsValue(-5000);
            flag = true;
        }

        // Failed: Collision
        Vector3 RayPos = m_Car.transform.position  + new Vector3(0, 1, 0);
        for (int i = 0; i < RayNum-1; i ++)
        {
            float subAngle = -(360 / RayNum) * i;
            Quaternion q = Quaternion.AngleAxis(subAngle, Vector3.up);
            Vector3 forward = q * m_Car.transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(RayPos, forward * 3, Color.green);
            if (Physics.Raycast(RayPos, forward, 2.2f))
            {
                Debug.Log("collision");
                m_MyArea.AddGroupRewardAsValue(-500);
                break;
            }
        }
        return flag;
    }
}
