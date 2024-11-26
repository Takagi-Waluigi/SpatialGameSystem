using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVisibleTrackingCoin : MonoBehaviour
{
    public bool isVisibleInP1 = false;   
    public bool isVisibleInP2 = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        isVisibleInP1 = false;
        isVisibleInP2 = false;
    }

    private void OnWillRenderObject()
    {
        if(Camera.current.tag == "Projectoroid1") isVisibleInP1 = true;
        if(Camera.current.tag == "Projectoroid2") isVisibleInP2 = true;
    }
}
