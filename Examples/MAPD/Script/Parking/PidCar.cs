using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityStandardAssets.Vehicles.Car;
using MBaske.MLUtil;
using System.Data;
using System.IO;
using System.Text;
// using MBaske.Sensors.Grid;


public class PidCar : Agent
{
    private CarController m_Car;
    private Rigidbody m_CarRb;
    private float yaw_old = 0f;
    private float next_x;
    private float next_y;
    private float next_yaw;
    private int FrameRate = 50;
    private float MinDistance = 2000f;
    public int CarVel;
    public int MaximumSteerAngle;
    public DataTable dt;
    public int Step;

    
    public override void Initialize()
    {
        Application.targetFrameRate = FrameRate;

        m_Car = GetComponentInChildren<CarController>();
        m_Car.Initialize();
        m_CarRb = GetComponentInChildren<Rigidbody>();

        GetPath();
        Step = 0;
        m_Car.gameObject.SetActive(true);
        
        // Reset agent
        List<float> t_pos = GetPos();
        m_Car.transform.position = new Vector3(t_pos[0], 0.6f, t_pos[1]);
        m_Car.transform.rotation = Quaternion.Euler(new Vector3(0, t_pos[2] * 180f / (float)Math.PI, 0));
        // Debug.Log(Convert.ToString(t_pos[0]));
        // Debug.Log(Convert.ToString(t_pos[1]));

        // Get Next Pos
        t_pos = GetPos();
        next_x = t_pos[0];
        next_y = t_pos[1];
        next_yaw = t_pos[2] * 180f / (float)Math.PI;
    }


    void FixedUpdate()
    {
        // Stop Agent
        if (Step >= MaxStep)
        {
            m_Car.gameObject.SetActive(false);
            return;
        }
        
        // Get Now Pos
        Vector3 NowPos = m_Car.transform.position;
        float now_x = NowPos[0];
        float now_y = NowPos[2];

        float PosDistance = CalculateEuclideanDistance(now_x, now_y, next_x, next_y);
        if (PosDistance < MinDistance)
        {
            List<float> t_pos = GetPos();
            next_x = t_pos[0];
            next_y = t_pos[1];
            next_yaw = t_pos[2] * 180f / (float)Math.PI;
        }

        // Calculate Cmd
        List<float> t_cmd = PidCmd();
        float steer = t_cmd[0];
        float accel = t_cmd[1];

        // Norm Cmd
        float NormSteer = steer / MaximumSteerAngle;
        float NormAccel = accel * -1;

        // float steering, float accel, float footbrake, float handbrake
        // steer (-1, 1)
        // footbrake (-1, 0)
        // m_Car.Move(0f, 0f, -0.5f, 0f); 
        m_Car.Move(NormSteer, 0f, NormAccel, 0f);
    }

    public List<float> PidCmd()
    {
        // Pid_cmd List
        List<float> pid_cmd = new List<float>();

        // Get vel and yaw
        Vector3 localVel = m_Car.LocalVelocity;
        float now_vel = localVel.sqrMagnitude;
        Vector3 localYaw = m_Car.transform.rotation.eulerAngles;
        float now_yaw = localYaw[1];

        // calculate accel
        float accel = 2f * (CarVel - now_vel); 

        // calculate yaw
        float dy = (next_yaw - now_yaw);
        float steer = 0.2f * dy;

        Debug.Log(Convert.ToString(dy));

        // update pid_cmd
        pid_cmd.Add(steer);
        pid_cmd.Add(accel);

        return pid_cmd;
    }

    public List<float> GetPos()
    {
        List<float> pos = new List<float>();
        pos.Add(Convert.ToSingle(dt.Rows[Step][0]));
        pos.Add(Convert.ToSingle(dt.Rows[Step][1]));
        pos.Add(Convert.ToSingle(dt.Rows[Step][2]));
        Step += 2;
        return pos;
    }

    public void GetPath()
    {
        string filePath = "Assets\\Examples\\MAPD\\Script\\Parking\\real_path_1.csv";
        dt = OpenCSV(filePath);
    }

    public DataTable OpenCSV(string filePath)
    {
        DataTable dt = new DataTable();
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                string strLine = "";
                string[] aryLine = null;
                string[] tableHead = null;

                int columnCount = 0;
                bool IsFirst = true;
                while ((strLine = sr.ReadLine()) != null)
                {
                    if (IsFirst == true)
                    {
                        tableHead = strLine.Split(',');
                        IsFirst = false;
                        columnCount = tableHead.Length;
                        for (int i = 0; i < columnCount; i++)
                        {
                            DataColumn dc = new DataColumn(tableHead[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        aryLine = strLine.Split(',');
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                        {
                            dr[j] = aryLine[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                if (aryLine != null && aryLine.Length > 0)
                {
                    dt.DefaultView.Sort = tableHead[0] + " " + "asc";
                }
                sr.Close();
                fs.Close();
                return dt;
            }
        }
    }

    static float Pi2Pi(double theta)
    {
        while (theta > Math.PI)
        {
            theta -= 2 * Math.PI;
        }

        while (theta < -Math.PI)
        {
            theta += 2 * Math.PI;
        }
        return (float)theta;
    }

    static float CalculateEuclideanDistance(float x1, float y1, float x2, float y2)
    {
        float deltaX = x1 - x2;
        float deltaY = y1 - y2;
        float sumOfSquares = deltaX * deltaX + deltaY * deltaY;
        
        return (float)Math.Sqrt(sumOfSquares);
    }
}
