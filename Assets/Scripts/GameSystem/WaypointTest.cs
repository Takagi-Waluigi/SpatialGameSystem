using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;

using UnityEngine.Splines;

public class WaypointTest : MonoBehaviour
{
    ROSConnection ros;
    [Header("トピック名の設定")] 
    [SerializeField] string waypointTopicName = "goal_pose_unity";
    [SerializeField] string cancelTopicName = "cancel_status"; 
    [SerializeField] string distanceTopicName = "distance_remain";
    [SerializeField] string rosNamespace = "";
    float distance_remain = 0f;
    // Start is called before the first frame update
    void Start()
    {
        waypointTopicName = "/" + waypointTopicName;
        cancelTopicName = "/" + cancelTopicName;
        distanceTopicName = "/" + distanceTopicName;
        
        if(rosNamespace != "")
        {
            waypointTopicName = "/" + rosNamespace + waypointTopicName;
            cancelTopicName = "/" + rosNamespace + cancelTopicName;
            distanceTopicName = "/" + rosNamespace + distanceTopicName;            
        }
        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<PoseStampedMsg>(waypointTopicName);
        ros.RegisterPublisher<BoolMsg>(cancelTopicName);

        ros.Subscribe<Float32Msg>(distanceTopicName, DistanceInfoCallback);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.P))
        {   var msg = new PoseStampedMsg();

            msg.header.stamp.sec = 0;
            msg.header.stamp.nanosec = 0;
            msg.header.frame_id = "map";

            msg.pose = ConvertTransfromUnityToRos(Vector3.zero, 0);

            ros.Publish(waypointTopicName, msg);
        }

        if(Input.GetKeyUp(KeyCode.S)) CancelNavigation();
    }

    public void CancelNavigation()
    {
        Debug.Log("CancelNavigation called");
        BoolMsg msg = new BoolMsg();
        msg.data = true;

        ros.Publish(cancelTopicName, msg);

        //enableNavigation = false;
        Debug.Log("Navigation has been canceled.");
    }

    PoseMsg ConvertTransfromUnityToRos(Vector3 unityPosition, float unityRotation)
    {
        PoseMsg pose = new PoseMsg();
        pose.position.x = unityPosition.z;
        pose.position.y = -unityPosition.x;
        pose.position.z = 0f;

        Quaternion unity_quat = Quaternion.Euler(0f, 0f, unityRotation - 90f);
        pose.orientation.x = unity_quat.x;
        pose.orientation.y = unity_quat.y;
        pose.orientation.z = unity_quat.z;
        pose.orientation.w = unity_quat.w;

        return pose;
    }

    void DistanceInfoCallback(Float32Msg msg)
    {
        distance_remain = msg.data;
        Debug.Log($"Received distance: {distance_remain}");
    }
}
