using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class OscScreenSizeSender : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] StateManager stateManager;
    [SerializeField] OscConnection oscConnection;
    [SerializeField] string OSCAddressScreenSize = "/screenSize";
    [SerializeField] [Range(1, 2)] int robotId = 1;
    OscClient client;
    float blurScreenSize;

    // Start is called before the first frame update
    void Start()
    {
        blurScreenSize = (robotId == 1)? stateManager.screenSize_p1 : stateManager.screenSize_p2;
        client = new OscClient(oscConnection.host, oscConnection.port);
        client.Send(OSCAddressScreenSize, camera.orthographicSize, blurScreenSize, stateManager.blurScale);
    }

    // Update is called once per frame
    void Update()
    {
         blurScreenSize = (robotId == 1)? stateManager.screenSize_p1 : stateManager.screenSize_p2;
        if(Input.GetKeyUp(KeyCode.F1)) client.Send(OSCAddressScreenSize, camera.orthographicSize, blurScreenSize, stateManager.blurScale);
    }
}
