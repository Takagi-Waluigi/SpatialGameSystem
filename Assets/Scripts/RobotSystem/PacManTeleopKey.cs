using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class PacManTeleopKey : MonoBehaviour
{
    ROSConnection ros;
    [Header("基本設定")]
    [SerializeField] StateManager stateManager;
    [SerializeField] string topicName = "cmd_vel";
    [SerializeField] string rosNamespace = "";
    [SerializeField] float publishRate = 30f;
    [SerializeField] Transform cameraTranform;
    [SerializeField] Transform breakingTargetTransform;
    [Header("ゲーム対応挙動設定")]
    [SerializeField] bool gameRelatedMode = false;
    [SerializeField] [Range(0, 2)] int channel = 0;
    [SerializeField] double baseVelocity = 0.035;
    [SerializeField] double boostRatio = 1.5;
    [SerializeField] double breakingDistanceThreshold = 2;
    [SerializeField] double breakingMinimunDistance = 0.5f;
    [SerializeField] double breakingMinimumVeclocity = 0.005f;
    [SerializeField] SinglePoseSubscriber singlePoseSubscriber;
    float lastTime = 0;
    double maxLinearVelocity = 0.1f;
    double maxAngularVelocity = 1.0f;
    TwistMsg twistMsg;
    bool enableMultiFeverDrive = false;
    // Start is called before the first frame update
    void Start()
    {
        topicName = "/" + topicName;
        if(rosNamespace != "") topicName = "/" + rosNamespace + topicName; 
        
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(topicName);

        twistMsg = new TwistMsg();

        string statusMessage = "";
        // if(channel == 0) statusMessage = "--- boost when fever time";
        // if(channel == 1)
        // Debug.Log("[DRIVE MODE- " + rosNamespace + " --- boost when user step onto P2]");
    }

    // Update is called once per frame
    void Update()
    {
        if(gameRelatedMode)
        {

            if(!stateManager.isGameOver)
            {
                switch(channel)
                {
                    case 0: //フィーバー中に加速するモード（ユーザスタディ1 条件2）
                        twistMsg.linear.x = (stateManager.enableFever)? baseVelocity * boostRatio: baseVelocity;
                    break;

                    case 1: //特定のオブジェクトとの距離に合わせて速度が比例して変化する（ユーザスタディ2 条件3）
                        Vector2 vec2CameraPosition = new Vector2(cameraTranform.position.x, cameraTranform.position.z);
                        Vector2 vec2BreakingPosition = new Vector2(breakingTargetTransform.position.x, breakingTargetTransform.position.z);
                        double distanceToBreakingObject = (double)Vector2.Distance(vec2CameraPosition, vec2BreakingPosition);

                        Debug.Log("Distance to break object:" + distanceToBreakingObject);
                        twistMsg.linear.x = map(distanceToBreakingObject, breakingMinimunDistance, breakingDistanceThreshold, breakingMinimumVeclocity, baseVelocity, true);
                    break;

                    case 2: //ユーザが乗っかったら加速するモード
                        if(Input.GetKeyUp(KeyCode.M)) 
                        {
                            enableMultiFeverDrive = !enableMultiFeverDrive;
                            if(enableMultiFeverDrive) Debug.Log("[DRIVE MODE- " + rosNamespace + " --- boost when user step onto P2]");
                        }
                        
                        if(enableMultiFeverDrive)
                        {
                            twistMsg.linear.x = (stateManager.enableFever && singlePoseSubscriber.isTracking)? baseVelocity * boostRatio: baseVelocity;
                        }
                        
                    break;
                }
                                
            }
            else
            {
                twistMsg.linear.x = 0.0;
                twistMsg.angular.z = 0.0;
            }            
        }
        else
        {

            if(Input.GetKeyUp(KeyCode.UpArrow)) twistMsg.linear.x += 0.01;
            if(Input.GetKeyUp(KeyCode.DownArrow)) twistMsg.linear.x -= 0.01;
        }
        
        if(Input.GetKeyUp(KeyCode.LeftArrow)) twistMsg.angular.z += 0.05;
        if(Input.GetKeyUp(KeyCode.RightArrow)) twistMsg.angular.z -= 0.05;

        if(Input.GetKeyUp(KeyCode.Space))
        {
            gameRelatedMode = false;
            twistMsg.linear.x = 0.0;
            twistMsg.angular.z = 0.0;
        } 

        if(Input.GetKeyUp(KeyCode.G)) gameRelatedMode = true;

        if(twistMsg.linear.x > maxLinearVelocity) twistMsg.linear.x = maxLinearVelocity;
        if(twistMsg.linear.x < -maxLinearVelocity) twistMsg.linear.x = -maxLinearVelocity;

        if(twistMsg.angular.z > maxAngularVelocity) twistMsg.angular.z = maxAngularVelocity;
        if(twistMsg.angular.z < -maxAngularVelocity) twistMsg.angular.z = -maxAngularVelocity;

        if(Time.time - lastTime > 1f / publishRate)
        {
            ros.Publish(topicName, twistMsg);
        }
    }

    public double map(double value, double inputMin, double inputMax, double outputMin, double outputMax, bool clamp)
    {
        double outVal = ((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);
	
		if( clamp ){
			if(outputMax < outputMin){
				if( outVal < outputMax )outVal = outputMax;
				else if( outVal > outputMin )outVal = outputMin;
			}else{
				if( outVal > outputMax )outVal = outputMax;
				else if( outVal < outputMin )outVal = outputMin;
			}
		}
		return outVal;
    }
}
