using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVisibleTrackingUser : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
