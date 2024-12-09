using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlipManager : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    float lastChangeCardTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {  
        if(stateManager.isGameOver)
        {
            if(Input.GetKeyUp(KeyCode.M)) 
            {
                stateManager.isMemoryPhase = !stateManager.isMemoryPhase;
                string phaseText = (stateManager.isMemoryPhase)? "[MEMORY]" : "[RECALL]";
                Debug.Log(phaseText + ": phase has been changed!!!!");
            }
        }
        if(!stateManager.isMemoryPhase)        
        {
            stateManager.memoryTime = 0;
            if(stateManager.isAnswered)
            {
                stateManager.flipBackTime += Time.deltaTime;

                if(stateManager.flipBackTime > stateManager.maxWaitTime)
                {
                    stateManager.enableFlipBack = true;
                    stateManager.isAnswered = false;

                    stateManager.targetCardId = (stateManager.targetCardId + (int)Random.Range(1, stateManager.numPattern)) % stateManager.numPattern;
                    lastChangeCardTime = Time.time;  
                }
            }
            else
            {
                if(Time.time - lastChangeCardTime > stateManager.timeOut)
                {
                    Debug.Log("Time out!!!");
                    stateManager.targetCardId = (stateManager.targetCardId + (int)Random.Range(1, stateManager.numPattern)) % stateManager.numPattern;
                    lastChangeCardTime = Time.time;
                } 

                stateManager.matchStatus = 0;
                stateManager.flipBackTime = 0f;
                stateManager.enableFlipBack = false;
            }
        }
    }
}
