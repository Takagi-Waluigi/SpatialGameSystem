using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCountScore : MonoBehaviour
{
    GameObject stateManagerObject = null;     
    bool isHit = false; //True:未取得状態 False:取得済み状態
    // Start is called before the first frame update
    void Start()
    {
        stateManagerObject = GameObject.Find("GameStateManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(stateManagerObject.GetComponent<StateManager>().isGameOver)
        {
            isHit = false;
        }
        
        this.gameObject.GetComponent<Renderer>().enabled = !isHit;
        this.gameObject.GetComponent<Collider>().enabled = !isHit;
    }

    void OnCollisionEnter(Collision collision)
    {
       // Debug.Log("Collision:" + collision.gameObject.name);
       if(stateManagerObject != null && collision.gameObject.name == "Character" 
       && stateManagerObject.GetComponent<StateManager>().isTrackingUser 
       && !stateManagerObject.GetComponent<StateManager>().isGameOver)
       {
            //点数の加算
            stateManagerObject.GetComponent<StateManager>().score ++;
            
            //コイン取得済みフラグをTrueに
            isHit = true;
       }
        
               
    }
}
