using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{   
    [Header("共通変数")]
    public int score = 0;
    public bool isTrackingUser = false;
    public float remainTimef;
    public float decisionTime = 0;

    [Header("パックマン用変数")]
    public int hitPoint = 0;
    public int maxHitPoint = 5;        
    [SerializeField] float maxTime = 5f;
    public bool isGameOver = false;
    public bool isAttacked = false;
    public bool isVisibleCharacter = false;

    [Header("絵合わせ用変数")]
    public int firstCardId = 0;
    public bool isFlippingFirst = false;
    public bool isFlippingSecond = false;
    public bool isMatching = false;
    public float flipBackTime = 0f;
    public bool enableFlipBack = false;
    public List<int> matchedId = new List<int>();

    

    void Update()
    {
      if(maxHitPoint - hitPoint <= 0) isGameOver = true;
      
      if(!isGameOver) decisionTime = 0;

      if(decisionTime > maxTime) InitValue();   
    
      isVisibleCharacter = false;      
    }

    public void InitValue()
    {
        score = 0;
        hitPoint = 0;
        isGameOver = false;
    }
}
