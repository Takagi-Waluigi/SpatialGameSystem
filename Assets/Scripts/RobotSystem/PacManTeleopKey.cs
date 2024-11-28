using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class PacManTeleopKey : MonoBehaviour
{
    ROSConnection ros;
    [Header("基本設定")]
    [SerializeField] StateManager stateManager;
    [SerializeField] string topicName = "cmd_vel";
    [SerializeField] string rosNamespace = "";
    [SerializeField] float publishRate = 30f;
    [Header("ゲーム対応挙動設定")]
    [SerializeField] bool gameRelatedMode = false;
    // [SerializeField] [Range(0, 1)] int channel = 0;
    // [SerializeField][Range(0, 0.05f)] double baseVelocity = 0.03;
    // [SerializeField][Range(1.0f, 5.0f)] double maxGain = 1.5;
    // [SerializeField] int maxFillCoinCount = 80;
    // [SerializeField] float velocityStep = 0.005f;
    int count = 0;
    int threshold = 5;
    int baseCoef = 5;
    float lastTime = 0;
    double scoreRelatedVelocity = 0.0;
    int lastScore = 0;
    double maxLinearVelocity = 0.1f;
    double maxAngularVelocity = 1.0f;

    TwistMsg twistMsg;
    // Start is called before the first frame update
    void Start()
    {
        topicName = "/" + topicName;
        if(rosNamespace != "") topicName = "/" + rosNamespace + topicName; 
        
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(topicName);

        twistMsg = new TwistMsg();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameRelatedMode)
        {
            if(!stateManager.isGameOver)
            {
                twistMsg.linear.x = (stateManager.enableFever)? 0.05 : 0.015;                
            }
            else
            {
                threshold = baseCoef;
                count = 0;
                scoreRelatedVelocity = 0.0;
                twistMsg.linear.x = 0.0;
                twistMsg.angular.z = 0.0;
            }
            
        }
        else
        {

            if(Input.GetKeyUp(KeyCode.UpArrow)) twistMsg.linear.x += 0.01;
            if(Input.GetKeyUp(KeyCode.DownArrow)) twistMsg.linear.x -= 0.01;
        }
        
        if(Input.GetKeyUp(KeyCode.LeftArrow)) twistMsg.angular.z += 0.05;
        if(Input.GetKeyUp(KeyCode.RightArrow)) twistMsg.angular.z -= 0.05;

        if(Input.GetKeyUp(KeyCode.Space))
        {
            threshold = baseCoef;
            count = 0;
            gameRelatedMode = false;
            twistMsg.linear.x = 0.0;
            twistMsg.angular.z = 0.0;
            scoreRelatedVelocity = 0.0;
        } 

        if(Input.GetKeyUp(KeyCode.G)) gameRelatedMode = true;

        if(twistMsg.linear.x > maxLinearVelocity) twistMsg.linear.x = maxLinearVelocity;
        if(twistMsg.linear.x < -maxLinearVelocity) twistMsg.linear.x = -maxLinearVelocity;

        if(twistMsg.angular.z > maxAngularVelocity) twistMsg.angular.z = maxAngularVelocity;
        if(twistMsg.angular.z < -maxAngularVelocity) twistMsg.angular.z = -maxAngularVelocity;

        if(Time.time - lastTime > 1f / publishRate)
        {
            ros.Publish(topicName, twistMsg);
        }
    }
}
