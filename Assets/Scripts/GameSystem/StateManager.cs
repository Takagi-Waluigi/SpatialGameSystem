using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public Material scoredMaterial;
    public Material unscoredMaterial;
    public int count;
    public bool isAttacked = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isAttacked) isAttacked = false;
    }
}
