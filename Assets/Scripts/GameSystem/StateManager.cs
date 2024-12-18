using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StateManager : MonoBehaviour
{

  [Header("データ収集")]
  public bool enableCreateCsvData = false;
  [Range(1, 2)] public int userStudyID = 1;
  public string userID = "0";
  [Range(0, 2)] public int conditionID = 0;
  public float dataSaveRate = 10f;

  [Header("共通変数")]
  public int score = 0;
  public bool isTrackingUser = false;
  public float maxGamePlayTime = 300f;
  public float maxGamePlayTime_short = 150f;
  public float remainTimef;
  public float decisionTime = 0;
  public float maxDecisionTime = 5f;
  public bool isGameOver = false;
  public float distanceBetweenSceens = 0f;
  public bool isPlayingPacman = false;
  public bool enablePTPS = false;
  public bool isTrackingUserOnP1 = false;
  public bool isTrackingUserOnP2 = false;
  public float trackingTimeOnP1 = 0f;
  public float trackingTimeOnP2 = 0f;
  public int userPlayingScreen = 0;

  [Header("パックマン用変数")]
  public int hitPoint = 0;
  public int maxHitPoint = 5;         
  public bool isAttacked = false;
  public bool isVisibleCharacter = false;
  public int visibleCoinCountInP1 = 0;
  public int visibleCoinCountInP2 = 0;
  public bool enableFever = false;
  public bool enableFeverTrigger = false;
  public float feverTime = 10f;
  public float feverAlpha = 0;
  public float feverDistanceThreshold = 1f;
  public float feverDisableDistanceThreshold = 1f;
  public float interval = 15f;

  [Header("絵合わせ用変数")]  
  public bool isMemoryPhase = false;
  public float stepOnThresholdTime = 3f;
  public float maxWaitTime = 3f;
  public int targetCardId = 0;  
  public float memoryTime = 0f;
  public float maxMemoryTime = 150f;
  public float maxMemoryTime_short = 75f;
  public float timeOut = 15f;
  public bool isAnswered = false;
  public int matchStatus = 0;
  public float flipBackTime = 0f;
  public bool enableFlipBack = false;
  public int numPattern = 8;
  public int wrongCount = 0;
  public int trialCount = 0;

  [Header("画面設定関連")]
  [Range(0f, 1f)] public float screenSize_p1 = 0.85f;
  [Range(0f, 1f)] public float screenSize_p2 = 0.85f;
  [Range(0, 40)] public int blurScale = 40;

  void Start()
  {
    var sceneName = SceneManager.GetActiveScene().name;
    if(sceneName == "SAR-Pacman") isPlayingPacman = true;

    // numPattern = (conditionID == 0)? (int)(numPattern * 0.5) : numPattern;
    // maxGamePlayTime = (userStudyID == 2 && conditionID == 0)? maxGamePlayTime_short : maxGamePlayTime;
    // memoryTime = (userStudyID == 2 && conditionID == 0)? maxMemoryTime_short : memoryTime;
  }

  void Update()
  {
    if(maxHitPoint - hitPoint <= 0) isGameOver = true;
    
    remainTimef -= Time.deltaTime;

    if(!isGameOver) decisionTime = 0;

    if(decisionTime > maxDecisionTime) InitValue();   

    isVisibleCharacter = false;  

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
    enableFeverTrigger = false;

    if(isMemoryPhase)
    {
      remainTimef = maxMemoryTime;
    }
  }
}
