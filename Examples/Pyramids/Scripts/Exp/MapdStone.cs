using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgentsExamples;

public class MapdStone : MonoBehaviour
{
    public GameObject area;
    MapdArea m_MyArea;
    Rigidbody m_Rb;

    void Start()
    {
        m_MyArea = area.GetComponent<MapdArea>();
        m_Rb = GetComponent<Rigidbody>();
        this.gameObject.SetActive(true);
    } 

    public void ResetStone()
    {
        this.gameObject.SetActive(true);
        var enumerable = Enumerable.Range(0, 8).OrderBy(x => Guid.NewGuid()).Take(8);
        var itemsIndex = enumerable.ToArray();
        var pos = m_MyArea.GetRandomSpawnPos(itemsIndex[0]);
        var rot = m_MyArea.GetRandomRot();

        gameObject.transform.SetPositionAndRotation(pos, rot);
        m_Rb.velocity = Vector3.zero;
        m_Rb.angularVelocity = Vector3.zero;
    }
}
