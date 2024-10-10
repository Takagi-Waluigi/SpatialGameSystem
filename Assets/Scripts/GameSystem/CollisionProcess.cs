using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionProcess : MonoBehaviour
{
    GameObject characterObject;
    bool isActive = true; //True:未取得状態 False:取得済み状態
    StateManager characterState;
    // Start is called before the first frame update
    void Start()
    {
        //Insatantiateで管理不可のためGameObjectの名前で指定
        characterObject = GameObject.Find("Character");
        GetComponent<MeshRenderer>().material = characterObject.GetComponent<StateManager>().deactiveMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        //まだ踏まれていなければ判定を有効に
        if(isActive)
        {
            characterState = characterObject.GetComponent<StateManager>();
            if(collision.gameObject.name == characterObject.name && characterState.isTrackingUser)
            {
                //キャラクターオブジェクトと衝突した場合キャラクターオブジェクト内のフラグをTrueに
                GetComponent<MeshRenderer>().material = characterState.activeMaterial;
                
                //キャラクターオブジェクトと衝突した回数を加算
                characterObject.GetComponent<StateManager>().count ++;

                //踏まれた判定にして加算を1回に制限する
                isActive = false;
            }
        }
        
               
    }
}
