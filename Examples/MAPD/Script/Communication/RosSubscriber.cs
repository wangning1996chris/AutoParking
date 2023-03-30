using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using 

RosMessageTypes.Sensor;

public class RosSubscriber : MonoBehaviour
{

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<LaserScanMsg>("scan", PrintLaserScan);
    }

    void PrintLaserScan(LaserScanMsg LaserScan)
    {
        Debug.Log(string.Join(",",LaserScan.ranges));
    }
}