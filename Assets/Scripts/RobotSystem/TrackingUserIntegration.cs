using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingUserIntegration : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] Transform baseTransform;
    [SerializeField] SinglePoseSubscriber p1ObjectSubscriber;
    [SerializeField] SinglePoseSubscriber p2ObjectSubscriber;
    [SerializeField] float distanceThreshold = 1f;
    [SerializeField] StateManager stateManager;
    public Vector3 integratedPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stateManager.isTrackingUser = (!p1ObjectSubscriber.isNotIncomingData || !p2ObjectSubscriber.isNotIncomingData);
        
        
        if(!p1ObjectSubscriber.isNotIncomingData && !p2ObjectSubscriber.isNotIncomingData)
        {    
            integratedPosition = (p1ObjectSubscriber.centerPosition + p2ObjectSubscriber.centerPosition) * 0.5f;
        }
        else if(!p1ObjectSubscriber.isNotIncomingData)
        {
            integratedPosition = p1ObjectSubscriber.centerPosition;
        }
        else if(!p2ObjectSubscriber.isNotIncomingData)
        {
            integratedPosition = p2ObjectSubscriber.centerPosition;

        }
        
    }
}