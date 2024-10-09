using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNavigation : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject enemyRootObject;
    [SerializeField] float freezingTime = 3f;
    bool isFreezing = false;
    [SerializeField] float freezingAreaMax = 0.5f;
    Vector3 basePosition;
    float beginTime = 0f;


    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(this.gameObject.GetComponent<StateManager>().isAttacked)
        {
            beginTime = Time.time;
            basePosition = this.transform.position;
            isFreezing = true;
        }

        this.gameObject.GetComponent<NavMeshAgent>().enabled = !isFreezing;

        if(isFreezing)
        {
            if(Time.time - beginTime > freezingTime) isFreezing = false;

            var freezingPosition = new Vector3(
                basePosition.x + Random.Range(-freezingAreaMax * 0.5f, freezingAreaMax * 0.5f),
                basePosition.y,
                basePosition.z + Random.Range(-freezingAreaMax * 0.5f, freezingAreaMax * 0.5f)
            );

            this.transform.position = freezingPosition;
        }
        else
        {            
            agent.SetDestination(targetTransform.position);
        }
    }
}
