using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics;
using RosMessageTypes.Nav;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;

public class T265Subscriber : MonoBehaviour
{
    ROSConnection ros;
    [SerializeField] string topicName = "camera/pose/sample";
    [SerializeField] GameObject debugObject;
    Pose t265TrackerPose = new Pose();

    // Start is called before the first frame update
    void Start()
    {
        topicName = "/" + topicName;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OdometryMsg>(topicName, OnSubscribeVisualOdometry);

    }

    // Update is called once per frame
    void Update()
    {
        debugObject.transform.position = t265TrackerPose.position;
        debugObject.transform.rotation = t265TrackerPose.rotation;
    }

    void OnSubscribeVisualOdometry(OdometryMsg msg)
    {
        Vector3Msg vector3Msg = new Vector3Msg();
        vector3Msg.x = msg.pose.pose.position.x;
        vector3Msg.y = msg.pose.pose.position.y;
        vector3Msg.z = msg.pose.pose.position.z;

        t265TrackerPose = TFUtility.ConvertToUnityPose(vector3Msg, msg.pose.pose.orientation);
    }
}
