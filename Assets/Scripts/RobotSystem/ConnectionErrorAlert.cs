using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics;
using Unity.Robotics.ROSTCPConnector;
using Unity.VisualScripting;
using System.Data.Common;
public class ConnectionErrorAlert : MonoBehaviour
{
    ROSConnection ros;
    [SerializeField] AudioSource sound;
    bool lastConnectionStatus = false;
    bool forceStopSound = false;
    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        sound.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(ros.HasConnectionError)
        {
            if(!lastConnectionStatus)
            {
                sound.Play();
                forceStopSound = false;

                Debug.LogWarning(Time.time + ":Endpointとの接続が切断されました/ Qキーでアラート無効");
            }

            if(Input.GetKey(KeyCode.Q)) forceStopSound = true;

            
        }

        if(!ros.HasConnectionError || forceStopSound) sound.Stop();

        lastConnectionStatus = ros.HasConnectionError;


    }
}
