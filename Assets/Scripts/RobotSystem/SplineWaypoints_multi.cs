// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Robotics;
// using RosMessageTypes.Geometry;
// using RosMessageTypes.Std;
// using Unity.Robotics.ROSTCPConnector;
// using UnityEngine.Splines;

// [System.Serializable]
// public class RobotRoute
// {
//     public string robotNamespace;   // 各ロボットの名前空間（ロボット名）
//     public GameObject splineObject; // 各ロボットの経路用スプライン
//     public string waypointTag;      // 各ロボットに対応するウェイポイントのTag
// }

// [System.Serializable]
// public class RouteConfig
// {
//     public string routeName; // Routeの名前 (A, B, C など)
//     public List<RobotRoute> robotRoutes = new List<RobotRoute>(); // 最大8台のロボットとスプラインを管理
// }

// public class SplineWaypoints_multi : MonoBehaviour
// {
//     ROSConnection ros;

//     [Header("必要なゲームオブジェクト")] 
//     [SerializeField] GameObject UiGameObject;
//     [SerializeField] MapTransformer mapTransformer;

//     [Header("Route設定")]
//     [SerializeField] List<RouteConfig> routeConfigs = new List<RouteConfig>(); // 3つのRoute設定 (A, B, C)

//     [Header("しきい値やレートの調整")]
//     [SerializeField] float distanceThreshold = 0.2f;
//     [SerializeField] [Range(0, 1f)] float nearDistanceRatio = 0.5f;
//     [SerializeField] float publishRate = 2f;

//     [Header("トピック名の設定")] 
//     [SerializeField] string waypointTopicName = "waypoints_unity";
//     [SerializeField] string cancelTopicName = "cancel_status"; 
//     [SerializeField] string distanceTopicName = "distance_remain";

//     [Header("ナビゲーション設定")]
//     [SerializeField] WaypointDirection WaypointDirection = WaypointDirection.Clockwise;

//     bool enableNavigation = false;
//     float lastTime = 0f;
//     float distance_remain = 0f;
//     List<PoseStampedMsg> currentWaypoints;
//     string currentWaypointTopic;

//     // パブリッシャーが登録されているかどうかを追跡する辞書
//     Dictionary<string, bool> registeredPublishers = new Dictionary<string, bool>();

//     void Start()
//     {
//         ros = ROSConnection.GetOrCreateInstance();

//         // 初期設定として Route A を自動的に開始する
//         SwitchRoute(0); // Route A に対応するすべてのロボットをロードしてナビゲーション開始
//         PublishWaypoints(); // 初期設定として "Route A" のウェイポイントをパブリッシュ
//     }

//     // Routeを切り替えてすべてのロボットを動かす
//     public void SwitchRoute(int newRouteIndex)
//     {
//         // 新しいルートに切り替える前に、すべてのロボットのナビゲーションを一括キャンセル
//         CancelAllNavigation();

//         if (newRouteIndex < 0 || newRouteIndex >= routeConfigs.Count)
//         {
//             Debug.LogWarning("指定されたRouteインデックスが範囲外です！");
//             return;
//         }

//         RouteConfig selectedRoute = routeConfigs[newRouteIndex];
//         Debug.Log($"{selectedRoute.routeName} ルートが切り替えられました");

//         foreach (RobotRoute robotRoute in selectedRoute.robotRoutes)
//         {
//             string robotNamespace = robotRoute.robotNamespace;
//             string waypointTag = robotRoute.waypointTag;

//             // トピック名に名前空間を付加して設定
//             string fullWaypointTopicName = $"/{robotNamespace}/{waypointTopicName}";
//             string fullCancelTopicName = $"/{robotNamespace}/{cancelTopicName}";
//             string fullDistanceTopicName = $"/{robotNamespace}/{distanceTopicName}";

//             // パブリッシャーが登録されているかを辞書で追跡
//             if (!registeredPublishers.ContainsKey(fullCancelTopicName))
//             {
//                 // ROSのパブリッシャー登録
//                 ros.RegisterPublisher<PoseStampedMsg>(fullWaypointTopicName);
//                 ros.RegisterPublisher<BoolMsg>(fullCancelTopicName);

//                 // パブリッシャーが登録されたことを記録
//                 registeredPublishers[fullCancelTopicName] = true;
//             }

//             // 距離情報をサブスクライブ
//             ros.Subscribe<Float32Msg>(fullDistanceTopicName, (msg) => DistanceInfoCallback(msg, robotNamespace));

//             // ウェイポイント生成
//             List<PoseStampedMsg> waypointMessages = GenerateWaypointsFromSpline(robotRoute.splineObject, waypointTag);

//             // 現在のルートのウェイポイントとトピック名を保存
//             currentWaypoints = waypointMessages;
//             currentWaypointTopic = fullWaypointTopicName;
//         }
//     }

//     // スプラインのコントロールポイントからウェイポイントを生成
//     List<PoseStampedMsg> GenerateWaypointsFromSpline(GameObject splineObject, string waypointTag)
//     {
//         List<PoseStampedMsg> waypointMessages = new List<PoseStampedMsg>();
//         Transform[] splinePoints = splineObject.GetComponentsInChildren<Transform>();
//         Debug.Log($"Found {splinePoints.Length} markers on spline.");

//         foreach (Transform point in splinePoints)
//         {
//             if (point != splineObject.transform && point.CompareTag(waypointTag)) // 指定されたTagのポイントのみ処理
//             {
//                 PoseStampedMsg poseStamped = new PoseStampedMsg();
//                 poseStamped.header.stamp.sec = 0;
//                 poseStamped.header.stamp.nanosec = 0;
//                 poseStamped.header.frame_id = "map";

//                 Vector3 worldPosition = point.position + mapTransformer.OriginPos;
//                 poseStamped.pose = ConvertTransfromUnityToRos(worldPosition, 0);
//                 waypointMessages.Add(poseStamped);
//             }
//         }

//         if (waypointMessages.Count == 0)
//         {
//             Debug.LogWarning($"ウェイポイントが見つかりません。指定されたTag: {waypointTag}");
//         }
//         else
//         {
//             Debug.Log($"Total waypoints added: {waypointMessages.Count}");
//         }
        
//         SortWaypoints(waypointMessages);
//         return waypointMessages;
//     }

//     // Waypointsを選択した方向に基づいて並べ替え
//     void SortWaypoints(List<PoseStampedMsg> waypointMessages)
//     {
//         if (WaypointDirection == WaypointDirection.Clockwise)
//         {
//             Debug.Log("Sorting waypoints in Clockwise direction.");
//         }
//         else
//         {
//             Debug.Log("Sorting waypoints in CounterClockwise direction.");
//             waypointMessages.Reverse(); // 反時計回りの場合はリストを逆にする
//         }
//     }

//     // ウェイポイントを定期的にパブリッシュする
//     IEnumerator PublishWaypointsCoroutine(List<PoseStampedMsg> waypointMessages, string waypointTopicName)
//     {
//         int currentWaypointID = 0;
//         while (true)
//         {
//             if (Time.time - lastTime > publishRate)
//             {
//                 if (currentWaypointID >= waypointMessages.Count)
//                 {
//                     currentWaypointID = 0;
//                     Debug.Log("Reached the last waypoint. Looping back to the first waypoint.");
//                 }

//                 Debug.Log($"Publishing waypoint {currentWaypointID}: {waypointMessages[currentWaypointID].pose.position.x}, {waypointMessages[currentWaypointID].pose.position.y}");
//                 ros.Publish(waypointTopicName, waypointMessages[currentWaypointID]);
//                 lastTime = Time.time;

//                 currentWaypointID++;
//             }

//             yield return null;
//         }
//     }

//     // ウェイポイントを手動でパブリッシュする
//     public void PublishWaypoints()
//     {
//         if (currentWaypoints != null && currentWaypoints.Count > 0)
//         {
//             Debug.Log("PublishWaypoints called manually.");
//             StartCoroutine(PublishWaypointsCoroutine(currentWaypoints, currentWaypointTopic));
//         }
//         else
//         {
//             Debug.LogWarning("Waypointsが設定されていません");
//         }
//     }

//     // 距離情報を受け取って処理する
//     void DistanceInfoCallback(Float32Msg msg, string robotNamespace)
//     {
//         distance_remain = msg.data;
//         Debug.Log($"{robotNamespace} の残り距離: {distance_remain}");
//     }

//     // ナビゲーションをすべてのロボットで一括キャンセル
//     public void CancelAllNavigation()
//     {
//         foreach (RouteConfig route in routeConfigs)
//         {
//             foreach (RobotRoute robotRoute in route.robotRoutes)
//             {
//                 string fullCancelTopicName = $"/{robotRoute.robotNamespace}/{cancelTopicName}";
//                 BoolMsg msg = new BoolMsg();
//                 msg.data = true;

//                 // パブリッシャーが登録されているか確認
//                 if (registeredPublishers.ContainsKey(fullCancelTopicName) && registeredPublishers[fullCancelTopicName])
//                 {
//                     ros.Publish(fullCancelTopicName, msg);
//                     Debug.Log($"{robotRoute.robotNamespace} のナビゲーションがキャンセルされました。");
//                 }
//                 else
//                 {
//                     Debug.LogError($"No registered publisher on topic {fullCancelTopicName}");
//                 }
//             }
//         }
//     }

//     // Unityの座標をROSの座標に変換
//     PoseMsg ConvertTransfromUnityToRos(Vector3 unityPosition, float unityRotation)
//     {
//         PoseMsg pose = new PoseMsg();
//         pose.position.x = unityPosition.z;
//         pose.position.y = -unityPosition.x;
//         pose.position.z = 0f;

//         Quaternion unity_quat = Quaternion.Euler(0f, 0f, unityRotation - 90f);
//         pose.orientation.x = unity_quat.x;
//         pose.orientation.y = unity_quat.y;
//         pose.orientation.z = unity_quat.z;
//         pose.orientation.w = unity_quat.w;

//         return pose;
//     }

//     // RouteAボタン、RouteBボタン、RouteCボタンそれぞれに経路を設定
//     public void OnRouteAButtonPressed() // RouteAボタン用
//     {
//         SwitchRoute(0); // RouteAに対応するすべてのロボットをロード
//     }

//     public void OnRouteBButtonPressed() // RouteBボタン用
//     {
//         SwitchRoute(1); // RouteBに対応するすべてのロボットをロード
//     }

//     public void OnRouteCButtonPressed() // RouteCボタン用
//     {
//         SwitchRoute(2); // RouteCに対応するすべてのロボットをロード
//     }

//     public void OnCancelAllButtonPressed() // すべてのナビゲーションをキャンセル
//     {
//         CancelAllNavigation(); // 全ロボットのナビゲーションを一括でキャンセル
//     }
// }

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

[System.Serializable]
public class RobotRoute
{
    public string robotNamespace;   // 各ロボットの名前空間（ロボット名）
    public GameObject splineObject; // 各ロボットの経路用スプライン
    public string waypointTag;      // 各ロボットに対応するウェイポイントのTag
}

[System.Serializable]
public class RouteConfig
{
    public string routeName; // Routeの名前 (A, B, C など)
    public List<RobotRoute> robotRoutes = new List<RobotRoute>(); // 最大8台のロボットとスプラインを管理
}

public class SplineWaypoints_multi : MonoBehaviour
{
    ROSConnection ros;

    [Header("必要なゲームオブジェクト")] 
    [SerializeField] GameObject UiGameObject;
    [SerializeField] MapTransformer mapTransformer;

    [Header("Route設定")]
    [SerializeField] List<RouteConfig> routeConfigs = new List<RouteConfig>(); // 3つのRoute設定 (A, B, C)

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

    bool enableNavigation = false;
    float lastTime = 0f;
    float distance_remain = 0f;
    List<PoseStampedMsg> currentWaypoints;
    string currentWaypointTopic;

    // パブリッシャーが登録されているかどうかを追跡する辞書
    Dictionary<string, bool> registeredPublishers = new Dictionary<string, bool>();

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        // 初期設定として Route A を自動的に開始する
        SwitchRoute(0); // Route A に対応するすべてのロボットをロードしてナビゲーション開始
        PublishWaypoints(); // 初期設定として "Route A" のウェイポイントをパブリッシュ
    }

    // Routeを切り替えてすべてのロボットを動かす
    public void SwitchRoute(int newRouteIndex)
    {
        // 新しいルートに切り替える前に、すべてのロボットのナビゲーションを一括キャンセル
        CancelAllNavigation();

        if (newRouteIndex < 0 || newRouteIndex >= routeConfigs.Count)
        {
            Debug.LogWarning("指定されたRouteインデックスが範囲外です！");
            return;
        }

        RouteConfig selectedRoute = routeConfigs[newRouteIndex];
        Debug.Log($"{selectedRoute.routeName} ルートが切り替えられました");

        foreach (RobotRoute robotRoute in selectedRoute.robotRoutes)
        {
            string robotNamespace = robotRoute.robotNamespace;
            string waypointTag = robotRoute.waypointTag;

            // トピック名に名前空間を付加して設定
            string fullWaypointTopicName = $"/{robotNamespace}/{waypointTopicName}";
            string fullCancelTopicName = $"/{robotNamespace}/{cancelTopicName}";
            string fullDistanceTopicName = $"/{robotNamespace}/{distanceTopicName}";

            // パブリッシャーが登録されているかを辞書で追跡
            if (!registeredPublishers.ContainsKey(fullCancelTopicName))
            {
                // ROSのパブリッシャー登録
                ros.RegisterPublisher<PoseStampedMsg>(fullWaypointTopicName);
                ros.RegisterPublisher<BoolMsg>(fullCancelTopicName);

                // パブリッシャーが登録されたことを記録
                registeredPublishers[fullCancelTopicName] = true;
            }

            // 距離情報をサブスクライブ
            ros.Subscribe<Float32Msg>(fullDistanceTopicName, (msg) => DistanceInfoCallback(msg, robotNamespace));

            // ウェイポイント生成
            List<PoseStampedMsg> waypointMessages = GenerateWaypointsFromSpline(robotRoute.splineObject, waypointTag);

            // 現在のルートのウェイポイントとトピック名を保存
            currentWaypoints = waypointMessages;
            currentWaypointTopic = fullWaypointTopicName;
        }
    }

    // スプラインのコントロールポイントからウェイポイントを生成
    List<PoseStampedMsg> GenerateWaypointsFromSpline(GameObject splineObject, string waypointTag)
    {
        List<PoseStampedMsg> waypointMessages = new List<PoseStampedMsg>();
        Transform[] splinePoints = splineObject.GetComponentsInChildren<Transform>();
        Debug.Log($"Found {splinePoints.Length} markers on spline.");

        foreach (Transform point in splinePoints)
        {
            if (point != splineObject.transform && point.CompareTag(waypointTag)) // 指定されたTagのポイントのみ処理
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

        if (waypointMessages.Count == 0)
        {
            Debug.LogWarning($"ウェイポイントが見つかりません。指定されたTag: {waypointTag}");
        }
        else
        {
            Debug.Log($"Total waypoints added: {waypointMessages.Count}");
        }
        
        SortWaypoints(waypointMessages);
        return waypointMessages;
    }

    // Waypointsを選択した方向に基づいて並べ替え
    void SortWaypoints(List<PoseStampedMsg> waypointMessages)
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

    // ウェイポイントを定期的にパブリッシュする
    IEnumerator PublishWaypointsCoroutine(List<PoseStampedMsg> waypointMessages, string waypointTopicName)
    {
        int currentWaypointID = 0;
        while (true)
        {
            if (Time.time - lastTime > publishRate)
            {
                if (currentWaypointID >= waypointMessages.Count)
                {
                    currentWaypointID = 0;
                    Debug.Log("Reached the last waypoint. Looping back to the first waypoint.");
                }

                Debug.Log($"Publishing waypoint {currentWaypointID}: {waypointMessages[currentWaypointID].pose.position.x}, {waypointMessages[currentWaypointID].pose.position.y}");
                ros.Publish(waypointTopicName, waypointMessages[currentWaypointID]);
                lastTime = Time.time;

                currentWaypointID++;
            }

            yield return null;
        }
    }

    // ウェイポイントを手動でパブリッシュする
    public void PublishWaypoints()
    {
        if (currentWaypoints != null && currentWaypoints.Count > 0)
        {
            Debug.Log("PublishWaypoints called manually.");
            StartCoroutine(PublishWaypointsCoroutine(currentWaypoints, currentWaypointTopic));
        }
        else
        {
            Debug.LogWarning("Waypointsが設定されていません");
        }
    }

    // 距離情報を受け取って処理する
    void DistanceInfoCallback(Float32Msg msg, string robotNamespace)
    {
        distance_remain = msg.data;
        Debug.Log($"{robotNamespace} の残り距離: {distance_remain}");
    }

    // ナビゲーションをすべてのロボットで一括キャンセル
    public void CancelAllNavigation()
    {
        foreach (RouteConfig route in routeConfigs)
        {
            foreach (RobotRoute robotRoute in route.robotRoutes)
            {
                string fullCancelTopicName = $"/{robotRoute.robotNamespace}/{cancelTopicName}";
                BoolMsg msg = new BoolMsg();
                msg.data = true;

                // パブリッシャーが登録されているか確認
                if (registeredPublishers.ContainsKey(fullCancelTopicName) && registeredPublishers[fullCancelTopicName])
                {
                    ros.Publish(fullCancelTopicName, msg);
                    Debug.Log($"{robotRoute.robotNamespace} のナビゲーションがキャンセルされました。");
                }
                else
                {
                    Debug.LogError($"No registered publisher on topic {fullCancelTopicName}");
                }
            }
        }
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

    // RouteAボタン、RouteBボタン、RouteCボタンそれぞれに経路を設定
    public void OnRouteAButtonPressed() // RouteAボタン用
    {
        SwitchRoute(0); // RouteAに対応するすべてのロボットをロード
    }

    public void OnRouteBButtonPressed() // RouteBボタン用
    {
        SwitchRoute(1); // RouteBに対応するすべてのロボットをロード
    }

    public void OnRouteCButtonPressed() // RouteCボタン用
    {
        SwitchRoute(2); // RouteCに対応するすべてのロボットをロード
    }

    public void OnCancelAllButtonPressed() // すべてのナビゲーションをキャンセル
    {
        CancelAllNavigation(); // 全ロボットのナビゲーションを一括でキャンセル
    }
}
