using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgentsExamples;
using MBaske.Sensors.Grid;

// Parking
public class PitTrainArea : Area
{
    // public GameObject[] TruckList;
    public GameObject m_GoalRb;
    private Vector3 ParkingSpot;

    public Vector3 GetParkingSpot()
    {
        // ParkingSpot = m_GoalRb.transform.position+ new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        ParkingSpot = m_GoalRb.transform.position;
        return ParkingSpot;
    }
}
