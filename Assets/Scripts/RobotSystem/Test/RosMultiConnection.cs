using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

public class RosMultiConnection : MonoBehaviour
{
    [SerializeField] ROSConnection ros_robotA;
    [SerializeField] ROSConnection ros_robotB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A)) ros_robotA.Connect();
        if(Input.GetKey(KeyCode.B)) ros_robotB.Connect();
    }
}
