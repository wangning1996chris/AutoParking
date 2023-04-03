using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Geometry;


public class RosSubscriber : MonoBehaviour
{
    private string LaserScanInfo;
    private double[] posListInfo;

    void Start()
    {
        // ROSConnection.GetOrCreateInstance().Subscribe<LaserScanMsg>("scan", PrintLaserScan);
        ROSConnection.GetOrCreateInstance().Subscribe<PoseWithCovarianceStampedMsg>("amcl_pose", PrintPoseData);
    }

    void PrintLaserScan(LaserScanMsg LaserScan)
    {
        LaserScanInfo = string.Join(",", LaserScan.ranges);
    }

    void PrintPoseData(PoseWithCovarianceStampedMsg PoseData)
    {
        double p_x = PoseData.pose.pose.position.x;
        double p_y = PoseData.pose.pose.position.y;
        double p_z = PoseData.pose.pose.position.z;

        double o_x = PoseData.pose.pose.orientation.x;
        double o_y = PoseData.pose.pose.orientation.y;
        double o_z = PoseData.pose.pose.orientation.z;
        double o_w = PoseData.pose.pose.orientation.w;

        posListInfo = new double[7]{p_x, p_y, p_z, o_x, o_y, o_z, o_w};
    }

    public double[] UpdatePos()
    {
        // string TransformInfo = string.Join(",", posListInfo);
        // Debug.Log(TransformInfo);
        return posListInfo;
    }

    public string UpdateScan()
    {
        return LaserScanInfo;
    }
}