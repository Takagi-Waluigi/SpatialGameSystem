using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class OscStateSender : MonoBehaviour
{
    [SerializeField] OscConnection oscConnection;
    [SerializeField] string OSCAddressStatic = "/gameState/static";
    [SerializeField] string OSCAddressDynamic = "/gameState/dynamic";
    [SerializeField] StateManager stateManager;
    [SerializeField] float publishRate = 30f;
    OscClient client;
    int lastScore = 0;
    int lastHitPoint = 0;
    float lastTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        client = new OscClient(oscConnection.host, oscConnection.port);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastTime > (1f / publishRate))
        {
            client.Send(OSCAddressDynamic, stateManager.remainTimef);
            lastTime = Time.time;
        }

        if(stateManager.score != lastScore || stateManager.hitPoint != lastHitPoint || stateManager.isGameOver)
        {
            int isGameOverInt = (stateManager.isGameOver)? 1 : 0;
            client.Send(OSCAddressStatic, (int)stateManager.score, (int)stateManager.hitPoint, isGameOverInt);
        }
        
        lastScore = stateManager.score;
        lastHitPoint = stateManager.hitPoint;
        
    }

    void OnDisable()
    {
        client.Dispose();
    }
}
