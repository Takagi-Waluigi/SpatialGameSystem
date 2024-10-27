using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class WaypointChanger : MonoBehaviour
{
    [SerializeField] SplineAnimate splineAnimate;
    [SerializeField] SplineContainer[] paths;
    int currentPathId = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
