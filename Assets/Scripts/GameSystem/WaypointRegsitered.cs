using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;

using UnityEngine.Splines;
using System;

public class WaypointRegsitered : MonoBehaviour
{
     ROSConnection ros;
    [SerializeField] MapTransformer mapTransformer;
    [Header("トピック名の設定")] 
    [SerializeField] string waypointTopicName = "goal_pose_unity";
    [SerializeField] string cancelTopicName = "cancel_status"; 
    [SerializeField] string distanceTopicName = "distance_remain";
    [SerializeField] string rosNamespace = "";
    [Header("ナビゲーションの設定")]
    [SerializeField] float publishRate = 1;
    [SerializeField] float minimumDisntanceThreshold = 0.5f;
    [SerializeField] Transform circleCenterTransform;
    [SerializeField] Transform[] waypoints;
    float distance_remain = 0f;
    int currentWaypointID = 0;
    float lastTime = 0f;

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
    
    void Update()
    {
        if(Time.time - lastTime > publishRate)
        {
            lastTime = Time.time;
            if(waypoints.Length > 0)
            {
                var wp = new PoseStampedMsg();
            
                wp.header.stamp.sec = 0;
                wp.header.stamp.nanosec = 0;
                wp.header.frame_id = "map";
                ros.Publish(waypointTopicName, wp);

                Vector3 position = new Vector3(
                    waypoints[currentWaypointID].position.x,
                    0f,
                    waypoints[currentWaypointID].position.z
                );

                Vector3 worldPosition = position + mapTransformer.OriginPos;

                wp.pose = ConvertTransfromUnityToRos(worldPosition, 0);

                if(distance_remain < minimumDisntanceThreshold)
                {
                    Debug.Log("Near!!! Then Start New Navgation!!!");
                    currentWaypointID = (currentWaypointID + 1) % waypoints.Length;
                }
            }   
            
            Debug.Log("Current ID is:" + currentWaypointID);
            Debug.Log($"Received distance: {distance_remain}");
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
        
    }
}
