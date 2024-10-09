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
        //Insatantiateで管理不可のためGameObjectの名前で指定
        characterObject = GameObject.Find("Character");
        GetComponent<MeshRenderer>().material = characterObject.GetComponent<StateManager>().unscoredMaterial;
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
            if(collision.gameObject.name == characterObject.name)
            {
                //キャラクターオブジェクトと衝突した場合キャラクターオブジェクト内のフラグをTrueに
                GetComponent<MeshRenderer>().material = characterObject.GetComponent<StateManager>().scoredMaterial;
                
                //キャラクターオブジェクトと衝突した回数を加算
                characterObject.GetComponent<StateManager>().count ++;

                //踏まれた判定にして加算を1回に制限する
                isActive = false;
            }
        }
        
               
    }
}
