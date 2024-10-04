using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPositionTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var position = this.transform.position;

        if(Input.GetKey(KeyCode.UpArrow)) position.z += 0.01f;

        if(Input.GetKey(KeyCode.DownArrow)) position.z -= 0.01f;

        if(Input.GetKey(KeyCode.RightArrow)) position.x += 0.01f;

        if(Input.GetKey(KeyCode.LeftArrow)) position.x -= 0.01f;

        this.transform.position = position;
    }
}
