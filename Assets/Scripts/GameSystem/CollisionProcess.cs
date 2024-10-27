using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionProcess : MonoBehaviour
{
    GameObject gameControlObject;
    GameObject characterObject1, characterObject2;
    bool isHit = false; //True:未取得状態 False:取得済み状態
    StateManager characterState;
    // Start is called before the first frame update
    void Start()
    {
        //Insatantiateで管理不可のためGameObjectの名前で指定
        gameControlObject = GameObject.Find("GameControl");
        GetComponent<MeshRenderer>().material = gameControlObject.GetComponent<StateManager>().deactiveMaterial;
        if(gameControlObject == null) Debug.Log("not inserted _1");

        characterObject1 = GameObject.Find("Character1");
        characterObject2 = GameObject.Find("Character2");

        if(characterObject1 == null || characterObject2 == null) Debug.Log("not inserted");
    }

    // Update is called once per frame
    void Update()
    {
        characterState = gameControlObject.GetComponent<StateManager>();
        if(characterState.isAttacked) isHit = false;

        this.gameObject.GetComponent<Renderer>().enabled = !isHit;
        this.gameObject.GetComponent<Collider>().enabled = !isHit;
    }

    void OnCollisionEnter(Collision collision)
    {
        // //まだ踏まれていなければ判定を有効に
        // if(isActive)
        // {
        //     characterState = gameControlObject.GetComponent<StateManager>();
        //     if(collision.gameObject.name == gameControlObject.name && characterState.isTrackingUser)
        //     {
        //         //キャラクターオブジェクトと衝突した場合キャラクターオブジェクト内のフラグをTrueに
        //         GetComponent<MeshRenderer>().material = characterState.activeMaterial;
                
        //         //キャラクターオブジェクトと衝突した回数を加算
        //         gameControlObject.GetComponent<StateManager>().count ++;

        //         //踏まれた判定にして加算を1回に制限する
        //         isActive = false;
        //     }
        // }
        characterState = gameControlObject.GetComponent<StateManager>();

       if(((collision.gameObject.name == characterObject1.name) && characterState.isTrackingUser_1) || 
            ((collision.gameObject.name == characterObject2.name) && characterState.isTrackingUser_2))
        {
            //GameObject.Destroy(this.gameObject);
            isHit = true;
        }
        
               
    }
}
