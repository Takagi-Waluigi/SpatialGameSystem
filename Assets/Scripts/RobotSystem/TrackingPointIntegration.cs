using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingPointIntegration : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] Transform baseTransform;
    [SerializeField] LaserObjectSubscriber p1ObjectSubscriber;
    [SerializeField] LaserObjectSubscriber p2ObjectSubscriber;
    [SerializeField] float distanceThreshold = 1f;


    [Header("視覚化用パラメータ")]
    [SerializeField] bool visualize;
    [SerializeField] GameObject baseObject;
    [SerializeField] int numberOfThresoldObject = 15;
    [Header("トラッキングされた位置")]
    GameObject[] thresholdObjets;
    
    public List<Vector3> integratedPositions = new List<Vector3>();
    
    // Start is called before the first frame update
    void Start()
    {
        thresholdObjets = new GameObject[numberOfThresoldObject];

        if(visualize)
        {
            for(int i = 0; i < thresholdObjets.Length; i ++)
            {
                thresholdObjets[i] = GameObject.Instantiate(baseObject, baseTransform);
                thresholdObjets[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                thresholdObjets[i].name = "integratedPosition_" + i;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        integratedPositions.Clear();
        int p1Count = p1ObjectSubscriber.objectWorldPositions.Count;
        int p2Count = p2ObjectSubscriber.objectWorldPositions.Count;

        if(p1Count > 0 && p2Count > 0)
        {

            foreach(Vector3 p1pos in p1ObjectSubscriber.objectWorldPositions)integratedPositions.Add(p1pos);
            
            for(int i = 0; i < p2Count; i ++)
            {
                for(int j = 0; j < p1Count; j ++)
                {
                    float distance = Vector3.Distance(
                        p2ObjectSubscriber.objectWorldPositions[i],
                        p1ObjectSubscriber.objectWorldPositions[j]
                    );

                    if(distance > distanceThreshold) integratedPositions.Add(p2ObjectSubscriber.objectWorldPositions[i]);
                }
            }
        }
        else if(p1Count > 0)
        {
            foreach(Vector3 p1pos in p1ObjectSubscriber.objectWorldPositions)integratedPositions.Add(p1pos);
        }
        else if(p2Count > 0)
        {
            foreach(Vector3 p2pos in p2ObjectSubscriber.objectWorldPositions)integratedPositions.Add(p2pos);
        }


        for(int i = 0; i < thresholdObjets.Length; i ++)
            {
                if(i < integratedPositions.Count)
                {
                    thresholdObjets[i].transform.position = integratedPositions[i]; 
                    thresholdObjets[i].transform.localScale = (visualize == true) ? new Vector3(0.07f, 0.07f, 0.07f) : Vector3.zero;
                }
                else
                {
                    thresholdObjets[i].transform.position = Vector3.zero;
                    thresholdObjets[i].transform.localScale = Vector3.zero;                    
                }
            }
    }
}
