using System;
using System.IO;
using System.Collections;
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
public class Pit : Agent
{
    // RayPerceptionSensorComponent3D m_RayPerceptionSensor;
    // public GridBuffer m_SensorBuffer;
    public GameObject ParkSpot;
    private Vector3 Destination;
    private CarController m_Car;
    private Rigidbody m_CarRb;

    private const int Goal = 0;
    private const int Agent = 1;
    private const int aroundScale = 4;
    private float t_distance, t_angle;
    private float MaxDistance, MaxAngle;
    private float p_distance, p_angle;

    private float total_reward;

    private float t_y_pos; //terrain y 
    private const int RayNum = 18;

    private ArrayList  List  =  new  ArrayList(); 

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
        // m_RayPerceptionSensor = GetComponentInChildren<RayPerceptionSensorComponent3D>();

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
        // int random_num = UnityEngine.Random.Range(0,3);
        int random_num = 0;
        switch(random_num)
        {
            case 0:  //start to park
                Destination = new Vector3(145, 1, 150);
                ParkSpot.transform.position = new Vector3(145, 0, 150);
                ParkSpot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                m_Car.transform.position = new Vector3(-135, 1, 3)+new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10));
                m_Car.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)+new Vector3(0f, UnityEngine.Random.Range(-15, 15), 0f));
                break;
            case 1:  //park to shovel
                Destination = new Vector3(98, 1, -237);
                ParkSpot.transform.position = new Vector3(98, 1, -237);
                ParkSpot.transform.rotation = Quaternion.Euler(new Vector3(0,305, 0));
                m_Car.transform.position = new Vector3(174, 1, 150)+new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
                m_Car.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0)+new Vector3(0f, UnityEngine.Random.Range(-5, 5), 0f));
                break;
            case 2:  //shovel back to start
                Destination = new Vector3(-135, 1, 3);
                ParkSpot.transform.position = new Vector3(-135, 1, 3);
                ParkSpot.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                m_Car.transform.position = new Vector3(116, 1, -259)+new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
                m_Car.transform.rotation = Quaternion.Euler(new Vector3(0, 306, 0)+new Vector3(0f, UnityEngine.Random.Range(-5, 5), 0f));
                break;
            
        }
       
        m_CarRb.velocity = Vector3.zero;
        m_CarRb.angularVelocity = Vector3.zero;
        t_y_pos = 1;
        
        // reset distance
        MaxDistance = (m_Car.transform.position - Destination).magnitude;
        t_distance = (m_Car.transform.position - Destination).magnitude;
        MaxAngle = 270;
        t_angle = Math.Abs(m_Car.transform.rotation.eulerAngles[1] - ParkSpot.transform.rotation.eulerAngles[1]);

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
        sensor.AddObservation(Destination-m_Car.transform.position);
    }
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actions = actionBuffers.ContinuousActions;
        m_Car.Move(actions[0], actions[1]*100, actions[1], actions[2]);
        IsEndEpisode();

        t_distance = (m_Car.transform.position - Destination).magnitude;

        // float distance_reward = -t_distance / MaxDistance;
        // float angle_reward = -t_angle / MaxAngle;
        float distance_reward = (p_distance - t_distance)  * 10;
        // float angle_reward = (p_angle - t_angle) / MaxAngle ;
        
        AddReward(distance_reward );
        // Debug.Log(distance_reward);
        // Debug.Log(m_CarRb.velocity.magnitude);
        List.Add(m_CarRb.velocity.magnitude);

        p_distance = t_distance;

        
        // RayPerceptionInput spec = m_RayPerceptionSensor.GetRayPerceptionInput();
        // RayPerceptionOutput obs = RayPerceptionSensor.Perceive(spec);
        // RayPerceptionOutput.RayOutput rayoutput = obs.RayOutputs[0];
        // float[] output_buffer = new float[3];
        // rayoutput.ToFloatArray(1,0,output_buffer);
        // float left_distance = output_buffer[2];
        // if(rayoutput.HitTaggedObject&&(left_distance<0.2f || left_distance>0.4f)){
            
        //     float left_dirve_reward = -Math.Abs(left_distance-0.3f);
        //     AddReward(left_dirve_reward);
        //     Debug.Log("left drive reward is " + left_dirve_reward);
        //     // Debug.Log(left_distance);
        // }
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
        // m_SensorBuffer.Clear();
        
        // Agent
        Vector3 curr_Agent_Pos =  m_Car.transform.position;
        int a_x = (int)Math.Round(curr_Agent_Pos[0]);
        int a_y = (int)Math.Round(curr_Agent_Pos[2]);
        t_y_pos = curr_Agent_Pos[1];
        // updateSenorAround(Agent, a_x, a_y);
        
        // Goal
        Vector3 curr_Goal_Pos =  Destination;
        int g_x = (int)Math.Round(curr_Goal_Pos[0]);
        int g_y = (int)Math.Round(curr_Goal_Pos[2]);
        // updateSenorAround(Goal, g_x, g_y);
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

    private void IsEndEpisode()
    {
        // Success
        Vector3 CarRota = m_Car.transform.rotation.eulerAngles;
        float speed = Math.Abs(m_Car.ForwardSpeed);
        float angle = Math.Abs(CarRota[1] - 270);
        if (t_distance < 3.5 && speed < 1.5) 
        {   
            // using (StreamWriter sw = new StreamWriter("names.txt"))
            // {
            //     foreach (float  item in List)
            //     {
            //         sw.WriteLine(item);
            //     }
            // }
            paths.Add(path);
            pos = new List<Vector3>();
            Debug.Log("success");
            AddReward(2000);
            EndEpisode();
        }

        // Failed: out of space
        if (!Physics.Raycast(m_Car.transform.position, Vector3.down, 3f) || t_y_pos < 0) //Attention! 1 -> 0
        {
            // Debug.Log("out");
            AddReward(-1000);
            EndEpisode();
        }

        // Failed: Collision
        Vector3 RayPos = m_Car.transform.position  + new Vector3(0f, 0.5f, 0f);
        for (int i = 0; i < RayNum-1; i ++)
        {
            float subAngle = -(360 / RayNum) * i;
            Quaternion q = Quaternion.AngleAxis(subAngle, Vector3.up);
            Vector3 forward = q * m_Car.transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(RayPos, forward * 3, Color.green);
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