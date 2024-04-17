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
    public float yaw_old = 0f;
    private int FrameRate = 20;
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
    }
    
    public override void OnEpisodeBegin()
    {
        m_Car.gameObject.SetActive(true);
        
        // reset agent
        List<float> t_pos = GetPos();
        float x = t_pos[0];
        float y = t_pos[1];
        float yaw = t_pos[2] * 180f / (float)Math.PI;
        m_Car.transform.position = new Vector3(x, 0.6f, y);
        m_Car.transform.rotation = Quaternion.Euler(new Vector3(0, yaw, 0));
    }

    void FixedUpdate()
    {
        // Stop
        if (Step >= MaxStep)
        {
            m_Car.gameObject.SetActive(false);
            return;
        }

        // Cal Cmd
        List<float> t_cmd = PidCmd();
        float steer = t_cmd[0];
        float accel = t_cmd[1];

        // Norm
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

        // Get Next Pos
        List<float> t_pos = GetPos();
        float next_x = t_pos[0];
        float next_y = t_pos[1];
        float next_yaw = t_pos[2] * 180f / (float)Math.PI;

        // Get vel and yaw
        Vector3 localVel = m_Car.LocalVelocity;
        float vel_now = localVel.sqrMagnitude;

        // calculate accel
        float accel = 0.3f * (CarVel - vel_now); 

        // calculate yaw
        float dy = (next_yaw - yaw_old) * FrameRate;
        float steer = Pi2Pi(-Math.Atan(4.5 * dy)) * 180f / (float)Math.PI;
        Debug.Log(Convert.ToString(steer));

        // update pid_cmd
        pid_cmd.Add(steer);
        pid_cmd.Add(accel);

        // update yaw_old
        yaw_old = next_yaw;

        return pid_cmd;
    }

    public List<float> GetPos()
    {
        List<float> pos = new List<float>();
        pos.Add(Convert.ToSingle(dt.Rows[Step][0]));
        pos.Add(Convert.ToSingle(dt.Rows[Step][1]));
        pos.Add(Convert.ToSingle(dt.Rows[Step][2]));
        Step += 1;
        return pos;
    }

    public float GetPos_i_step(int i, int j)
    {
        float res = Convert.ToSingle(dt.Rows[i][j]);
        return res;
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
}
