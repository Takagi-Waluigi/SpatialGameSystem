using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEnemy : MonoBehaviour
{
    GameObject stateManagerObject = null;  
    float lastHitTime = 0;
    float interval = 2f;
    // Start is called before the first frame update
    void Start()
    {
        stateManagerObject = GameObject.Find("GameStateManager");
    }

    // Update is called once per frame
    void Update()
    {
        var stateManager = stateManagerObject.GetComponent<StateManager>();
        this.GetComponent<MeshRenderer>().enabled = !stateManager.enableFever;
        this.GetComponent<BoxCollider>().enabled = !stateManager.enableFever;

    }

    void OnCollisionEnter(Collision collision)
    {
       var stateManager = stateManagerObject.GetComponent<StateManager>();

       if(collision.gameObject.name == "Character" 
       && stateManager.isTrackingUser 
       && !stateManager.isGameOver 
       //&& stateManager.isVisibleCharacter
       && Time.time - lastHitTime > interval)
       {
            //ヒットポイントの加算
            stateManagerObject.GetComponent<StateManager>().hitPoint ++;

            lastHitTime = Time.time;            
       }        
    }
}
