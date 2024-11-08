using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionRespawn : MonoBehaviour
{
    GameObject stateManagerObject = null;
    float decisionTime = 0;
    float maxTime = 5f;
    // Start is called before the first frame update
    void Start()
    {
        stateManagerObject = GameObject.Find("GameStateManager");
    }

    void OnCollisionStay(Collision collision)
    {

       if(stateManagerObject != null && collision.gameObject.name == "User" && stateManagerObject.GetComponent<StateManager>().isTrackingUser)
       {
            //値の初期化
            stateManagerObject.GetComponent<StateManager>().decisionTime += Time.deltaTime;
       }       
               
    }
}
