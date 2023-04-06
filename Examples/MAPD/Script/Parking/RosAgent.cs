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
    private Rigidbody m_CarRb;
    // connect with ros
    public RosPublisher rosPub;
    public RosSubscriber rosSub;
    private double[] posList;
    private string laserScan;
    private float x, y, z;
    
    public override void Initialize()
    {
        m_CarRb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // posistion info
        sensor.AddObservation(m_CarRb.transform.position);
    }

    public override void OnEpisodeBegin()
    {
        m_CarRb.transform.position = new Vector3(10, 2, 0);
        m_CarRb.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Start Train
        // MoveAgent(actionBuffers.DiscreteActions);

        // Start ROS
        rosPub.Update();
        Invoke("updateRosSub", 0.02f);
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
        m_CarRb.AddForce(dirToGo, ForceMode.VelocityChange);
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


    private void updateRosSub()
    {
        // Ros Connect
        posList = rosSub.UpdatePos();
        laserScan = rosSub.UpdateScan();

        // Update
        x = (float)posList[0] * 4;
        y = 2;
        z = (float)posList[1] * 4;
        m_CarRb.transform.position = new Vector3(x, y, z);
    }
}
