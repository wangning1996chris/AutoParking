using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

/// <summary>
///
/// </summary>
public class RosPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/cmd_vel";
    public float publishMessageFrequency = 0.5f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(topicName);
    }

    public void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            Vector3Msg linear = new Vector3Msg(1,0,0);
            Vector3Msg angular = new Vector3Msg(0,0,0);

            TwistMsg twist = new TwistMsg(linear,angular);

            // Finally send the message to server_endpoint.py running in ROS
            ros.Publish(topicName, twist);
            timeElapsed = 0;
        }
    }
}