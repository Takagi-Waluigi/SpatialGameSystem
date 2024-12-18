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
       var stateManager = stateManagerObject.GetComponent<StateManager>();

       if(collision.gameObject.name == "Character" 
       && stateManager.isTrackingUser 
       && !stateManager.isGameOver 
       //&& stateManager.isVisibleCharacter
       )
       {
            //点数の加算
            int incrementValue = (stateManager.enableFever)? 5 : 1;
            stateManagerObject.GetComponent<StateManager>().score += incrementValue;
            
            //コイン取得済みフラグをTrueに
            isHit = true;
       }
    }
}
