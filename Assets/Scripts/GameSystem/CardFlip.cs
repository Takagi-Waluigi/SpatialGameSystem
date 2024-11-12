using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] StateManager stateManager;
    [Header("各種設定")]
    [SerializeField] float stepOnThresholdTime = 3f;       
    [SerializeField] [Range(0, 5)] int cardId;
    float enableTime = 0f;
    bool userStepOn = false;
    bool isFlipped, lastIsFlipped;
    bool isLocked = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        isLocked = false;

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
        
        if(!isLocked)
        {
            if(stateManager.enableFlipBack) FlipBack();

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
        if(!isLocked && collision.gameObject.name == "User" && stateManager.isTrackingUser)
        {
            enableTime += Time.deltaTime;
            if(enableTime > stepOnThresholdTime) userStepOn = true;  
        }       
               
    }
}
