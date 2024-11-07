using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{   
    public int score;
    public int hitPoint;
    public int maxHitPoint = 5;
    public float remainTimef;
    public bool isGameOver = false;
    public bool isAttacked = false;
    public bool isTrackingUser_1 = false;
    public bool isTrackingUser_2 = false;
    public float respawnTime = 3f;
    float gameOverTime = 0;
    bool lastIsAttacked;

    void Update()
    {
      if(maxHitPoint - hitPoint < 0) isGameOver = true;    
    }
}
