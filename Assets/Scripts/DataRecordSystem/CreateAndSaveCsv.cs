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
    Pose unityPose;
    StreamWriter sw;
    float lastUpdateTime = 0f;
    bool lastEnableCreateCsv = false;
    bool isSubscribingData = false;

    void Start()
    {
        topicName = "/" +  topicName;
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OdometryMsg>(topicName, OnSubscribeOdometryFromT265);

        sw = new StreamWriter(@"Assets/RecordedData/UserStudy1/" +  stateManager.userID + "_" + stateManager.conditionID.ToString() + ".csv", true, Encoding.GetEncoding("Shift_JIS"));
        string[] s1 = {"position.x", "position.z" + "time" };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return)) stateManager.enableCreateCsvData = !stateManager.enableCreateCsvData;

        if(stateManager.enableCreateCsvData && !lastEnableCreateCsv)  Debug.Log("[CSV] Data collection has been started in " + Time.time);
        if(!stateManager.enableCreateCsvData && lastEnableCreateCsv) SaveData();
        
        if(stateManager.enableCreateCsvData && Time.time - lastUpdateTime > 1f / stateManager.dataSaveRate && isSubscribingData)
        {
            WriteData(unityPose.position.x.ToString(), unityPose.position.z.ToString(), Time.time.ToString());
            lastUpdateTime = Time.time;
        }

        lastEnableCreateCsv = stateManager.enableCreateCsvData;
        isSubscribingData = false;
    }

    public void WriteData(string txt1, string txt2, string txt3)
    {
        string[] s1 = { txt1, txt2, txt3 };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
    }

    public void SaveData()
    {
        Debug.Log("[CSV] Data has been saved!!! in " + Time.time);
        sw.Close();
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

