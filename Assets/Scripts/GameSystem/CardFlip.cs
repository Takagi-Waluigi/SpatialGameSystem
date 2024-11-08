using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    [SerializeField] float stepOnThresholdTime = 3f;
    [SerializeField] float stepOffThresholdTime = 6f;
    float enableTime = 0f;
    float disableTime = 0f;
    bool _userStepOn = false;
    bool userStepOn = false;
    bool lastUserStepOn = false;
    GameObject userObject;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!_userStepOn)
        {
            disableTime += Time.deltaTime;
            if(disableTime > stepOffThresholdTime) userStepOn = false; 
        }
       // Debug.Log("enableTime:" + enableTime + "/disableTime:" + disableTime);

        if(lastUserStepOn != userStepOn)
        {
            disableTime = 0f;
            enableTime = 0f;        
        }

        Debug.Log("disable time:" + disableTime + "/enable time:" + enableTime);
        Debug.Log("User StepOn Status:" + userStepOn);

        _userStepOn = false;

        lastUserStepOn = userStepOn;


        Vector3 eularAngles = this.transform.rotation.eulerAngles;
        if(userStepOn)
        {           
            eularAngles.z += 10f;

            if(eularAngles.z > 180f) eularAngles.z = 180f;            
        }
        else
        {
            eularAngles.z += 10f;

            if(eularAngles.z < 180f) eularAngles.z = 0f;

        }

        this.transform.rotation = Quaternion.Euler(eularAngles); 
    }

    void OnCollisionStay(Collision collision)
    {

       if(collision.gameObject.name == "User" && stateManager.isTrackingUser)
       {
           _userStepOn = true;  

           enableTime += Time.deltaTime;
           if(enableTime > stepOnThresholdTime) userStepOn = true;  
       }       
               
    }
}
