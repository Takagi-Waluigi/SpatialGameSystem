using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionProcess : MonoBehaviour
{
    GameObject characterObject;
    bool isActive = true; //True:未取得状態 False:取得済み状態
    // Start is called before the first frame update
    void Start()
    {
        characterObject = GameObject.Find("Character");
        GetComponent<MeshRenderer>().material = characterObject.GetComponent<StateManager>().unscoredMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        //キャラクターに当たったら削除
        if(isActive)
        {
            if(collision.gameObject.name == characterObject.name)
            {
                GetComponent<MeshRenderer>().material = characterObject.GetComponent<StateManager>().scoredMaterial;
                characterObject.GetComponent<StateManager>().count ++;

                isActive = false;
            }
        }
        
               
    }
}
