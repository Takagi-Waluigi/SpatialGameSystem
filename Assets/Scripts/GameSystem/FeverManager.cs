using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverManager : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
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
        if(stateManager.userStudyID == 1)
        {
            if(stateManager.enableFever)
            {
                if(!lastIsFever) beginTime = Time.time;
                if(Time.time - beginTime > stateManager.feverTime) stateManager.enableFever = false;
            }
            
            lastIsFever = stateManager.enableFever;  
        }
              
    }
}
