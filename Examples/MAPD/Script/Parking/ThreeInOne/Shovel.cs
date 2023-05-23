using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityStandardAssets.Vehicles.Car;
using MBaske.MLUtil;
using MBaske.Sensors.Grid;

// Parking
public class Shovel : Agent
{
    // public TrainArea m_MyArea; 
    // public GridBuffer m_SensorBuffer;

    private Vector3 Destination;
    private CarController m_Car;
    private Rigidbody m_CarRb;

    private const int Goal = 0;
    private const int Agent = 1;
    private const int Size = 60;
    private const int aroundScale = 4;
    private float t_distance, t_angle;
    private float MaxDistance, MaxAngle;
    private float p_distance, p_angle;
    private float t_y_pos;
    private const int RayNum = 18;

      //draw line 
    public GameObject lineprefab;
    public GameObject currentline;
    public GameObject emptyPrefab;
    public GameObject lineObject;
    public LineRenderer line;
    private Vector3[] path;
    private List<Vector3> pos = new List<Vector3>();
    private List<Vector3[]> paths = new List<Vector3[]>();
    private float timer;

    public override void Initialize()
    {
        m_Car = GetComponentInChildren<CarController>();
        m_Car.Initialize();
        m_CarRb = GetComponentInChildren<Rigidbody>();

        // m_SensorBuffer = new ColorGridBuffer(2, new Vector2Int(Size, Size));
        // var sensorComp = GetComponent<MBaske.Sensors.Grid.GridSensorComponent>();
        // sensorComp.GridBuffer = m_SensorBuffer;
        // sensorComp.ChannelLabels = new List<ChannelLabel>()
        // {
        //     new ChannelLabel("goal", new Color32(0, 128, 255, 255)),
        //     new ChannelLabel("agent", new Color32(64, 255, 64, 255)),
        // };
    }

    public override void OnEpisodeBegin()
    {
        Destination = new Vector3(116, 1, -259);
        // reset agent
        m_Car.transform.position = new Vector3(93, 1, -250)+new Vector3(UnityEngine.Random.Range(-7, 11), 0, UnityEngine.Random.Range(-7, 7));
        m_Car.transform.rotation = Quaternion.Euler(new Vector3(0f, 209f, 0f)+new Vector3(0f, UnityEngine.Random.Range(-30, 30), 0f));
        // var xRange = 5;
        // var zRange = 10;
        // m_Car.transform.position = new Vector3(UnityEngine.Random.Range(-xRange, 0), 1, UnityEngine.Random.Range(-zRange, zRange));
        // m_Car.transform.rotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(-30, 30), 0f));
        m_CarRb.velocity = Vector3.zero;
        m_CarRb.angularVelocity = Vector3.zero;
        t_y_pos = 1;
        
        // reset distance
        MaxDistance = (m_Car.transform.position - Destination).magnitude;
        t_distance = (m_Car.transform.position - Destination).magnitude;
        MaxAngle = 270;
        t_angle = Math.Abs(m_Car.transform.rotation.eulerAngles[1] - 306);

        // reset p_value
        p_angle = t_angle;
        p_distance = t_distance;

         //drwa line
        lineObject = Instantiate(emptyPrefab, m_Car.transform.position, Quaternion.identity, gameObject.transform);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(m_Car.NormSteer);
        sensor.AddObservation(Normalization.Sigmoid(m_Car.LocalSpin));
        sensor.AddObservation(Normalization.Sigmoid(m_Car.LocalVelocity));
        sensor.AddObservation(m_Car.transform.position);
        sensor.AddObservation(Destination);
    }
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actions = actionBuffers.ContinuousActions;
        m_Car.Move(actions[0], actions[1]*200, actions[1], actions[2]);
        IsEndEpisode();

        t_distance = (m_Car.transform.position - Destination).magnitude;
        t_angle = Math.Abs(m_Car.transform.rotation.eulerAngles[1] - 306);
        
        float distance_reward = -t_distance / MaxDistance;
        float angle_reward = -t_angle / MaxAngle;
        // float distance_reward = (p_distance - t_distance) / MaxDistance * 100;
        // float angle_reward = (p_angle - t_angle) / MaxAngle *50;
        
        AddReward(distance_reward * 2 + angle_reward);
        Debug.Log(distance_reward * 2 + angle_reward);

        p_distance = t_distance;
        p_angle = t_angle;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
        actions[1] = Mathf.RoundToInt(Input.GetAxis("Vertical"));
        actions[2] = Mathf.RoundToInt(Input.GetAxis("Jump"));

    }

    private void FixedUpdate()
    {
           //drwa line
        if (timer <= 0)
        {
            currentline = Instantiate(lineprefab,m_Car.transform.position, Quaternion.identity, lineObject.transform);
            line = currentline.GetComponentInChildren<LineRenderer>();
            pos.Add(m_Car.transform.position);
            path = pos.ToArray();
            timer = 0.1f;
        }
        timer -= Time.deltaTime;

        if (path.Length != 0)
        {
            line.positionCount = path.Length;
            line.SetPositions(path);
            foreach(Vector3[] item in paths){
                line.positionCount = item.Length;
                line.SetPositions(item);
            }
        }

    }


    // private void updateSenorAround(int Flag, int x, int y)
    // {
    //     x = x + Size / 2;
    //     y = y + Size / 2;
    //     int maxN = (aroundScale - 1) / 2;
    //     int minN = (1 - aroundScale) / 2; 
    //     for (int i = minN; i <= maxN; i++)
    //     {
    //         for (int j = minN; j <= maxN; j++)
    //         {
    //             m_SensorBuffer.Write(Flag, x+i, y+j, 1);
    //         }
    //     }
    // }

    private void IsEndEpisode()
    {
        // Success
        Vector3 CarRota = m_Car.transform.rotation.eulerAngles;
        float speed = Math.Abs(m_Car.ForwardSpeed);
        float angle = Math.Abs(CarRota[1] - 306);
        // Debug.Log("distance"+t_distance);
        // Debug.Log("angle"+m_Car.transform.rotation.eulerAngles[1]);
        if (t_distance < 3.8 && speed < 1.5 && angle < 18)
        {
            paths.Add(path);
            pos = new List<Vector3>();
            Debug.Log("success");
            AddReward(2000);
            EndEpisode();
        }

        // // Failed: out of space
        // if (!Physics.Raycast(m_Car.transform.position, Vector3.down, 3f) || t_y_pos < 1)
        // {
        //     Debug.Log("out");
        //     AddReward(-1000);
        //     EndEpisode();
        // }

        // Failed: Collision
        Vector3 RayPos = m_Car.transform.position  + new Vector3(0, 1, 0);
        for (int i = 0; i < RayNum; i ++)
        {
            float subAngle = -(360 / RayNum) * i;
            Quaternion q = Quaternion.AngleAxis(subAngle, Vector3.up);
            Vector3 forward = q * m_Car.transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(RayPos, forward * 3, Color.green);
            //调整射线长度，使长方形车身前后与左右方向探测距离相等
            float rayLength;
            if(i<=2 || i>= RayNum-3){ //前方
                rayLength = 2.5f;
            }else if(i>=RayNum/2 && i<= RayNum/2+2){ //后方
                rayLength = 3.0f;
            }else {          //左右
                rayLength = 1.5f; 
            }

            if (Physics.Raycast(RayPos, forward, rayLength))
            {
                Debug.Log("collision");
                AddReward(-1000);
                EndEpisode();  
                break;
            }
        }

    }
}
