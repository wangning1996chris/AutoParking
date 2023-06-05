using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PyramidGlobalSensor : Agent
{
    public GameObject area;
    PyramidArea m_MyArea;

    // Start is called before the first frame update
    public override void Initialize()
    {
        m_MyArea = area.GetComponent<PyramidArea>();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;
    }

    public void ResetSensor()
    {
        this.gameObject.SetActive(true);
    }
    
}
