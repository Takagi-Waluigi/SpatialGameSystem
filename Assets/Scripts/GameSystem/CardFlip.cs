using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] StateManager stateManager;
    [Header("各種設定")]      
    [SerializeField] [Range(0, 5)] int cardId;
    float enableTime = 0f;
    bool userStepOn = false;
    bool isFlipped, lastIsFlipped;
    bool isLocked = false;
    bool lastIsGameOver = false;
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
            if(stateManager.matchedId.Count > 0)
            {
                foreach(int id in stateManager.matchedId)
                {
                    if(id == cardId)
                    {
                        isLocked = true;
                        break;
                    }
                }
            }
        }

        
        if(!isLocked)
        {
            if(stateManager.enableFlipBack || stateManager.isGameOver) FlipBack();

            Flip();

            if(isFlipped && !lastIsFlipped)
            {
                if(!stateManager.isFlippingFirst)
                {
                    stateManager.isFlippingFirst = true;
                    stateManager.firstCardId = cardId;
                }            
                else
                {
                    stateManager.isFlippingSecond = true;

                    if(stateManager.firstCardId == cardId)
                    {
                        stateManager.isMatching = true;
                        stateManager.score ++;
                    }
                    else
                    {
                        stateManager.isMatching = false;
                        stateManager.flipBackTime = 0f;
                    }
                }          
            }

            lastIsFlipped = isFlipped;
        }

        lastIsGameOver = stateManager.isGameOver;   
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
