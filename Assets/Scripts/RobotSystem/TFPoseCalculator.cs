using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;
using RosMessageTypes.Tf2;

/// <summary>
/// TFトピックをサブスクライブしてロボットの絶対座標を計算するスクリプト
/// </summary>
public class TFPoseCalculator : MonoBehaviour
{
    [SerializeField] MapTransformer mapTransformer;
    [SerializeField] GameObject targetObject;
    ROSConnection ros;
    [SerializeField] string parentFrameName = "odom";
    [SerializeField] string childFrameName = "base_footprint";
    [SerializeField] string rosNamespace = "";
    string topicName = "/tf";
    Transform mapFrameTransform; 
    Pose mapFramePose = new Pose();
    Pose odomFramePose = new Pose();
    [SerializeField] float frameRate = 30f;
    float lastTime = 0f;
    void Start()
    {
        if(rosNamespace != "")
        {
            topicName = "/" + rosNamespace + topicName;
        }
        
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<TFMessageMsg>(topicName, ReceiveTFMsg);
        var empty = new GameObject();
        mapFrameTransform = empty.transform; 
    }

    void Update()
    {
        float interval = 1f / frameRate;

        if(Time.time - lastTime > interval)
        {
            mapFrameTransform.position = mapFramePose.position;
            mapFrameTransform.rotation = mapFramePose.rotation;
            // odomフレームの座標をmapフレーム基準に変換する
            targetObject.transform.position = TFUtility.GetRelativePosition(mapFrameTransform, odomFramePose.position) - mapTransformer.OriginPos;
            targetObject.transform.rotation = TFUtility.GetRelativeRotation(mapFrameTransform, odomFramePose.rotation);
            
            lastTime = Time.time;
        }
        
    }

    void ReceiveTFMsg(TFMessageMsg msg)
    {        
        for (int i = 0; i < msg.transforms.Length; i++)
            {
            if (msg.transforms[i].child_frame_id == parentFrameName) {
                mapFramePose = TFUtility.ConvertToUnityPose(msg.transforms[i].transform.translation, msg.transforms[i].transform.rotation);
            } else if (msg.transforms[i].child_frame_id == childFrameName) {
                odomFramePose = TFUtility.ConvertToUnityPose(msg.transforms[i].transform.translation, msg.transforms[i].transform.rotation);
            }  
        } 
    }
}

/// <summary>
/// TFの計算に使用する関数群
/// </summary>
public static class TFUtility {
    public static Pose ConvertToUnityPose(Vector3Msg rosPosMsg, QuaternionMsg rosQuaternionMsg)
    {
        Pose unityPose = new Pose();

        Vector3<Unity.Robotics.ROSTCPConnector.ROSGeometry.FLU> rosPos = rosPosMsg.As<FLU>();
        Vector3 unityPos = rosPos.toUnity;
        unityPose.position = unityPos;

        Quaternion<Unity.Robotics.ROSTCPConnector.ROSGeometry.FLU> rosQuaternion = rosQuaternionMsg.As<FLU>();
        Quaternion unityQuaternion = rosQuaternion.toUnity;
        unityPose.rotation = unityQuaternion;

        return unityPose;
    }

    public static Vector3 GetRelativePosition(Transform t, Vector3 pos)
    {
        return t.TransformPoint(pos);
    }

    public static Quaternion GetRelativeRotation(Transform t, Quaternion rot)
    {
        return rot * t.rotation;
    }
}
