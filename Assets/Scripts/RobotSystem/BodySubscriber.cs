using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics;
using RosMessageTypes.Geometry;
using RosMessageTypes.Nav;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;


// # keypointの位置毎の名称定義
// KEYPOINTS_NAMES = [
//     "nose",  # 0
//     "eye(L)",  # 1
//     "eye(R)",  # 2
//     "ear(L)",  # 3
//     "ear(R)",  # 4
//     "shoulder(L)",  # 5
//     "shoulder(R)",  # 6
//     "elbow(L)",  # 7
//     "elbow(R)",  # 8
//     "wrist(L)",  # 9
//     "wrist(R)",  # 10
//     "hip(L)",  # 11
//     "hip(R)",  # 12
//     "knee(L)",  # 13
//     "knee(R)",  # 14
//     "ankle(L)",  # 15
//     "ankle(R)",  # 16
// ]

public class BodySubscriber : MonoBehaviour
{
    ROSConnection ros;

    [Header("基本設定")]
    [SerializeField] Transform baseTransform;
    [SerializeField] GameObject targetObject;
    [SerializeField] string topicName = "";  
    [SerializeField] string rosNamespace = "";  
    [SerializeField] [Range(0f, 5f)] float maxRange = 3.0f;
    [SerializeField] [Range(0f, 5f)] float minRange = 0.250f;
    [SerializeField] [Range(0, 16)] int bodyId = 13;
    [SerializeField] float coefThreshold;

    // Start is called before the first frame update
    void Start()
    {
        topicName = "/" + topicName;

        if(rosNamespace != "") topicName = "/" + rosNamespace + topicName;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseArrayMsg>(topicName, OnSubsribeArray);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     void OnSubsribeArray(PoseArrayMsg msg)
    {
        //Debug.Log(msg.poses.Length);

        targetObject.transform.position = new Vector3(
            (float)msg.poses[bodyId].position.x,
            (float)msg.poses[bodyId].position.y,
            (float)msg.poses[bodyId].position.z
        );

        Debug.Log(msg.poses[bodyId].orientation.x);

        
    }
}
