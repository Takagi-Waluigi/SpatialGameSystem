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
        if(stateManager.isFlippingFirst && stateManager.isFlippingSecond)
        {
            stateManager.flipBackTime += Time.deltaTime;

            if(stateManager.flipBackTime > stateManager.maxWaitTime)
            {
                stateManager.isFlippingFirst = false;
                stateManager.isFlippingSecond = false;

                if(!stateManager.isMatching)
                {
                    stateManager.enableFlipBack = true;                    
                }
                else
                {
                    stateManager.matchedId.Add(stateManager.firstCardId);
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
