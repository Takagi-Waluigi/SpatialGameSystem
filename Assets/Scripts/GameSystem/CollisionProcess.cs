using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionProcess : MonoBehaviour
{
    GameObject gameControlObject;
    bool isActive = true; //True:未取得状態 False:取得済み状態
    StateManager characterState;
    // Start is called before the first frame update
    void Start()
    {
        //Insatantiateで管理不可のためGameObjectの名前で指定
        gameControlObject = GameObject.Find("GameControl");
        GetComponent<MeshRenderer>().material = gameControlObject.GetComponent<StateManager>().deactiveMaterial;
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
            characterState = gameControlObject.GetComponent<StateManager>();
            if(collision.gameObject.name == gameControlObject.name && characterState.isTrackingUser)
            {
                //キャラクターオブジェクトと衝突した場合キャラクターオブジェクト内のフラグをTrueに
                GetComponent<MeshRenderer>().material = characterState.activeMaterial;
                
                //キャラクターオブジェクトと衝突した回数を加算
                gameControlObject.GetComponent<StateManager>().count ++;

                //踏まれた判定にして加算を1回に制限する
                isActive = false;
            }
        }
        
               
    }
}
