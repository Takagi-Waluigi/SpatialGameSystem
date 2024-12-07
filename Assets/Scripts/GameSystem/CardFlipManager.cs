using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlipManager : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    int lastScore = 0;
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

                    if(stateManager.unmatchedId.Count > 0)
                    {
                        if(stateManager.matchStatus == 2)
                        {
                            int nextIndex = (stateManager.targetCardId + (int)Random.Range(0, stateManager.unmatchedId.Count)) % stateManager.unmatchedId.Count;
                            stateManager.targetCardId = stateManager.unmatchedId[nextIndex];
                        }
                        
                    }
                    else
                    {
                        stateManager.isGameOver = true;
                    }
                                
                }
            }
            else
            {
                stateManager.matchStatus = 0;
                stateManager.flipBackTime = 0f;
                stateManager.enableFlipBack = false;
            }
        }
    }
}
