using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionProcess : MonoBehaviour
{
    GameObject characterObject;
    // Start is called before the first frame update
    void Start()
    {
        characterObject = GameObject.Find("GameControl");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        //キャラクターに当たったら削除
        if(collision.gameObject.name == characterObject.name)
        {
           characterObject.GetComponent<StateManager>().isAttacked = true;
        }       
               
    }
}
