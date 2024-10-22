using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using Unity.Robotics;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;

using UnityEngine.Splines;

public enum WaypointDirection { Clockwise, CounterClockwise }

public class SplineWaypointNavigator : MonoBehaviour
{
    ROSConnection ros;

    [Header("必要なゲームオブジェクト")] 
    [SerializeField] GameObject robotObject;
    // [SerializeField] GameObject UiGameObject;
    [SerializeField] MapTransformer mapTransformer;

    [Header("Spline（経路）設定")]
    [SerializeField] GameObject splineObject; // スプラインのGameObjectをここにアタッチ

    [Header("しきい値やレートの調整")]
    [SerializeField] float distanceThreshold = 0.2f;
    [SerializeField] [Range(0, 1f)] float nearDistanceRatio = 0.5f;
    [SerializeField] float publishRate = 2f;

    [Header("トピック名の設定")] 
    [SerializeField] string waypointTopicName = "waypoints_unity";
    [SerializeField] string cancelTopicName = "cancel_status"; 
    [SerializeField] string distanceTopicName = "distance_remain";
    [SerializeField] string rosNamespace = "";

    [Header("ナビゲーション設定")]
    [SerializeField] WaypointDirection waypointDirection = WaypointDirection.Clockwise;

    List<PoseStampedMsg> waypointMessages = new List<PoseStampedMsg>();
    bool enableNavigation = false;
    int currentWaypointID = 0;
    float lastTime = 0f;
    float distance_remain = 0f;

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

        Debug.Log("Threshold Distance is " + distanceThreshold * nearDistanceRatio);

        if (splineObject != null)
        {
            GenerateWaypointsFromSpline();
            PublishWaypoints();
        }
        else
        {
            Debug.LogWarning("Splineオブジェクトがアタッチされていません！");
        }
    }

    // スプラインのコントロールポイントからウェイポイントを生成
    void GenerateWaypointsFromSpline()
    {
        Transform[] splinePoints = splineObject.GetComponentsInChildren<Transform>();
        Debug.Log($"Found {splinePoints.Length} markers on spline.");

        foreach (Transform point in splinePoints)
        {
            if (point != splineObject.transform) // 自身のスプラインオブジェクトを除外
            {
                PoseStampedMsg poseStamped = new PoseStampedMsg();
                poseStamped.header.stamp.sec = 0;
                poseStamped.header.stamp.nanosec = 0;
                poseStamped.header.frame_id = "map";

                Vector3 worldPosition = point.position + mapTransformer.OriginPos;
                poseStamped.pose = ConvertTransfromUnityToRos(worldPosition, 0);
                waypointMessages.Add(poseStamped);
            }
        }

        Debug.Log($"Total waypoints added: {waypointMessages.Count}");
        SortWaypoints();
    }

    // Waypointsを選択した方向に基づいて並べ替え
    void SortWaypoints()
    {
        if (waypointDirection == WaypointDirection.Clockwise)
        {
            Debug.Log("Sorting waypoints in Clockwise direction.");
        }
        else
        {
            Debug.Log("Sorting waypoints in CounterClockwise direction.");
            waypointMessages.Reverse(); // 反時計回りの場合はリストを逆にする
        }
    }

    void Update()
    {
        if (enableNavigation)
        {
            if (Time.time - lastTime > publishRate)
            {
                Debug.Log($"Publishing waypoint {currentWaypointID}: {waypointMessages[currentWaypointID].pose.position.x}, {waypointMessages[currentWaypointID].pose.position.y}");
                ros.Publish(waypointTopicName, waypointMessages[currentWaypointID]);
                lastTime = Time.time;

                if (distance_remain < distanceThreshold * nearDistanceRatio)
                {
                    Debug.Log($"Moving to next waypoint: {currentWaypointID + 1}");
                    currentWaypointID++;
                    if (currentWaypointID >= waypointMessages.Count)
                    {
                        // enableNavigation = false;
                        currentWaypointID = 0;
                        Debug.Log("Reached the last waypoint. Stopping navigation.");
                    }
                }
            }
        }

        if(Input.GetKeyUp(KeyCode.S)) CancelNavigation();
    }

    // ナビゲーションの開始
    public void PublishWaypoints()
    {
        Debug.Log("PublishWaypoints called");
        if (waypointMessages.Count > 0)
        {
            enableNavigation = true;
            currentWaypointID = 0;
            Debug.Log("Navigation has been started!!!");
        }
        else
        {
            Debug.LogWarning("Waypointsが設定されていません");
        }
    }

    // ナビゲーションのキャンセル
    public void CancelNavigation()
    {
        Debug.Log("CancelNavigation called");
        BoolMsg msg = new BoolMsg();
        msg.data = true;

        ros.Publish(cancelTopicName, msg);

        enableNavigation = false;
        Debug.Log("Navigation has been canceled.");
    }

    // Unityの座標をROSの座標に変換
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

    // ROSからの距離情報を受信
    void DistanceInfoCallback(Float32Msg msg)
    {
        distance_remain = msg.data;
        Debug.Log($"Received distance: {distance_remain}");
    }
}
