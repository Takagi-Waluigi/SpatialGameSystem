using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionProcess : MonoBehaviour
{
    GameObject characterObject1, characterObject2;
    GameObject stateManager;
    // Start is called before the first frame update
    void Start()
    {
        characterObject1 = GameObject.Find("Character1");
        characterObject2 = GameObject.Find("Character2");
        stateManager = GameObject.Find("GameControl");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        //キャラクターに当たったら削除
        if(((collision.gameObject.name == characterObject1.name) && stateManager.GetComponent<StateManager>().isTrackingUser_1) || 
            ((collision.gameObject.name == characterObject2.name) && stateManager.GetComponent<StateManager>().isTrackingUser_2))
        {
           stateManager.GetComponent<StateManager>().isAttacked = true;
        }       
               
    }
}
