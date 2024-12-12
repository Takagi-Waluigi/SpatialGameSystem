using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRobotFeverManagement : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    [SerializeField] Transform cameraTransformP1;
    [SerializeField] Transform cameraTransformP2;
    bool lastIsNear = false;
    bool lastIsFever = false;
    float beginTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(stateManager.userStudyID == 2)
        {
            if(stateManager.conditionID == 2)
            {
                stateManager.distanceBetweenSceens = Vector3.Distance(cameraTransformP1.position, cameraTransformP2.position);

                if(!stateManager.isGameOver)
                {
                    if(stateManager.distanceBetweenSceens < stateManager.feverDistanceThreshold)
                    {
                        stateManager.enableFeverTrigger = true;
                    }
                    else
                    {
                        stateManager.enableFeverTrigger = false;
                        if(stateManager.enableFever)
                        {
                            if(lastIsNear) beginTime = Time.time;
                            if(Time.time - beginTime > stateManager.feverTime)
                            {
                                Debug.Log("[DEACTIVATE FEVER]" + (Time.time - beginTime));
                                stateManager.enableFever = false;
                            } 
                        }
                    }           
                
                    lastIsNear = stateManager.distanceBetweenSceens < stateManager.feverDistanceThreshold; 
                }             
            }
            else
            {
                if(stateManager.enableFever)
                {
                    if(!lastIsFever) beginTime = Time.time;
                    if(Time.time - beginTime > stateManager.feverTime) stateManager.enableFever = false;
                }
                
                lastIsFever = stateManager.enableFever;
            }
        }
        
    }
}
