using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{   
    public int score = 0;
    public int hitPoint = 0;
    public int maxHitPoint = 5;
    public float remainTimef;
    public float decisionTime = 0;
    [SerializeField] float maxTime = 5f;
    public bool isGameOver = false;
    public bool isAttacked = false;
    public bool isTrackingUser = false;

    void Update()
    {
      if(maxHitPoint - hitPoint <= 0) isGameOver = true;
      
      if(!isGameOver) decisionTime = 0;

      if(decisionTime > maxTime) InitValue();
    }

    public void InitValue()
    {
        score = 0;
        hitPoint = 0;
        isGameOver = false;
    }
}
