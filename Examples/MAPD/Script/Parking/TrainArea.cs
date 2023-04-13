using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgentsExamples;
using MBaske.Sensors.Grid;

// Parking
public class TrainArea : Area
{
    public GameObject[] TruckList;
    public GameObject m_GoalRb;
    private Vector3 ParkingSpot;

    public void ResetObject()
    {
        for (int i = 0; i < TruckList.Length; i = i + 1)
        {
            TruckList[i].SetActive(true);
        }

        // var spawnTransform = AreaList[index].transform;
        // var xRange = spawnTransform.localScale.x / 3.5f;
        // var zRange = spawnTransform.localScale.z / 3.5f;

        // m_GoalRb.transform.position = new Vector3(Random.Range(-xRange, xRange), 1.5f, Random.Range(-zRange, zRange))
        //     + spawnTransform.position;
    }

    public Vector3 GetParkingSpot()
    {
        // int index = Random.Range(0, TruckList.Length);
        // ParkingSpot = TruckList[index].transform.position + new Vector3(Random.Range(-20, 0), -0.2f, 0);
        // m_GoalRb.transform.position = TruckList[index].transform.position + new Vector3(Random.Range(-20, 0), -0.2f, 0);
        int index = 3;
        ParkingSpot = TruckList[index].transform.position;
        m_GoalRb.transform.position = TruckList[index].transform.position + new Vector3(0, -0.2f, 0);
        TruckList[index].SetActive(false);
        return ParkingSpot;
    }
}
