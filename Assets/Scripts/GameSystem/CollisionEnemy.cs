using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEnemy : MonoBehaviour
{
    GameObject stateManagerObject = null;  
    // Start is called before the first frame update
    void Start()
    {
        stateManagerObject = GameObject.Find("GameStateManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
       var stateManager = stateManagerObject.GetComponent<StateManager>();

       if(collision.gameObject.name == "Character" && stateManager.isTrackingUser && !stateManager.isGameOver && stateManager.isVisibleCharacter)
       {
            //ヒットポイントの加算
            stateManagerObject.GetComponent<StateManager>().hitPoint ++;
            
       }        
    }
}
