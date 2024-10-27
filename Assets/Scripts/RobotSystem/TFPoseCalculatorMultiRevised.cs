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
    [SerializeField] string mapToBaseFootprint = "map_to_base_footprint";
    [SerializeField] string odomTopicName = "odom";
    [SerializeField] string rosNamespace = "";
    [SerializeField] bool useOdometryMessage = true;
    Transform mapFrameTransform; 
    Pose mapFramePose = new Pose();
    Pose odomFramePose = new Pose();
    
    void Start()
    {
        mapToBaseFootprint = "/" + mapToBaseFootprint;
        mapToOdomTopic = "/" + mapToOdomTopic;
        odomTopicName = "/" + odomTopicName;

        if(rosNamespace != "")
        {
            mapToBaseFootprint = "/" + rosNamespace + mapToBaseFootprint;
            mapToOdomTopic = "/" + rosNamespace + mapToOdomTopic;
            odomTopicName = "/" + rosNamespace + odomTopicName;
        }
        
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseMsg>(mapToOdomTopic, ReciveMapToOdomMsg);

        if(useOdometryMessage)
        {
            ros.Subscribe<OdometryMsg>(odomTopicName, ReciveSimpleOdomMsg);
        }
        else
        {
            ros.Subscribe<PoseMsg>(mapToBaseFootprint, ReciveMapToBaseFootprint);
        }       
        

        var empty = new GameObject();
        mapFrameTransform = empty.transform; 
    }

    void Update()
    {
        mapFrameTransform.position = mapFramePose.position;
        mapFrameTransform.rotation = mapFramePose.rotation;

        //odomフレームの座標をmapフレーム基準に変換する
        targetObject.transform.position = TFUtility.GetRelativePosition(mapFrameTransform, odomFramePose.position) - mapTransformer.OriginPos;
        targetObject.transform.rotation = TFUtility.GetRelativeRotation(mapFrameTransform, odomFramePose.rotation);
             
    }

    void ReciveMapToOdomMsg(PoseMsg msg)
    { 
        Vector3Msg vector3Msg = new Vector3Msg();
        vector3Msg.x = msg.position.x;
        vector3Msg.y = msg.position.y;
        vector3Msg.z = msg.position.z;

        mapFramePose =  TFUtility.ConvertToUnityPose(vector3Msg, msg.orientation);
    }

    void ReciveMapToBaseFootprint(PoseMsg msg)
    {
        Vector3Msg vector3Msg = new Vector3Msg();
        vector3Msg.x = msg.position.x;
        vector3Msg.y = msg.position.y;
        vector3Msg.z = msg.position.z;

        odomFramePose = TFUtility.ConvertToUnityPose(vector3Msg, msg.orientation);
    }

    void ReciveSimpleOdomMsg(OdometryMsg msg)
    {
        Vector3Msg vector3Msg = new Vector3Msg();
        vector3Msg.x = msg.pose.pose.position.x;
        vector3Msg.y = msg.pose.pose.position.y;
        vector3Msg.z = msg.pose.pose.position.z;

        odomFramePose = TFUtility.ConvertToUnityPose(vector3Msg, msg.pose.pose.orientation);
    }
}