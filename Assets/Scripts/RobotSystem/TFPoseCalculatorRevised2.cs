using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;
using RosMessageTypes.Tf2;
using RosMessageTypes.Nav;

/// <summary>
/// TFトピックをサブスクライブしてロボットの絶対座標を計算するスクリプト
/// </summary>
public class TFPoseCalculatorRevised2 : MonoBehaviour
{
    [SerializeField] MapTransformer mapTransformer;
    [SerializeField] GameObject targetObject;
    ROSConnection ros;
    [SerializeField] string parentFrameName = "odom";
    [SerializeField] string childFrameName = "base_footprint";
    [SerializeField] string rosNamespace = "";
    string topicName = "/tf";
    string odomTopicName = "/odom";
    Transform mapFrameTransform; 
    Pose mapFramePose = new Pose();
    Pose odomFramePose = new Pose();
    
    void Start()
    {
        if(rosNamespace != "")
        {
            topicName = "/" + rosNamespace + topicName;
            odomTopicName = "/" + rosNamespace + odomTopicName;
        }
        
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<TFMessageMsg>(topicName, ReceiveTFMsg);
        ros.Subscribe<OdometryMsg>(odomTopicName, ReciveOdomMsg);
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

    void ReceiveTFMsg(TFMessageMsg msg)
    {        
        for (int i = 0; i < msg.transforms.Length; i++)
            {
            if (msg.transforms[i].child_frame_id == parentFrameName) {
                mapFramePose = TFUtility.ConvertToUnityPose(msg.transforms[i].transform.translation, msg.transforms[i].transform.rotation);
            // } else if (msg.transforms[i].child_frame_id == childFrameName) {
            //     odomFramePose = TFUtility.ConvertToUnityPose(msg.transforms[i].transform.translation, msg.transforms[i].transform.rotation);
            }  
        } 
    }

    void ReciveOdomMsg(OdometryMsg msg)
    {
        Vector3Msg vector3Msg = new Vector3Msg();
        vector3Msg.x = msg.pose.pose.position.x;
        vector3Msg.y = msg.pose.pose.position.y;
        vector3Msg.z = msg.pose.pose.position.z;
        odomFramePose = TFUtility.ConvertToUnityPose(vector3Msg, msg.pose.pose.orientation);
    }
}

