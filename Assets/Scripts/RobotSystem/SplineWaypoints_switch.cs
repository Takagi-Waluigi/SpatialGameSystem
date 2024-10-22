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



public class SplineWaypoints_switch : MonoBehaviour
{
    ROSConnection ros;

    [Header("必要なゲームオブジェクト")] 
    [SerializeField] GameObject robotObject;
    [SerializeField] GameObject UiGameObject;
    [SerializeField] MapTransformer mapTransformer;

    [Header("スプライン設定")]
    [SerializeField] List<GameObject> splineObjects; // 複数のスプラインオブジェクトをリストで管理

    [Header("しきい値やレートの調整")]
    [SerializeField] float distanceThreshold = 0.2f;
    [SerializeField] [Range(0, 1f)] float nearDistanceRatio = 0.5f;
    [SerializeField] float publishRate = 2f;

    [Header("トピック名の設定")] 
    [SerializeField] string waypointTopicName = "waypoints_unity";
    [SerializeField] string cancelTopicName = "cancel_status"; 
    [SerializeField] string distanceTopicName = "distance_remain";

    [Header("ナビゲーション設定")]
    [SerializeField] WaypointDirection WaypointDirection = WaypointDirection.Clockwise;

    List<PoseStampedMsg> waypointMessages = new List<PoseStampedMsg>();
    bool enableNavigation = false;
    int currentWaypointID = 0;
    float lastTime = 0f;
    float distance_remain = 0f;
    int currentSplineIndex = 0;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<PoseStampedMsg>(waypointTopicName);
        ros.RegisterPublisher<BoolMsg>(cancelTopicName);

        ros.Subscribe<Float32Msg>(distanceTopicName, DistanceInfoCallback);

        Debug.Log("Threshold Distance is " + distanceThreshold * nearDistanceRatio);

        if (splineObjects.Count > 0)
        {
            LoadSpline(0); // 最初のスプラインを読み込む
        }
        else
        {
            Debug.LogWarning("スプラインオブジェクトが設定されていません！");
        }
    }

    // スプラインを切り替えてWaypointsを生成
    public void LoadSpline(int splineIndex)
    {
        if (splineIndex < 0 || splineIndex >= splineObjects.Count)
        {
            Debug.LogWarning("指定されたスプラインインデックスが範囲外です！");
            return;
        }

        currentSplineIndex = splineIndex;
        waypointMessages.Clear(); // 前のWaypointsをクリア
        GenerateWaypointsFromSpline(splineObjects[splineIndex]); // 新しいスプラインでWaypointsを生成
        Debug.Log($"Spline {splineIndex + 1} が読み込まれました");
    }

    // スプラインのコントロールポイントからウェイポイントを生成
    void GenerateWaypointsFromSpline(GameObject splineObject)
    {
        Transform[] splinePoints = splineObject.GetComponentsInChildren<Transform>();
        Debug.Log($"Found {splinePoints.Length} markers on spline.");

        foreach (Transform point in splinePoints)
        {
            if (point != splineObject.transform) // 自身のスプラインオブジェクトを除外
            {
                // 矢印のタグを確認し、それぞれ別の処理を行う
                if (point.CompareTag("Spline1_Arrow"))
                {
                    Debug.Log("Processing Spline 1 Arrow");
                    // Spline1用の処理を追加
                }
                else if (point.CompareTag("Spline2_Arrow"))
                {
                    Debug.Log("Processing Spline 2 Arrow");
                    // Spline2用の処理を追加
                }

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
        if (WaypointDirection == WaypointDirection.Clockwise)
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
                        currentWaypointID = 0;
                        Debug.Log("Reached the last waypoint. Looping back to the first waypoint.");
                    }
                }
            }
        }
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

    // Aボタン、Bボタンそれぞれに経路を設定
    public void SwitchToFirstSpline() // Aボタン用
    {
        SwitchSpline(0);
    }

    public void SwitchToSecondSpline() // Bボタン用
    {
        SwitchSpline(1);
    }

    // スプラインを切り替える
    public void SwitchSpline(int splineIndex)
    {
        LoadSpline(splineIndex);
    }
}
