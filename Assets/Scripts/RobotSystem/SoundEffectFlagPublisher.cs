using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
public class SoundEffectFlagPublisher : MonoBehaviour
{
    ROSConnection ros;
    [SerializeField] string topicName = "sound";
    [SerializeField] StateManager gameStateManager;
    int lastCount = 0;
    bool lastIsAttacked = false;

    // Start is called before the first frame update
    void Start()
    {
        topicName = "/" + topicName;
        
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Int32Msg>(topicName);
    }

    // Update is called once per frame
    void Update()
    {
        var msg = new Int32Msg();

        if(gameStateManager.isAttacked && !lastIsAttacked)
        {
            msg.data = 0;
            ros.Publish(topicName, msg);
        } 

        if(gameStateManager.score != lastCount)
        {
            msg.data = 1;
            ros.Publish(topicName, msg);
        }

        lastCount = gameStateManager.score;
        lastIsAttacked = gameStateManager.isAttacked;
    }
}
