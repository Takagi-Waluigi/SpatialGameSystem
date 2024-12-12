using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ResultAndRespawn : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    [SerializeField] GameObject baseButtonObject;
    [SerializeField] Transform screen_1;
    [SerializeField] Transform screen_2;
    [SerializeField] bool enableDemoForPTPS = false;
    GameObject button_1, button_2;
    bool lastIsGameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(stateManager.isGameOver && lastIsGameOver != stateManager.isGameOver)
        {
            button_1 = GameObject.Instantiate(baseButtonObject, screen_1.position, Quaternion.identity);
            button_2 = GameObject.Instantiate(baseButtonObject, screen_2.position, Quaternion.identity);   

            if(enableDemoForPTPS) stateManager.enablePTPS = true;         
        }

        if(stateManager.isGameOver)
        {
            button_1.transform.position = new Vector3(screen_1.transform.position.x, 0 , screen_1.transform.position.z);
            button_2.transform.position = new Vector3(screen_2.transform.position.x, 0 , screen_2.transform.position.z);
        }

        if(!stateManager.isGameOver && lastIsGameOver != stateManager.isGameOver)
        {
            GameObject.Destroy(button_1);
            GameObject.Destroy(button_2);

            if(enableDemoForPTPS) stateManager.enablePTPS = false;
        }

        lastIsGameOver = stateManager.isGameOver;
    }
}
