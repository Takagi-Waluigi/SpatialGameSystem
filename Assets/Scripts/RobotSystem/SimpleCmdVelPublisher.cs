using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;

public class SimpleCmdVelPublisher : MonoBehaviour
{
    ROSConnection ros;
    [SerializeField] string topicName = "cmd_vel";
    [SerializeField] string rosNamespace = "";

    [SerializeField] float publishRate = 30f;
    float interval = 0f;

    float lastTime = 0f;
    float linear = 0f;
    float angular = 0f;
    // Start is called before the first frame update
    void Start()
    {
        topicName = "/" + topicName;
        if(rosNamespace != "" ) topicName = "/" + rosNamespace + topicName;

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(topicName);

        interval = 1f / publishRate;
    }

    // Update is called once per frame
    void Update()
    {
        var msg = new TwistMsg();

        if(Input.GetKeyUp(KeyCode.W)) linear += 0.01f;
        if(Input.GetKeyUp(KeyCode.X)) linear -= 0.01f;

        if(Input.GetKeyUp(KeyCode.A)) angular += 0.01f;
        if(Input.GetKeyUp(KeyCode.D)) angular -= 0.01f;

        if(Input.GetKeyUp(KeyCode.S))
        {
            angular = 0f;
            linear = 0f;
        }

        msg.linear.x = linear;
        msg.linear.y = 0f;
        msg.linear.z = 0f;

        msg.angular.x = 0f;
        msg.angular.y = 0f;
        msg.angular.z = angular;

        if(Input.anyKeyDown) Debug.Log("linear:" + linear + "/angular:" + angular);

        if(Time.time - lastTime > interval) 
        {
            ros.Publish(topicName, msg);
            lastTime = Time.time;
        }
        
        
    }
}
