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


public class PidCar : Agent
{
    private CarController m_Car;
    private Rigidbody m_CarRb;
    
    public override void Initialize()
    {
        m_Car = GetComponentInChildren<CarController>();
        m_Car.Initialize();
        m_CarRb = GetComponentInChildren<Rigidbody>();
    }
    
    public override void OnEpisodeBegin()
    {
        m_Car.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        // float steering, float accel, float footbrake, float handbrake
        m_Car.Move(1, 5000, 0, 0); 
    }

}
