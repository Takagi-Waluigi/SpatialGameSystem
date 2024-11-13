using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class OscScreenSizeSender : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] OscConnection oscConnection;
    [SerializeField] string OSCAddressScreenSize = "/screenSize";
    OscClient client;

    // Start is called before the first frame update
    void Start()
    {
        client = new OscClient(oscConnection.host, oscConnection.port);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.F1)) client.Send(OSCAddressScreenSize, camera.orthographicSize);
    }
}
