
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] StateManager stateManager;
    [Header("各種設定")]      
    [SerializeField] [Range(0, 5)] int cardId;
    public bool isDone = false;

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

        if(!stateManager.isGameOver)
        {

            if(isDone) isLocked = true;
        }
        else
        {
            isDone = false;
        }

        if(Input.GetKeyUp(KeyCode.F)) userStepOn = !userStepOn;

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
                if(cardId == stateManager.targetCardId)
                {
                    Debug.Log("Collect!!!!");
                    stateManager.score ++;

                    for(int i = 0; i < stateManager.unmatchedId.Count; i ++)
                    {
                        if(stateManager.unmatchedId[i] == cardId)
                        {
                            stateManager.unmatchedId.RemoveAt(i);
                        }
                    }
                    
                    isDone = true;
                }
                else
                {
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
        if(!isLocked && collision.gameObject.name == "User" && stateManager.isTrackingUser && !stateManager.isGameOver)
        {
            enableTime += Time.deltaTime;
            if(enableTime > stateManager.stepOnThresholdTime) userStepOn = true;  
        }       
               
    }
}
