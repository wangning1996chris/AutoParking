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


public class RosAgent : Agent
{
    public TrainArea m_MyArea;
    private Vector3 Destination;
    private Rigidbody m_CarRb;
    // connect with ros
    public RosPublisher rosPub;
    public RosSubscriber rosSub;
    private float [] posList;
    private string laserScan;
    private float x, y, z;
    private float t_distance, t_angle;
    private float MaxDistance, MaxAngle;
    private float p_distance, p_angle;
    private float t_y_pos;
    private const int RayNum = 18;
    
    public override void Initialize()
    {
        m_CarRb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // posistion info
        sensor.AddObservation(m_CarRb.transform.position);
        sensor.AddObservation(m_CarRb.transform.rotation);
        sensor.AddObservation(m_CarRb.velocity[0]);
        sensor.AddObservation(Destination);
    }

    public override void OnEpisodeBegin()
    {
        // updateRosSub();
        m_CarRb.transform.position = new Vector3(0, 2, -2);
        m_CarRb.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
        m_MyArea.ResetObject();
        Destination = m_MyArea.GetParkingSpot();
        t_y_pos = 1;
        
        // reset distance
        MaxDistance = (m_CarRb.transform.position - Destination).magnitude;
        t_distance = (m_CarRb.transform.position - Destination).magnitude;
        MaxAngle = Math.Abs(m_CarRb.transform.rotation.eulerAngles[1] - 270);
        t_angle = Math.Abs(m_CarRb.transform.rotation.eulerAngles[1] - 270);

        // reset p_value
        p_angle = t_angle;
        p_distance = t_distance;

        // InvokeRepeating("updateRosSub", 1,1);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Start Train
        MoveAgent(actionBuffers.DiscreteActions);
        IsEndEpisode();
        // Debug.Log(m_CarRb.angularVelocity.x);
        // Debug.Log(m_CarRb.angularVelocity.y);
        // Debug.Log(m_CarRb.angularVelocity.z);
        t_distance = (m_CarRb.transform.position - Destination).magnitude;
        t_angle = Math.Abs(m_CarRb.transform.rotation.eulerAngles[1] - 270);
        
        float distance_reward = -t_distance / MaxDistance;
        float angle_reward = -t_angle / MaxAngle;
        // float distance_reward = (p_distance - t_distance) / MaxDistance * 100;
        // float angle_reward = (p_angle - t_angle) / MaxAngle *50;
        
        AddReward(distance_reward * 2 + angle_reward);
        // Debug.Log(distance_reward + angle_reward);

        p_distance = t_distance;
        p_angle = t_angle;
        // Start ROS

        
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 0.2f;
                break;
            case 2:
                dirToGo = transform.forward * -0.2f;
                break;
            case 3:
                rotateDir = transform.right * 0.5f;
                break;
            case 4:
                rotateDir = transform.right * -0.5f;
                break;
        }
        // transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_CarRb.AddForce(dirToGo, ForceMode.Acceleration);
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


    // private void FixedUpdate()
    // {
    //     rosPub.Publish(m_CarRb.velocity,m_CarRb.angularVelocity);
    //     // updateRosSub();
    // }


    private void updateRosSub()
    {
        // Ros Connect
        posList = rosSub.UpdatePos();
        // laserScan = rosSub.UpdateScan();

        // Update
        x = (float)posList[0];
        y = 2;
        z = (float)posList[2];
        // Debug.Log("x="+x+" z="+z);
        m_CarRb.transform.position = new Vector3(x, y, z);
        m_CarRb.transform.rotation = new Quaternion((float)posList[3],(float)posList[4],(float)posList[5],(float)posList[6]);
    }


    private void IsEndEpisode()
    {
        // Success
        Vector3 CarRota = m_CarRb.transform.rotation.eulerAngles;
        float speed = Math.Abs(m_CarRb.velocity[0]);
        float angle = Math.Abs(CarRota[1] - 270);
        if (t_distance < 2.8 && speed < 1.5 )
        {
            Debug.Log("success");
            AddReward(2000);
            EndEpisode();
        }

        // Failed: out of space
        if (!Physics.Raycast(m_CarRb.transform.position, Vector3.down, 3f) || t_y_pos < 1)
        {
            Debug.Log("out");
            AddReward(-1000);
            EndEpisode();
        }

        // Failed: Collision
        Vector3 RayPos = m_CarRb.transform.position  + new Vector3(0, 1, 0);
        for (int i = 0; i < RayNum-1; i ++)
        {
            float subAngle = -(360 / RayNum) * i;
            Quaternion q = Quaternion.AngleAxis(subAngle, Vector3.up);
            Vector3 forward = q * m_CarRb.transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(RayPos, forward * 3, Color.green);
            if (Physics.Raycast(RayPos, forward, 1.8f))
            {
                Debug.Log("collision");
                AddReward(-1000);
                EndEpisode();
            }
        }

    }
}
