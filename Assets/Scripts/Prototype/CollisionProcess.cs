using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionProcess : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Hit by " + collision.gameObject.name);
        if(collision.gameObject.name == "Character")
        {
            GameObject.Destroy(this.gameObject);
        }
        
    }
}
