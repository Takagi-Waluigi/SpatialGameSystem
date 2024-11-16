using System.IO;
using System.Text;
using UnityEngine;

public class SampleSaveCsvScript : MonoBehaviour
{
    private StreamWriter sw_robot1, sw_robot2, sw_user;

    [SerializeField] string userID = "0";
    [SerializeField] string experimentID = "0";
    [SerializeField] string conditionID = "0";

    void Start()
    {
        sw_robot1 = new StreamWriter(@"Assets/RecordedData/Robot1/" +  userID + "_" +  experimentID + "_" + conditionID + ".csv", true, Encoding.GetEncoding("Shift_JIS"));
        sw_robot2 = new StreamWriter(@"Assets/RecordedData/Robot2/" +  userID + "_" +  experimentID + "_" + conditionID + ".csv", true, Encoding.GetEncoding("Shift_JIS"));
        string[] s1 = { "r1_screenX", "r1_screenZ", "r2_screenX", "r2_screenZ", "userX", "userZ" + "time" };
        string s2 = string.Join(",", s1);
        sw_robot1.WriteLine(s2);
    }

    public void SaveData(string txt1, string txt2, string txt3)
    {
        string[] s1 = { txt1, txt2, txt3 };
        string s2 = string.Join(",", s1);
        sw_robot1.WriteLine(s2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Data has been saved");
            sw_robot1.Close();
        }

    }
}

