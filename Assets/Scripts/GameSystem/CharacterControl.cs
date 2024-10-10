using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class CharacterControl : MonoBehaviour
{
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
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<NavMeshAgent>().enabled = !this.GetComponent<StateManager>().isAttacked;
        
        if(this.GetComponent<NavMeshAgent>().enabled) PlayTime(); else GameOver();        
    }

    void PlayTime()
    {
        //どちらかのオブジェクトでトラッキングされている場合にアクティブに
        isTracking = (laserObject_1.objectWorldPositions.Count > 0 || laserObject_2.objectWorldPositions.Count > 0);

        //StateManagerに伝達
        this.GetComponent<StateManager>().isTrackingUser = isTracking;

        //急いでユーザのもとに駆け寄ってきてもらう
        if(!lastFrameIsTracking && isTracking) agent.speed = 10f;
        

        if(isTracking)  //トラッキングがアクティブの時の処理
        {
            //見た目の変更
            GetComponent<MeshRenderer>().material = activeMaterial;

            int minId = 0;
            float minDistance = 1000000f;

            for(int i = 0; i < laserObject_1.objectWorldPositions.Count; i ++)
            {
                float distanceToObject = Vector3.Distance(
                    this.transform.position, 
                    laserObject_1.objectWorldPositions[i]);

                if(distanceToObject < minDistance)
                {
                    minDistance = distanceToObject;
                    minId = i;
                }
            }

            if(agent.remainingDistance < 1.0f)
            {
                agent.speed = 1f;
            }

            destinationPosition = laserObject_1.objectWorldPositions[minId];
        }
        else //トラッキングが非アクティブの時の処理
        {
            //見た目の変更
            GetComponent<MeshRenderer>().material = deactiveMaterial;

            Transform baseTransform = screen_1; //要変更！！！！！

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
