using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public Material activeMaterial;
    public Material deactiveMaterial;
    public int count;
    public bool isAttacked = false;
    public bool isTrackingUser = false;
    public float respawnTime = 3f;
    float gameOverTime = 0;
    bool lastIsAttacked;
    void Update()
    {
      if(isAttacked && !lastIsAttacked) gameOverTime = Time.time;

      if(isAttacked) 
      {
        if(Time.time - gameOverTime > respawnTime) isAttacked = false;        
      }

      lastIsAttacked = isAttacked;          
    }
}
