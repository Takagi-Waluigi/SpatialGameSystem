using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;
using RosMessageTypes.Nav;
using RosMessageTypes.Tf2;

/// <summary>
/// TFトピックをサブスクライブしてロボットの絶対座標を計算するスクリプト
/// </summary>
public class TFPoseCalculatorRevised : MonoBehaviour
{
    [SerializeField] MapTransformer mapTransformer;
    [SerializeField] GameObject targetObject;
    ROSConnection ros;
    [SerializeField] string mapToOdomTopic = "map_to_odom";
    [SerializeField] string odomToBaseFootprintTopic = "odom_to_base_footprint";
    [SerializeField] string rosNamespace = "";
    Transform mapFrameTransform; 
    Pose mapFramePose = new Pose();
    Pose odomFramePose = new Pose();
    
    void Start()
    {
        mapToOdomTopic = "/" + mapToOdomTopic;
        odomToBaseFootprintTopic = "/" + odomToBaseFootprintTopic;

        if(rosNamespace != "")
        {
            mapToOdomTopic = "/" + rosNamespace + mapToOdomTopic;
            odomToBaseFootprintTopic = "/" + rosNamespace + odomToBaseFootprintTopic;
        }
        
        ros = ROSConnection.GetOrCreateInstance();

        ros.Subscribe<PoseStampedMsg>(mapToOdomTopic, ReciveMapToOdomMsg);
        ros.Subscribe<PoseStampedMsg>(odomToBaseFootprintTopic, ReciveOdomToBaseFootprintMsg);

        var empty = new GameObject();
        mapFrameTransform = empty.transform; 
    }

    void Update()
    {
        mapFrameTransform.position = mapFramePose.position;
        mapFrameTransform.rotation = mapFramePose.rotation;
        // odomフレームの座標をmapフレーム基準に変換する
        targetObject.transform.position = TFUtility.GetRelativePosition(mapFrameTransform, odomFramePose.position) - mapTransformer.OriginPos;
        targetObject.transform.rotation = TFUtility.GetRelativeRotation(mapFrameTransform, odomFramePose.rotation);
    }

    void ReciveMapToOdomMsg(PoseStampedMsg msg)
    {    
        Debug.Log("Get Data -A");
        Vector3Msg vector3Msg = new Vector3Msg();
        vector3Msg.x = msg.pose.position.x;
        vector3Msg.y = msg.pose.position.y;
        vector3Msg.z = msg.pose.position.z;

        mapFramePose =  TFUtility.ConvertToUnityPose(vector3Msg, msg.pose.orientation);
    }

    void ReciveOdomToBaseFootprintMsg(PoseStampedMsg msg)
    {    
        Debug.Log("Get Data -B");
        Vector3Msg vector3Msg = new Vector3Msg();
        vector3Msg.x = msg.pose.position.x;
        vector3Msg.y = msg.pose.position.y;
        vector3Msg.z = msg.pose.position.z;

        odomFramePose =  TFUtility.ConvertToUnityPose(vector3Msg, msg.pose.orientation);
    }
}