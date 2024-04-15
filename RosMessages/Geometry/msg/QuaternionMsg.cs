//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Geometry
{
    [Serializable]
    public class QuaternionMsg : Message
    {
        public const string k_RosMessageName = "geometry_msgs/Quaternion";
        public override string RosMessageName => k_RosMessageName;

        //  This represents an orientation in free space in quaternion form.
        public double x;
        public double y;
        public double z;
        public double w;

        public QuaternionMsg()
        {
            this.x = 0.0;
            this.y = 0.0;
            this.z = 0.0;
            this.w = 0.0;
        }

        public QuaternionMsg(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static QuaternionMsg Deserialize(MessageDeserializer deserializer) => new QuaternionMsg(deserializer);

        private QuaternionMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.x);
            deserializer.Read(out this.y);
            deserializer.Read(out this.z);
            deserializer.Read(out this.w);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.x);
            serializer.Write(this.y);
            serializer.Write(this.z);
            serializer.Write(this.w);
        }

        public override string ToString()
        {
            return "QuaternionMsg: " +
            "\nx: " + x.ToString() +
            "\ny: " + y.ToString() +
            "\nz: " + z.ToString() +
            "\nw: " + w.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
