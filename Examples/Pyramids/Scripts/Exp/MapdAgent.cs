using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MapdAgent : Agent
{
    public GameObject area;
    public GameObject switchButton;
    public Material normalMaterial;
    public Material goodMaterial;
    private bool carryStone;
    private int layerNum;
    MapdSwitch m_MySwitch;
    MapdArea m_MyArea;
    Rigidbody m_AgentRb;
    GridSensorComponent m_GridSensor;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<MapdArea>();
        m_MySwitch = switchButton.GetComponent<MapdSwitch>();
        m_GridSensor = GetComponent<GridSensorComponent>();
        layerNum = LayerMask.NameToLayer("UI");
        m_GridSensor.ColliderMask = ~(1 << 1);
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));
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
        m_AgentRb.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
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

    public bool getCarryFlag()
    {
        return carryStone;
    }

    public void ResetInfo()
    {
        carryStone = false;
        gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
        m_GridSensor.ColliderMask = ~(1 << 1);
    }

    void OnCollisionEnter(Collision collision)
    {
        string m_switchButton = "switch_" + m_MySwitch.indexButton;
        if (collision.gameObject.CompareTag("wall") || collision.gameObject.CompareTag("agent"))
        {
            Debug.Log("coll");
            m_MyArea.ScoredReward(-0.5f);
        }

        if (collision.gameObject.CompareTag("stone") && !carryStone)
        {
            carryStone = true;
            gameObject.GetComponentInChildren<Renderer>().material = goodMaterial;
            m_GridSensor.ColliderMask = ~(1 << layerNum);
            m_MySwitch.EnableSwitch();
            m_MyArea.ScoredReward(50);
            collision.gameObject.GetComponent<MapdStone>().ResetStone();
        }

        if (collision.gameObject.CompareTag(m_switchButton))
        {
            ResetInfo();
            m_MySwitch.ResetSwitch();
            m_MyArea.ScoredReward(50);
        }
    }

}
