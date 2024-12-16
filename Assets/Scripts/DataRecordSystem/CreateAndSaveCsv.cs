using System.IO;
using System.Text;
using UnityEngine;
using Unity.Robotics;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;
using RosMessageTypes.Geometry;

public class CreateAndSaveCsv : MonoBehaviour
{
    ROSConnection ros;
    [SerializeField] StateManager stateManager;
    [SerializeField] string topicName;
    [SerializeField] Transform screen_1;
    [SerializeField] Transform screen_2;
    Pose unityPose;
    StreamWriter sw_1, sw_2;
    float lastUpdateTime = 0f;
    bool lastEnableCreateCsv = false;
    bool isSubscribingData = false;

    void Start()
    {
        topicName = "/" +  topicName;
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OdometryMsg>(topicName, OnSubscribeOdometryFromT265);

        if(stateManager.userStudyID == 1)
        {
            sw_1 = new StreamWriter(@"Assets/RecordedData/UserStudy1/" +  stateManager.userID + "_" + stateManager.conditionID.ToString() + ".csv", true, Encoding.GetEncoding("Shift_JIS"));
            string[] s1 = {"position.x", "position.z" , "time" , "score"};
            string s2 = string.Join(",", s1);
            sw_1.WriteLine(s2);
        }
        else if(stateManager.userStudyID == 2)
        {
            sw_1 = new StreamWriter(@"Assets/RecordedData/UserStudy2/" +  stateManager.userID + "_" + stateManager.conditionID.ToString() + "_" + stateManager.conditionIDSub + ".csv", true, Encoding.GetEncoding("Shift_JIS"));
            string[] s1 = {"position.x", "position.z" , "time" , "score", "trackingTimeP1", "trackingTimeP2", "distance"};
            string s2 = string.Join(",", s1);
            sw_1.WriteLine(s2);
        }

        
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return)) stateManager.enableCreateCsvData = !stateManager.enableCreateCsvData;

        if(stateManager.enableCreateCsvData && !lastEnableCreateCsv)  Debug.Log("[CSV] Data collection has been started in " + Time.time);
        if(!stateManager.enableCreateCsvData && lastEnableCreateCsv) SaveData();
        
        if(stateManager.enableCreateCsvData && Time.time - lastUpdateTime > 1f / stateManager.dataSaveRate && isSubscribingData)
        {
            if(stateManager.userStudyID == 1)
            {
                WriteData(unityPose.position.x.ToString(), unityPose.position.z.ToString(), Time.time.ToString(), stateManager.score.ToString());
            }
            else if(stateManager.userStudyID == 2)
            {
                if(!stateManager.isGameOver)
                {
                    float distanceBetweenSceens = Vector3.Distance(screen_1.position, screen_2.position);
                    WriteData(unityPose.position.x.ToString(), unityPose.position.z.ToString(), Time.time.ToString(), stateManager.score.ToString(), stateManager.trackingTimeOnP1.ToString(), stateManager.trackingTimeOnP2.ToString(), distanceBetweenSceens.ToString());
                }
            }
            
            lastUpdateTime = Time.time;
        }

        lastEnableCreateCsv = stateManager.enableCreateCsvData;
        isSubscribingData = false;
    }

    public void WriteData(string txt1, string txt2, string txt3, string txt4, string txt5, string txt6, string txt7)
    {
        string[] s1 = { txt1, txt2, txt3, txt4, txt5, txt6, txt7};
        string s2 = string.Join(",", s1);
        sw_1.WriteLine(s2); 
    }

    public void WriteData(string txt1, string txt2, string txt3, string txt4)
    {
        string[] s1 = { txt1, txt2, txt3, txt4};
        string s2 = string.Join(",", s1);
        sw_1.WriteLine(s2);
    }

    public void SaveData()
    {
        Debug.Log("[CSV] Data has been saved!!! in " + Time.time);

        sw_1.Close();
    }

     void OnSubscribeOdometryFromT265(OdometryMsg msg)
    {
        isSubscribingData = true;

        Vector3Msg vector3Msg = new Vector3Msg();
        vector3Msg.x = msg.pose.pose.position.x;
        vector3Msg.y = msg.pose.pose.position.y;
        vector3Msg.z = msg.pose.pose.position.z;

        unityPose = TFUtility.ConvertToUnityPose(vector3Msg, msg.pose.pose.orientation);             
    }
}

