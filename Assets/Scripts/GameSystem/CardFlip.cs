using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] StateManager stateManager;
    [Header("各種設定")]      
    [SerializeField] [Range(0, 8)] int cardId;

    float enableTime = 0f;
    bool userStepOn = false;
    bool isFlipped, lastIsFlipped;
    bool isLocked = false;
    bool lastIsGameOver = false;
    bool lastIsMemoryPhase = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    void InitAllStatus()
    {
        userStepOn = false;
        isFlipped = false;
        isLocked = false;
    }

    // Update is called once per frame
    void Update()
    {

        isLocked = false;

        // if(!stateManager.isGameOver)
        // {

        //     if(isDone) isLocked = true;
        // }
        // else
        // {
        //     isDone = false;
        // }

        if(stateManager.isMemoryPhase)
        {
            userStepOn = true;
        }
        
        
        if(!isLocked)
        {
            if(stateManager.enableFlipBack || stateManager.isGameOver || (!stateManager.isMemoryPhase && lastIsMemoryPhase)) FlipBack();

            Flip();

            if(isFlipped && !lastIsFlipped && !stateManager.isMemoryPhase)
            {
                stateManager.isAnswered = true;
                stateManager.trialCount ++;
                if(cardId == stateManager.targetCardId)
                {
                    Debug.Log("Collect!!!!");
                    stateManager.score ++;
                    stateManager.matchStatus = 2;
                }
                else
                {
                    stateManager.matchStatus = 1;
                    stateManager.wrongCount ++;
                    Debug.Log("Wrong...");
                }        
            }

            lastIsFlipped = isFlipped;
        }

        lastIsGameOver = stateManager.isGameOver;
        lastIsMemoryPhase = stateManager.isMemoryPhase;   
    }

    void Flip()
    {
        Vector3 eularAngles = this.transform.rotation.eulerAngles;
        if(userStepOn)
        {            
            eularAngles.z += 10f;

            if(eularAngles.z > 180f) 
            {   
                isFlipped = true;
                eularAngles.z = 180f;  
            }
            else
            {
                isFlipped = false;
            }         
        }
        else
        {
            eularAngles.z += 10f;

            if(eularAngles.z < 180f) eularAngles.z = 0f;
        }

        this.transform.rotation = Quaternion.Euler(eularAngles); 
    }

    void FlipBack()
    {
        userStepOn = false;
        enableTime = 0f;
    }

    void OnCollisionStay(Collision collision)
    {
        //if(!isLocked && collision.gameObject.name == "User" && stateManager.isTrackingUser && !stateManager.isGameOver && stateManager.isVisibleCharacter)
        if(!isLocked && collision.gameObject.name == "User" && stateManager.isTrackingUser && !stateManager.isGameOver)
        {
            enableTime += Time.deltaTime;
            if(enableTime > stateManager.stepOnThresholdTime) userStepOn = true;  
        }       
               
    }
}
