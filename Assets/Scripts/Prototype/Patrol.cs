using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] float radius;

    [SerializeField] GameObject baseObject;
    
    public GameObject shipObject;
    Vector3 position = new Vector3();
    Vector3 lastPosition = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        shipObject = GameObject.Instantiate(baseObject, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        //周回位置の算出
        position.x = radius * Mathf.Cos(Time.time);
        position.y = radius * 0.1f * Mathf.Sin(Time.time * 3.0f);
        position.z = radius * Mathf.Sin(Time.time);        
        shipObject.transform.localPosition = position;

        //前回位置との差分から角度計算
        float angleDiff = Mathf.Atan2(position.z - lastPosition.z, position.x - lastPosition.x);
        angleDiff = angleDiff / Mathf.PI * 180f;   
        Quaternion orientation = Quaternion.Euler(0, -angleDiff, 0);
        shipObject.transform.localRotation = orientation;

        //更新
        lastPosition = position;
       
    }
}
