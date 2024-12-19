using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FeverNavigation : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] StateManager stateManager;
    [SerializeField] Transform P1CameraTransform;
    [SerializeField] Transform P2CameraTransform;
    Vector3 cameraPosition = new Vector3();
    [SerializeField] [Range(0f, 2f)] float maxRadius = 0.64f;
    [SerializeField] float objectSpeed = 0.1f;
    [SerializeField] float apearenceSpeed = 0.05f;
    [SerializeField] float frameRate = 60f;
    float lastUpdateTime = 0;
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Color defaultColor;
    bool lastFever = false;
    float lastTime = 0;
    float interval = 0;
    // Start is called before the first frame update
    void Start()
    {
        renderer.material.color = defaultColor;
    }

    // Update is called once per frame
    void Update()
    {  
        interval = (stateManager.enableFeverTrigger)? stateManager.interval : stateManager.interval * 1.5f;

        if(!stateManager.enableFever && lastFever)
        {
            stateManager.enableFeverTrigger = false;
            lastTime = Time.time;
        }

        if(Time.time -lastTime > interval)
        {
            stateManager.enableFeverTrigger = !stateManager.enableFeverTrigger;
            lastTime = Time.time;
        }    

        if(stateManager.userPlayingScreen == 1)
        {
            cameraPosition = P1CameraTransform.position;
        }
        else if(stateManager.userPlayingScreen == 2)
        {
            cameraPosition = P2CameraTransform.position;
        }

        if(Time.time - lastUpdateTime > 1f / frameRate)
        {
            //ビジュアルの変更
            if(stateManager.enableFeverTrigger)
            {
                stateManager.feverAlpha += apearenceSpeed;
                if(stateManager.feverAlpha > 1.0f)  stateManager.feverAlpha = 1.0f;
            }
            else
            {
                stateManager.feverAlpha -= apearenceSpeed;
                if(stateManager.feverAlpha < 0.0f)  stateManager.feverAlpha = 0.0f; 
            }

            renderer.material.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, stateManager.feverAlpha);
            
            //位置に関する関数の記述
            float theta = Time.time * objectSpeed;
            float radiusX = maxRadius * 0.5f + maxRadius * 0.5f * Mathf.Sin(theta * (1f + Mathf.Sin(theta)));
            float radiusY = maxRadius * 0.5f + maxRadius * 0.5f * Mathf.Cos(theta * (1f + Mathf.Cos(theta)));

            Vector3 position = new Vector3(
                cameraPosition.x + radiusX * Mathf.Cos(theta),
                this.transform.position.y, 
                cameraPosition.z + radiusY * Mathf.Sin(theta)
            );

            this.transform.position = position;

            //更新時間の記録
            lastUpdateTime = Time.time;
        }
        
        this.GetComponent<Collider>().enabled = stateManager.enableFeverTrigger;      
        lastFever = stateManager.enableFever;
    }

     void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.name == "User" && stateManager.isTrackingUser && !stateManager.isGameOver && stateManager.feverAlpha > 0.250f)
        {
            stateManager.enableFever = true;
        }
    }


}
