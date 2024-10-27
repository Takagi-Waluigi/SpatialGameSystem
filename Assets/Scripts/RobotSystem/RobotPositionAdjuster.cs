using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPositionAdjuster : MonoBehaviour
{
    [SerializeField] GameObject targetObject;
    GameObject robotObject;
    [SerializeField] MapTransformer mapTransformer;
    [SerializeField] string frameName = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(robotObject == null)
        {
            robotObject = GameObject.Find(frameName);
        }
        else
        {
            targetObject.transform.position = robotObject.transform.position - mapTransformer.OriginPos;
            targetObject.transform.rotation = robotObject.transform.rotation;
        }
    }
}
