using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVisibleTrackingUser : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    float lastTrackingTimeOnP1 = 0f;
    float lastTrackingTimeOnP2 = 0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(lastTrackingTimeOnP1 != stateManager.trackingTimeOnP1)
        {
            stateManager.userPlayingScreen = 1;
            //Debug.Log("[PLAYING] on P1");
        }
        else if(lastTrackingTimeOnP2 != stateManager.trackingTimeOnP2)
        {
            stateManager.userPlayingScreen = 2;
            //Debug.Log("[PLAYING] on P2");
        }       

        lastTrackingTimeOnP1 = stateManager.trackingTimeOnP1;
        lastTrackingTimeOnP2 = stateManager.trackingTimeOnP2;
    }

    private void OnWillRenderObject()
    {
        if(Camera.current.tag == "Projectoroid1" && stateManager.isTrackingUserOnP1)
        {
            stateManager.trackingTimeOnP1 += Time.deltaTime;
        }

        if(Camera.current.tag == "Projectoroid2" && stateManager.isTrackingUserOnP2)
        {
            stateManager.trackingTimeOnP2 += Time.deltaTime;
        }

    }
}
