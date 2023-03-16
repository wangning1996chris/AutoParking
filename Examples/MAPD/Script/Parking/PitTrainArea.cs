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
    public GameObject[] WallList;
    public GameObject[] AreaList;
    public void ResetObject()
    {
        for (int i = 0; i < WallList.Length; i = i + 1)
        {
            WallList[i].SetActive(true);
        }
        int index = Random.Range(0, AreaList.Length);
        WallList[index].SetActive(false);
        var spawnTransform = AreaList[index].transform;
        var xRange = spawnTransform.localScale.x / 3.5f;
        var zRange = spawnTransform.localScale.z / 3.5f;

        m_GoalRb.transform.position = new Vector3(Random.Range(-xRange, xRange), 1, Random.Range(-zRange, zRange))
            + spawnTransform.position;
    }
    public Vector3 GetParkingSpot()
    {
        // ParkingSpot = m_GoalRb.transform.position+ new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        ResetObject();
        ParkingSpot = m_GoalRb.transform.position;
        return ParkingSpot;
    }
}
