using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlipManager : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    [SerializeField] int numAnimals = 3;
    [SerializeField] int numCardsPerAnimals = 4;

     int numMaxMatchedId;

    // Start is called before the first frame update
    void Start()
    {
        numMaxMatchedId =(int)(numAnimals * numCardsPerAnimals * 0.5f);

        Debug.Log("num matched id:" + numMaxMatchedId);
    }

    // Update is called once per frame
    void Update()
    {       
        Debug.Log(stateManager.matchedId.Count);

        if(stateManager.matchedId.Count >= numMaxMatchedId) stateManager.isGameOver = true;

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
                    stateManager.firstCard.GetComponent<CardFlip>().isDone = true;
                    stateManager.secondCard.GetComponent<CardFlip>().isDone = true;
                }

                stateManager.firstCardId = -1;
                stateManager.secondCardId = -1;

                stateManager.firstCard = null;
                stateManager.secondCard = null;
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
