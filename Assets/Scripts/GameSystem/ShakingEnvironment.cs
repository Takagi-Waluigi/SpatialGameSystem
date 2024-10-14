using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakingEnvironment : MonoBehaviour
{
    [SerializeField] GameObject environmentObject;
    [SerializeField] StateManager gameStateManager;
    [SerializeField] readonly float shakingRange = 0.1f;
    [SerializeField] [Range(0f, 1f)] float shakingDecrementRatio = 0.95f;
    float runtimeShakingRange;
    // Start is called before the first frame update
    void Start()
    {
        runtimeShakingRange = shakingRange;
    }

    // Update is called once per frame
    void Update()
    {
       if(gameStateManager.isAttacked)
       {
            environmentObject.transform.position = new Vector3(
                Random.Range(-runtimeShakingRange, runtimeShakingRange),
                0f,
                Random.Range(-runtimeShakingRange, runtimeShakingRange)
            );

            runtimeShakingRange *= shakingDecrementRatio;
       }
       else
       {
            runtimeShakingRange = shakingRange;
            environmentObject.transform.position = Vector3.zero;
       }
    }
}
