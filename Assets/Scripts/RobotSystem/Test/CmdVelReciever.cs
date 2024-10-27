

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class CmdVelReciever : MonoBehaviour
{
    [SerializeField] ROSConnection ros;
    [SerializeField] string topicName = "cmd_vel";
    bool isRegistered = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isRegistered)
        {
            if(ros.isActiveAndEnabled)
            {
                ros.Subscribe<TwistMsg>(topicName, CmdVelSubscriber);
                isRegistered = true;
            }
        }
    }

    void CmdVelSubscriber(TwistMsg msg)
    {

        Debug.Log("Data Recieved!!! ---/" + Time.time + "/" + topicName +"/linear velocoty:" + msg.linear.x);
    }
}
