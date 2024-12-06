using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using UnityEngine.SceneManagement;

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
    bool lastIsTracking = false;
    bool lastIsGameOver = false;

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
            var sceneName = SceneManager.GetActiveScene().name;
            int enableFeverInt = (stateManager.enableFever)? 1 : 0;
            if(sceneName == "SAR-Pacman")
            {
                client.Send(OSCAddressDynamic, stateManager.remainTimef, stateManager.decisionTime, enableFeverInt, stateManager.feverAlpha);
            }
            else
            {
                int isMemoryPhaseInt = (stateManager.isMemoryPhase)? 1 : 0;
                client.Send(OSCAddressDynamic, stateManager.remainTimef, stateManager.decisionTime, stateManager.targetCardId, isMemoryPhaseInt);
            }
           
            lastTime = Time.time;
        }

        if(stateManager.score != lastScore || stateManager.hitPoint != lastHitPoint || stateManager.isGameOver != lastIsGameOver || stateManager.isTrackingUser != lastIsTracking)
        {
            int isGameOverInt = (stateManager.isGameOver)? 1 : 0;
            int isTrackingUserInt = (stateManager.isTrackingUser)? 1 : 0;
            client.Send(OSCAddressStatic, (int)stateManager.score, (int)stateManager.hitPoint, isGameOverInt, isTrackingUserInt);
        }
        
        lastScore = stateManager.score;
        lastHitPoint = stateManager.hitPoint;
        lastIsTracking = stateManager.isTrackingUser;
        lastIsGameOver = stateManager.isGameOver;
        
    }

    void OnDisable()
    {
        client.Dispose();
    }
}
