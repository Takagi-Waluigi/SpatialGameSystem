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
        this.gameObject.GetComponent<Renderer>().enabled = !isHit;
        this.gameObject.GetComponent<Collider>().enabled = !isHit;
    }

    void OnCollisionEnter(Collision collision)
    {
       if(stateManagerObject != null && collision.gameObject.name == "Character")
       {
            //点数の加算
            stateManagerObject.GetComponent<StateManager>().score ++;
            
            //コイン取得済みフラグをTrueに
            isHit = true;
       }
        
               
    }
}
