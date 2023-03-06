using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgentsExamples;
using MBaske.Sensors.Grid;

public class MineArea : Area
{
    public GameObject[] AreaList;
    public SimpleObs[] ObsList;
    public GameObject m_GoalRb;

    public void ResetObject()
    {
        // int index = Random.Range(0, AreaList.Length);
        // // int index = 1;
        // var spawnTransform = AreaList[index].transform;
        // var xRange = spawnTransform.localScale.x / 4f;
        // var zRange = spawnTransform.localScale.z / 4f;

        // m_GoalRb.transform.position = new Vector3(Random.Range(-xRange, xRange), 1f, Random.Range(-zRange, zRange))
        //     + spawnTransform.position;

        foreach (var item in ObsList)
        {
            item.ResetPos();
        }
    }
}
