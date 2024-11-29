using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNavigation : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform targetTransform;
    [SerializeField] StateManager stateManager;
    [SerializeField] float defaultSpeed = 0.35f;
    [SerializeField] float higherSpeed = 0.6f;
      

    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        agent.speed = (stateManager.enableFever)? higherSpeed : defaultSpeed;
        agent.SetDestination(targetTransform.position);
    }
}
