using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class SelectAndNavigation : MonoBehaviour
{
    [SerializeField] LaserObjectSubscriber laserObject_1;
    // [SerializeField] LaserObjectSubscriber laserObject_2;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform[] freeMoveGoals;
    Vector3 destinationPosition = new Vector3();
    bool isTracking = false;
    bool lastFrameIsTracking = false;    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isTracking = laserObject_1.objectWorldPositions.Count > 0;

        if(!lastFrameIsTracking && isTracking)
        {
            agent.speed = 10f;
        }

        if(isTracking)
        {
           // Debug.Log("User has been detected!!!");
            int minId = 0;
            float minDistance = 1000000f;

            for(int i = 0; i < laserObject_1.objectWorldPositions.Count; i ++)
            {
                float distanceToObject = Vector3.Distance(
                    this.transform.position, 
                    laserObject_1.objectWorldPositions[i]);

                if(distanceToObject < minDistance)
                {
                    minDistance = distanceToObject;
                    minId = i;
                }
            }

            if(agent.remainingDistance < 1.0f)
            {
                agent.speed = 1f;
            }

            destinationPosition = laserObject_1.objectWorldPositions[minId];
        }
        else
        {
           // Debug.Log("User has not been detected!!!");
            
            if(freeMoveGoals.Length > 0)
            {
                if(agent.remainingDistance < 0.1f)
                {
                    int randomId = UnityEngine.Random.Range(0, freeMoveGoals.Length);
                    destinationPosition = freeMoveGoals[randomId].transform.position;
                }                
            }
        }

        agent.SetDestination(destinationPosition);

        lastFrameIsTracking = isTracking;
        
        
    }
}
