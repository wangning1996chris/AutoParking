using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class PyramidDynObs : MonoBehaviour
{
    private Rigidbody m_rb;
    private Transform m_tf;
    public float walkSpeed = 8;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        var m_rotDir = Random.Range(-1, 2);
        m_tf.Rotate(0, m_rotDir * Time.deltaTime * 100, 0, Space.Self);
        m_rb.velocity = m_tf.forward * walkSpeed;
        // m_rb.AddForce(dirToGo * walkSpeed, ForceMode.VelocityChange);
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("stone_1") || 
            col.gameObject.CompareTag("stone_2") || 
            col.gameObject.CompareTag("stone_3") || 
            col.gameObject.CompareTag("wall") || 
            col.gameObject.CompareTag("agent") ||
            col.gameObject.CompareTag("switchOff_1") || 
            col.gameObject.CompareTag("switchOff_2") ||
            col.gameObject.CompareTag("switchOff_3")
            )
        {
            m_tf.Rotate(0, -180, 0, Space.Self);
        }
    }
}
