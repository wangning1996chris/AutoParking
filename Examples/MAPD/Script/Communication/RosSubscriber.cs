using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Geometry;


public class RosSubscriber : MonoBehaviour
{
    private string LaserScanInfo;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<LaserScanMsg>("scan", PrintLaserScan);
        // ROSConnection.GetOrCreateInstance().Subscribe<PoseStampedMsg>("odom", PrintOdomScan);
    }

    void PrintLaserScan(LaserScanMsg LaserScan)
    {
        // Debug.Log(string.Join(",",LaserScan.ranges));
        LaserScanInfo = string.Join(",",LaserScan.ranges);
    }

    void PrintOdomScan(PoseStampedMsg OdomScan)
    {
        Debug.Log(string.Join(",",OdomScan));
    }

    public string Update()
    {
        Debug.Log(LaserScanInfo);
        return LaserScanInfo;
    }
}