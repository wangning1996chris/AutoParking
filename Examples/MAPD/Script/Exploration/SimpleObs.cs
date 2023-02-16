using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgentsExamples;

public class SimpleObs : MonoBehaviour
{
    public GameObject Area;

    private Rigidbody rb;
    private Vector3 dirToGo;
    private float walkSpeed;
    private Vector3 InitPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        InitPos = rb.transform.position;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.transform.position + rb.transform.forward * walkSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("wall"))
        {
            SetDirection();
        }
    }

    public void ResetPos()
    {
        walkSpeed = Random.Range(5f, 15f);
        rb.transform.position = InitPos;
        SetDirection();
    }

    void SetDirection()
    {
        var spawnTransform = Area.transform;
        var xRange = 50f;
        var zRange = 50f;
        var target = new Vector3(Random.Range(-xRange, xRange) + 1f, 1f, Random.Range(-zRange, zRange) - 1f) + spawnTransform.position;
        dirToGo = target - rb.transform.position;
        dirToGo.y = 0;
        rb.rotation = Quaternion.LookRotation(dirToGo);
    }
}
