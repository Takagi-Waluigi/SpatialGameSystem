using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

using RosMessageTypes.Nav;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

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

public class PoseSubscriber : MonoBehaviour
{
    ROSConnection ros;

    [SerializeField] string rosNamespace = "";
    [SerializeField] string arrayTopicName;
    [SerializeField] [Range(0, 1)] float coefThreshold;
    [SerializeField] bool localModePositonMode = false;
    //[SerializeField] Vector3 offsetPosition;
    [SerializeField] Transform rootTransform;
    [SerializeField] [Range(0.0f, 1.0f)] float baseMarkerSize;
    [SerializeField] [Range(1, 5)] int userID;
   // [SerializeField] [Range(0, 0.1f)] float disableThreshold;
    [SerializeField] Color baseColor;
    [SerializeField] bool enableRender = true;

    const int positionBufferSize = 10;
    const int incomingBufferSize = 100;
    float dataIncomingRate = 0f;
    [SerializeField] [Range(0f, 1f)] float dataIncomingRateThreshold = 0.1f;
    bool[] dataIncomingFlags = new bool[incomingBufferSize];
    bool isIncomingCurrentData = false;
    int isNotIncomingDataNum = 0;
    [SerializeField] int disableThresholdFrameNumber = 20;

    string[] KEYPOINTS_NAMES = {
    "nose",             // 0
    "eye(L)",           // 1
    "eye(R)",           // 2
    "ear(L)",           // 3
    "ear(R)",           // 4
    "shoulder(L)",      // 5
    "shoulder(R)",      // 6
    "elbow(L)",         // 7
    "elbow(R)",         // 8
    "wrist(L)",         // 9
    "wrist(R)",         // 10
    "hip(L)",           // 11
    "hip(R)",           // 12
    "knee(L)",          // 13
    "knee(R)",          // 14
    "ankle(L)",         // 15
    "ankle(R)"         // 16
    };

    List<GameObject> keyPointObjects = new List<GameObject>();
    List<List<Vector3>> keyPointsBuffer = new List<List<Vector3>>();
    List<int> keyPointsDisableCount= new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        arrayTopicName = "/" + arrayTopicName;

        if (rosNamespace != "")
        {
            arrayTopicName = "/" + rosNamespace + arrayTopicName;
        }

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseArrayMsg>(arrayTopicName, OnSubsribeArray);

        for(int i = 0; i < KEYPOINTS_NAMES.Length; i ++)
        {   
            GameObject insertGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            insertGameObject.name = userID.ToString() + "-" + KEYPOINTS_NAMES[i];
            insertGameObject.transform.SetParent(rootTransform);
            insertGameObject.transform.localScale = new Vector3(baseMarkerSize, baseMarkerSize, baseMarkerSize);
            insertGameObject.SetActive(false);
            insertGameObject.GetComponent<Renderer>().material.color = baseColor;
            insertGameObject.GetComponent<Renderer>().enabled = enableRender;
            insertGameObject.AddComponent<Rigidbody>();
            insertGameObject.GetComponent<Rigidbody>().isKinematic = true;

            keyPointObjects.Add(insertGameObject);

            List<Vector3> kpBuf = new List<Vector3>();
            for(int k = 0; k < positionBufferSize; k ++)
            {
                kpBuf.Add(insertGameObject.transform.position);
            }

            keyPointsBuffer.Add(kpBuf);

            keyPointsDisableCount.Add(0);

        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject kpo in keyPointObjects) 
        {
            kpo.GetComponent<Renderer>().enabled = enableRender;
            kpo.transform.localScale = new Vector3(baseMarkerSize, baseMarkerSize, baseMarkerSize);
        }
        
        float sum = 0;
        
        for(int i = incomingBufferSize - 1; i > 0; i --)
        {
            dataIncomingFlags[i] = dataIncomingFlags[i-1];
        }

        dataIncomingFlags[0] = isIncomingCurrentData;

        for(int i = 0; i < incomingBufferSize; i ++)
        {
            sum += (dataIncomingFlags[i])? 1: 0;
        }
                
        dataIncomingRate = sum / incomingBufferSize;

        isIncomingCurrentData = false;


         if(dataIncomingRate < dataIncomingRateThreshold)
         {
            foreach(GameObject kpo in keyPointObjects)
            {
                kpo.SetActive(false);
            }   
         }

        
    }

    void OnSubsribeArray(PoseArrayMsg msg)
     {
       
        //対象のIDがあれば割り当てる
        if (msg.header.frame_id == userID.ToString())
        {
            //対象のユーザのキーポイントを取得する
            for(int i = 0; i < KEYPOINTS_NAMES.Length; i ++)
            {                
                var coef = (float)msg.poses[i].orientation.x;

                //データの推定値が一定値を超えていれば取得している判定とする
                if(coef > coefThreshold)
                {
                    isIncomingCurrentData = true;
                    Debug.Log("Useful data is incoming!!!");

                    //取得したうえでデータの取得レートが一定値を超えていればアクティブにする
                    if(dataIncomingRate > dataIncomingRateThreshold)
                    {

                        keyPointObjects[i].SetActive(true);

                        var keypointPosition = new Vector3(
                            -(float)msg.poses[i].position.x,
                            (float)msg.poses[i].position.y,
                            -(float)msg.poses[i].position.z
                        ); 

                            if(msg.header.frame_id == "3") Debug.Log(keypointPosition);


                        //平均化バッファの更新処理
                        for(int k = positionBufferSize - 1; k > 0; k --)
                        {
                            keyPointsBuffer[i][k] = keyPointsBuffer[i][k - 1];
                        }

                        keyPointsBuffer[i][0] = keypointPosition;

                        if(Vector3.Distance(keypointPosition, keyPointsBuffer[i][1]) < 0.05f)
                        {
                            keyPointsDisableCount[i] ++;
                        }

                        Vector3 avgKeypointPosition = Vector3.zero;
                        
                        for(int k = 0; k < positionBufferSize; k ++)
                        {
                            avgKeypointPosition += keyPointsBuffer[i][k];
                        }

                        avgKeypointPosition = avgKeypointPosition / positionBufferSize;

                        if (localModePositonMode)
                        {
                            keyPointObjects[i].transform.localPosition = avgKeypointPosition;
                        }
                        else
                        {
                                keyPointObjects[i].transform.position = avgKeypointPosition;
                        }
                    }                    
                }
                else
                {
                    keyPointObjects[i].SetActive(false);
                }

            }
        }       
                
    }
}
