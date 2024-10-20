using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics;
using RosMessageTypes.Geometry;
using RosMessageTypes.Nav;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;

public class LaserObjectSubscriber : MonoBehaviour
{
    ROSConnection ros;

    [Header("基本設定")]
    [SerializeField] Transform baseTransform;
    [SerializeField] string topicName = "";  
    [SerializeField] string rosNamespace = "";  
    [SerializeField] [Range(0f, 5f)] float maxRange = 3.0f;
    [SerializeField] [Range(0f, 5f)] float minRange = 0.250f;
    [SerializeField] [Range(0f, 5f)] float maxDifferentialThresold = 3.0f;
    [SerializeField] float distanceCutOffPerFrame = 0.2f;
    [SerializeField] float offsetDistance = 0.39f;
    List<Vector3> objectPositions = new List<Vector3>();
    List<Vector3> smoothedObjectPositions = new List<Vector3>();

    public List<Vector3> objectWorldPositions = new List<Vector3>();

    [Header("視覚化用パラメータ")]
    [SerializeField] bool visualize;
    [SerializeField] GameObject baseObject;
    [SerializeField] int numberOfThresoldObject = 15;
    GameObject[] thresholdObjets;

    List<Vector3> appliedPositions = new List<Vector3>();
    List<Vector3> thresholdPositions = new List<Vector3>();

    const int BUFFER_SIZE = 10;
    const int DATA_WIDTH = 15;
    int lastFrameObjectCount = 0;
    bool dataIncoming = false;
    Vector3[,] depthBuffer = new Vector3[BUFFER_SIZE, DATA_WIDTH];
    
    Vector3[] averagePositionBuffer = new Vector3[DATA_WIDTH];

    
    // Start is called before the first frame update
    void Start()
    {
        topicName = "/" + topicName;

        if(rosNamespace != "") topicName = "/" + rosNamespace + topicName;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseArrayMsg>(topicName, OnSubsribeArray);
        thresholdObjets = new GameObject[numberOfThresoldObject];

        if(visualize)
        {
            for(int i = 0; i < thresholdObjets.Length; i ++)
            {
                thresholdObjets[i] = GameObject.Instantiate(baseObject, baseTransform);
                thresholdObjets[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                thresholdObjets[i].name = "laserObject_" + i;
            }
        }
    }

    void ResetBufferByCurrentData()
    {
        if(objectPositions.Count > 0)
        {
            //すべてのバッファを最新のデータに書き換える
            for(int i = 0; i < BUFFER_SIZE; i ++)
            {
                for(int j = 0; j < DATA_WIDTH; j ++)
                {
                    //depthBuffer[i, j] = objectPositions[j];
                }
            }
        }
        else
        {
            Debug.Log("Data number is not enough");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        lastFrameObjectCount = objectPositions.Count;

        thresholdPositions.Clear();
        objectPositions.Clear();
        smoothedObjectPositions.Clear();
        objectWorldPositions.Clear();

        if(appliedPositions.Count > 0)
        {
            thresholdPositions.Add(appliedPositions[0]);
            
            for(int i = 0; i < appliedPositions.Count; i ++)
            {
                if(i - 1 > 0)
                {
                    float distanceDiffPrevious = Vector3.Distance(appliedPositions[i - 1], appliedPositions[i]);

                    if(distanceDiffPrevious > maxDifferentialThresold)
                    {
                        thresholdPositions.Add(appliedPositions[i]);                    
                    }
                }
                
                if(i + 1 < appliedPositions.Count)
                {
                    float distanceDiffNext = Vector3.Distance(appliedPositions[i], appliedPositions[i + 1]);

                    if(distanceDiffNext > maxDifferentialThresold)
                    {
                        thresholdPositions.Add(appliedPositions[i]);                    
                    }
                }
                
            }

            thresholdPositions.Add(appliedPositions[appliedPositions.Count - 1]);


            if(thresholdPositions.Count > 1)
            {
                for(int i = 0; i < thresholdPositions.Count - 1; i += 2)
                {
                    Vector3 p1 = thresholdPositions[i];
                    Vector3 p2 = thresholdPositions[i + 1];

                    Vector3 centerPoint = (p1 + p2) * 0.5f;

                    objectPositions.Add(centerPoint);
                }
            }

            if(lastFrameObjectCount != objectPositions.Count) ResetBufferByCurrentData();

            if(dataIncoming)
            {
                for(int i = BUFFER_SIZE - 1; i > -1; i --)
                {
                    for(int j = 0; j < DATA_WIDTH; j ++)
                    {
                        if(i > 0)
                        {
                            //i番目のバッファにi-1番目のバッファを代入して更新
                            depthBuffer[i, j] = depthBuffer[i - 1, j];
                        }
                        else
                        {   
                            if(j < objectPositions.Count)
                            {
                                //0番目を最新のデータとする
                                depthBuffer[i, j] = objectPositions[j];
                            }
                            else
                            {
                                //横のデータが固定長の長さよりも小さい場合は原点を代入
                                depthBuffer[i, j] = Vector3.zero;
                            }
                        }                    
                    }
                }
                
                if(lastFrameObjectCount == objectPositions.Count)
                {
                    for(int i = 0; i < DATA_WIDTH; i ++)
                    {   
                        Vector3 averagePosition = Vector3.zero;
                        for(int j = 0; j < BUFFER_SIZE; j ++)
                        {
                            averagePosition += depthBuffer[j, i];
                        }

                        averagePosition = averagePosition / (float)BUFFER_SIZE;

                        
                        if(Vector3.Distance(averagePosition, averagePositionBuffer[i]) < distanceCutOffPerFrame 
                            && averagePosition != Vector3.zero) smoothedObjectPositions.Add(averagePosition);

                        averagePositionBuffer[i] = averagePosition;
                    }
                }

                for(int i = 0; i < smoothedObjectPositions.Count; i ++)
                {
                    smoothedObjectPositions[i] = new Vector3(
                            -smoothedObjectPositions[i].x,
                             smoothedObjectPositions[i].y,
                            -smoothedObjectPositions[i].z - offsetDistance); 
                }
            }

            for(int i = 0; i < thresholdObjets.Length; i ++)
            {
                if(i < smoothedObjectPositions.Count)
                {
                    thresholdObjets[i].transform.localPosition = smoothedObjectPositions[i]; 
                    thresholdObjets[i].transform.localScale = (visualize == true) ? new Vector3(0.07f, 0.07f, 0.07f) : Vector3.zero;

                    objectWorldPositions.Add(thresholdObjets[i].transform.position);
                }
                else
                {
                    thresholdObjets[i].transform.localPosition = Vector3.zero;
                    thresholdObjets[i].transform.localScale = Vector3.zero;                    
                }
            }
        }  

        dataIncoming = true;  
    }

    void OnSubsribeArray(PoseArrayMsg msg)
    {
        appliedPositions.Clear();
        for(int i = 0; i < msg.poses.Length; i ++)
        {
            if(minRange < (float)msg.poses[i].position.z && (float)msg.poses[i].position.z < maxRange)
            {
                var appliedPosition = new Vector3(
                (float)msg.poses[i].position.x,
                -(float)msg.poses[i].position.y,
                (float)msg.poses[i].position.z);

                appliedPositions.Add(appliedPosition);
            }
        } 

        dataIncoming = true; 
    }
}
