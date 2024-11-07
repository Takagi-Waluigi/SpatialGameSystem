using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationPac : MonoBehaviour
{
    [SerializeField] GameObject pacObjectA;
    [SerializeField] GameObject pacObjectB;
    [SerializeField] float intereval;

    float lastTime = 0;
    bool pacState = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastTime > intereval)
        {
            pacState = !pacState;
            lastTime = Time.time;
        }

        pacObjectA.GetComponent<MeshRenderer>().enabled = pacState;
        pacObjectB.GetComponent<MeshRenderer>().enabled = !pacState;
        
    }
}
