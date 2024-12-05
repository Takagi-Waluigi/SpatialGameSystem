using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlipManager : MonoBehaviour
{
    [SerializeField] StateManager stateManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {    
        if(stateManager.isMemoryPhase)
        {
            stateManager.memoryTime += Time.deltaTime;

            if(stateManager.memoryTime > stateManager.maxMemoryTime)
            {
                stateManager.isMemoryPhase = false;
            }
        }   
        else
        {
            stateManager.memoryTime = 0;
            if(stateManager.isAnswered)
            {
                stateManager.flipBackTime += Time.deltaTime;

                if(stateManager.flipBackTime > stateManager.maxWaitTime)
                {
                    stateManager.enableFlipBack = true;
                    stateManager.isAnswered = false;

                    if(stateManager.unmatchedId.Count > 0)
                    {
                        int nextIndex = (stateManager.targetCardId + 1) % stateManager.unmatchedId.Count; 
                        stateManager.targetCardId = stateManager.unmatchedId[nextIndex];
                    }
                    else
                    {
                        stateManager.isGameOver = true;
                    }
                                
                }
            }
            else
            {
                stateManager.isMatching = false;
                stateManager.flipBackTime = 0f;
                stateManager.enableFlipBack = false;
            }
        }
    }
}
