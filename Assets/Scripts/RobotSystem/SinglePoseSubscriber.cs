using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

using RosMessageTypes.Nav;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using UnityEditorInternal.VR;
using Unity.VisualScripting;

/*
 * KEYPOINTS_NAMES = [
    "nose",  # 0
    "eye(L)",  # 1
    "eye(R)",  # 2
    "ear(L)",  # 3
    "ear(R)",  # 4
    "shoulder(L)",  # 5
    "shoulder(R)",  # 6
    "elbow(L)",  # 7
    "elbow(R)",  # 8
    "wrist(L)",  # 9
    "wrist(R)",  # 10
    "hip(L)",  # 11
    "hip(R)",  # 12
    "knee(L)",  # 13
    "knee(R)",  # 14
    "ankle(L)",  # 15
    "ankle(R)",  # 16
]
 */
public class SinglePoseSubscriber : MonoBehaviour
{
    ROSConnection ros;
    [Header("基本設定")]
    [SerializeField] GameObject leftFootObject;
    [SerializeField] GameObject rightFootObject;
    [SerializeField] string topicName;
    [SerializeField] string rosNamespace = "";
    [SerializeField] [Range(0f, 5f)] float maxRange = 3.0f;
    [SerializeField] [Range(0f, 5f)] float minRange = 0.250f;    
    [SerializeField] int keyPointsID_left = 13;    
    [SerializeField] int keyPointsID_right = 14;
    [SerializeField] int searchUserNumber = 5;    
    [SerializeField] [Range(0, 1)] float coefThreshold;

    //[SerializeField] bool localModePositonMode = false;
    [SerializeField] float offsetDepth;
    public Vector3 leftFootWorldPosition;
    public Vector3 rightFootWorldPosition;
    public Vector3 centerPosition;
    const int bufferSize = 10;
    const int disableFrameNumber = 50;
    Vector3[] keyPointsBuffer_left = new Vector3[bufferSize];
    Vector3[] keyPointsBuffer_right = new Vector3[bufferSize];
    int isNotIncomingData_num = 0;
    // bool isNotIncomingData_perFrame = false;
    bool isTrackingPerFrame = false;
    public bool isTracking = false;

    // Start is called before the first frame update
    void Start()
    {
        topicName = "/" + topicName;

        if (rosNamespace != "")
        {
            topicName = "/" + rosNamespace + topicName;
        }

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseArrayMsg>(topicName, OnSubsribeArray);

        for(int i = 0; i < bufferSize; i ++)
        {
            keyPointsBuffer_left[i] = Vector3.zero;
            keyPointsBuffer_right[i] = Vector3.zero;
        }

    }

    // Update is called once per frame
    void Update()
    {
        leftFootWorldPosition = leftFootObject.transform.position;
        rightFootWorldPosition = rightFootObject.transform.position;
        centerPosition = (leftFootWorldPosition + rightFootWorldPosition) * 0.5f;

        if(isTrackingPerFrame)
        {
            isNotIncomingData_num = 0;
        }
        else
        {
            isNotIncomingData_num ++;
        }

        isTracking = (isNotIncomingData_num < disableFrameNumber);


        isTrackingPerFrame = false;
        
    }

    void OnSubsribeArray(PoseArrayMsg msg)
    {
        //isNotIncomingData_perFrame = true;
        //Debug.Log("[" + rosNamespace + "]" + "Level:0");
        
        bool isTrackingL = false;
        bool isTrackingR = false;
        for(int i = 0; i < searchUserNumber; i ++)
        {
            if (msg.header.frame_id == i.ToString())
            {
                isTrackingL = CalculateBufferPosition(msg, keyPointsID_left, keyPointsBuffer_left,leftFootObject);
                isTrackingR = CalculateBufferPosition(msg, keyPointsID_right, keyPointsBuffer_right,rightFootObject);

                if(isTrackingL && isTrackingR) isTrackingPerFrame = true;
            }

        }
       
    }

    bool CalculateBufferPosition(PoseArrayMsg msg, int kpid, Vector3[] keyPointsBuffer,GameObject targetObject)
    {
        var keypointPosition = new Vector3(
                          (float)msg.poses[kpid].position.x,
                          0f,
                          (float)msg.poses[kpid].position.z
                );

        var coef = (float)msg.poses[kpid].orientation.x;

        bool isTrackingBody = false;

        if(coef > coefThreshold && minRange < keypointPosition.z && keypointPosition.z < maxRange)
        {
            //Debug.Log("Level:2");
            isTrackingBody = true;
            for(int i = bufferSize - 1; i > 0; i --)
            {
                keyPointsBuffer[i] = keyPointsBuffer[i - 1];
            }

            keyPointsBuffer[0] = keypointPosition;

            Vector3 avgKeypointPosition = Vector3.zero;

            for(int i = 0; i < bufferSize; i ++)
            {
                avgKeypointPosition += keyPointsBuffer[i];
            }

            avgKeypointPosition = avgKeypointPosition / bufferSize;

            avgKeypointPosition.z += offsetDepth;

            targetObject.transform.localPosition = -avgKeypointPosition;
        }

        return isTrackingBody;
    }
    
}
