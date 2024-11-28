using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverManager : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    [SerializeField] Material material;
    [SerializeField] Color defalutColor;
    bool lastIsFever = false;
    int lastScore = 0;
    float beginTime = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(stateManager.enableFever)
        {
            if(!lastIsFever) beginTime = Time.time;
            if(Time.time - beginTime > stateManager.feverTime) stateManager.enableFever = false;
        }
        
        lastIsFever = stateManager.enableFever;
        
        
    //     if(!stateManager.isGameOver && )
    //     {
    //         float stdInterval = stateManager.comboRemainTime / stateManager.comboMaxTime;
    //         material.color = new Color(
    //             defalutColor.r * stdInterval,
    //             defalutColor.g * stdInterval,
    //             defalutColor.b * stdInterval
    //         );
            
    //        // Debug.Log("std inteval:" + stdInterval);
    //         if(stateManager.comboRemainTime < 0)
    //         {
    //             stateManager.comboMaxTime = stateManager.comboDefaultMaxTime;
    //             stateManager.comboRemainTime = stateManager.comboMaxTime;
    //         }
    //         else
    //         {
    //             if(stateManager.score > lastScore)
    //             {
    //                 material.color = defalutColor;

    //                 stateManager.comboMaxTime *= 0.95f;
    //                 stateManager.comboRemainTime = stateManager.comboMaxTime;
    //             }
    //         }

    //         lastScore = stateManager.score;
    //         stateManager.comboRemainTime -= Time.deltaTime;

    //     }
        
    }
}
