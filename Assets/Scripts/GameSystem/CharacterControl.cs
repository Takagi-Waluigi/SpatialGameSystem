using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class CharacterControl : MonoBehaviour
{
    [Header("ゲーム管理オブジェクトの登録")]
    [SerializeField] StateManager gameStateManager;
    [Header("トラッキング情報")]
    [SerializeField] LaserObjectSubscriber laserObject_1;
    [SerializeField] LaserObjectSubscriber laserObject_2;

    [Header("スクリーン位置")]
    [SerializeField] Transform screen_1;
    [SerializeField] Transform screen_2;
    [Header("マテリアル設定")]
    [SerializeField] Material activeMaterial;
    [SerializeField] Material deactiveMaterial;

    [Header("非トラッキング状態の設定")]
    [SerializeField] float maxRadius = 1.0f;

    [Header("NavMeshAgentの代入")]
    [SerializeField] NavMeshAgent agent;
    Vector3 destinationPosition = new Vector3();
    bool isTracking = false;
    bool lastFrameIsTracking = false;    

    // Start is called before the first frame update
    void Start()
    {
        destinationPosition = screen_1.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<NavMeshAgent>().enabled = !gameStateManager.isAttacked;
        if(this.GetComponent<NavMeshAgent>().enabled) PlayTime(); else GameOver();        
    }

    void PlayTime()
    {
        //どちらかのオブジェクトでトラッキングされている場合にアクティブに
        isTracking = (laserObject_1.objectWorldPositions.Count > 0 || laserObject_2.objectWorldPositions.Count > 0);

        //StateManagerに伝達
        gameStateManager.isTrackingUser = isTracking;

        // 急いでユーザのもとに駆け寄ってきてもらう
       // if(!lastFrameIsTracking && isTracking) agent.speed = 10f;
        

        if(isTracking)  //トラッキングがアクティブの時の処理
        {
            //見た目の変更
            GetComponent<MeshRenderer>().material = activeMaterial;

            int minId = 0;
            float minDistance = 1000000f;

            if(laserObject_2.objectWorldPositions.Count > 0)
            {
                for(int i = 0; i < laserObject_2.objectWorldPositions.Count; i ++)
                {
                    float distanceToObject = Vector3.Distance(
                        this.transform.position, 
                        laserObject_2.objectWorldPositions[i]);

                    if(distanceToObject < minDistance)
                    {
                        minDistance = distanceToObject;
                        minId = i;
                    }
                }

                destinationPosition = laserObject_2.objectWorldPositions[minId];
                destinationPosition.y = 0f;

            }

            

            //Debug.Log("destination:" + destinationPosition);
        }
        else //トラッキングが非アクティブの時の処理
        {
            //見た目の変更
            GetComponent<MeshRenderer>().material = deactiveMaterial;

            Transform baseTransform = screen_2; //要変更！！！！！

            if(agent.remainingDistance < 0.01f)
            {
                float randomRadius = UnityEngine.Random.Range(0, maxRadius);
                float randomAngular = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);

                destinationPosition = new Vector3(
                    baseTransform.position.x + randomRadius * Mathf.Cos(randomAngular),
                    0f,
                    baseTransform.position.z + randomRadius * Mathf.Sin(randomAngular)
                );
            }
        }

        agent.SetDestination(destinationPosition);

        lastFrameIsTracking = isTracking;
    }

    void GameOver()
    {

    }
}
