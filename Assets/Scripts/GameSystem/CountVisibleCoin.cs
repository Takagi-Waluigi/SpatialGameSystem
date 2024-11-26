
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountVisibleCoin : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    [SerializeField] float frameRate = 1f;
    
    float lastTime = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float interval = 1f /  frameRate;

        if(Time.time -lastTime > interval)
        {
            stateManager.visibleCoinCountInP1 = 0;
            stateManager.visibleCoinCountInP2 = 0;
            
            GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

            foreach(GameObject coin in coins)
            {
                var coinTracker = coin.GetComponent<CameraVisibleTrackingCoin>();

                if(coinTracker.isVisibleInP1) stateManager.visibleCoinCountInP1 ++;
                if(coinTracker.isVisibleInP2) stateManager.visibleCoinCountInP2 ++;
            }    
        }
    }
}
