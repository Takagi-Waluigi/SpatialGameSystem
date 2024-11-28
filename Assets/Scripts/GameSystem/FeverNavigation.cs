using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FeverNavigation : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] StateManager stateManager;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform characterTransform;
    [SerializeField] float goalUpdateFrequency = 1f;
    [SerializeField] [Range(0f, 1f)] float minRadius = 0.2f;
    [SerializeField] [Range(0f, 2f)] float maxRadius = 0.2f;
    [SerializeField] float nearThreshold = 0.17f;
    float lastTime = 0;
    int goalId = 0;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   
        if(Time.time - lastTime > goalUpdateFrequency)
        {
            float randomTheta = Random.Range(-Mathf.PI, Mathf.PI);
            float randomRadius = Random.Range(minRadius, maxRadius);
            Vector3 destination = new Vector3(
               randomRadius * Mathf.Cos(randomTheta) + characterTransform.position.x,
               0,
               randomRadius * Mathf.Sin(randomTheta) + characterTransform.position.z
            );

            agent.SetDestination(destination);

            lastTime = Time.time;
        } 

        float distance = Vector3.Distance(this.gameObject.transform.position, characterTransform.position); 
        Debug.Log("Distance to Character:" + distance);

        if(distance < nearThreshold)
        {
            stateManager.enableFever = true;
        }

        if(stateManager.enableFever)
        {
            this.agent.enabled = false;
            this.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            this.agent.enabled = true;
            this.GetComponent<MeshRenderer>().enabled = true;
        }
        
    }
}
