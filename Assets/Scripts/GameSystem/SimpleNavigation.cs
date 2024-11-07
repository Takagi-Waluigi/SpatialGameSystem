using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNavigation : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform targetTransform;
      

    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {

        agent.SetDestination(targetTransform.position);
    }
}
