using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Geometry;
using RosMessageTypes.Nav;
// using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class RosSubscriber : MonoBehaviour
{
    private string LaserScanInfo;
    private float[] posListInfo;

    void Start()
    {
        // ROSConnection.GetOrCreateInstance().Subscribe<LaserScanMsg>("scan", PrintLaserScan);
        ROSConnection.GetOrCreateInstance().Subscribe<OdometryMsg>("base_pose_ground_truth", PrintPoseData);
    }

    // void PrintLaserScan(LaserScanMsg LaserScan)
    // {
    //     LaserScanInfo = string.Join(",", LaserScan.ranges);
    // }

    void PrintPoseData(OdometryMsg PoseData)
    {
        PointMsg position = PoseData.pose.pose.position;
        // Vector3 position = CoordinateSpaceExtensions.From(point,CoordinateSpaceSelection.FLU);
        QuaternionMsg orientation = PoseData.pose.pose.orientation;
        // Quaternion orientation = CoordinateSpaceExtensions.From(orien,CoordinateSpaceSelection.FLU);
        //(-Y,Z,X) (-Y,Z,X,-W) 
        posListInfo = new float[7]{-(float)position.y,(float)position.z,(float)position.x,-(float)orientation.y,(float)orientation.z,(float)orientation.x,-(float)orientation.w};
    }

    public float[] UpdatePos()
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