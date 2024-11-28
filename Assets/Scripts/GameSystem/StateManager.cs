using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{   
    [Header("共通変数")]
    public int score = 0;
    public bool isTrackingUser = false;
    public float maxGamePlayTime = 120f;
    public float remainTimef;
    public float decisionTime = 0;
    public bool isGameOver = false;

    [Header("パックマン用変数")]
    public int hitPoint = 0;
    public int maxHitPoint = 5;        
    [SerializeField] float maxTime = 5f;
    public bool isAttacked = false;
    public bool isVisibleCharacter = false;
    public int visibleCoinCountInP1 = 0;
    public int visibleCoinCountInP2 = 0;
    public float comboDefaultMaxTime = 3f;
    public float comboMaxTime = 3f;
    public float comboRemainTime = 0;
    public bool enableFever = false;
    public float feverTime = 10f;

    [Header("絵合わせ用変数")]
    public float stepOnThresholdTime = 3f;
    public float maxWaitTime = 3f;
    public int firstCardId = 0;
    public int secondCardId = 0;
    public GameObject firstCard;
    public GameObject secondCard;
    public bool isFlippingFirst = false;
    public bool isFlippingSecond = false;
    public bool isMatching = false;
    public float flipBackTime = 0f;
    public bool enableFlipBack = false;
    public List<int> matchedId = new List<int>();
    [Header("画面設定関連")]
    [Range(0f, 1f)] public float screenSize_p1 = 0.85f;
    [Range(0f, 1f)] public float screenSize_p2 = 0.85f;
    [Range(0, 40)] public int blurScale = 40;

    void Start()
    {
      remainTimef = maxGamePlayTime;
    }

    void Update()
    {
      if(maxHitPoint - hitPoint <= 0) isGameOver = true;
    
      if(!isGameOver) decisionTime = 0;

      if(decisionTime > maxTime) InitValue();   
    
      isVisibleCharacter = false;    

      remainTimef -= Time.deltaTime;

      if(remainTimef < 0) 
      {
        remainTimef = 0;
        isGameOver = true;
      }
      
    }

    public void InitValue()
    {
      score = 0;
      hitPoint = 0;
      isGameOver = false;
      remainTimef = maxGamePlayTime;
      matchedId.Clear();
      isFlippingFirst = false;
      isFlippingSecond = false;
    }
}
