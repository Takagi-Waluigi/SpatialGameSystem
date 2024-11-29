using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FeverNavigation : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField] StateManager stateManager;
    [SerializeField] Transform cameraTransform;
    [SerializeField] [Range(0f, 2f)] float maxRadius = 0.64f;
    [SerializeField] float objectSpeed = 0.1f;
    [SerializeField] float transparentSpeed = 0.5f;
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Color defaultColor;
    bool enableFeverTrigger = false;
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
        interval = (enableFeverTrigger)? stateManager.interval : stateManager.interval * 0.5f;

        if(!stateManager.enableFever && lastFever)
        {
            enableFeverTrigger = false;
            lastTime = Time.time;
        }

        if(Time.time -lastTime > interval)
        {
            enableFeverTrigger = !enableFeverTrigger;
            lastTime = Time.time;
        }

        if(stateManager.enableFever)
        {
            this.GetComponent<Collider>().enabled = false;
            this.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            this.GetComponent<Collider>().enabled = true;
            this.GetComponent<MeshRenderer>().enabled = true;
            float calculatedAlpha = 0.25f + 0.75f * Mathf.Sin(stateManager.remainTimef * transparentSpeed);

            stateManager.feverAlpha = (enableFeverTrigger)? calculatedAlpha : 0f;

            renderer.material.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, stateManager.feverAlpha);
        }

        

        float theta = Time.time * objectSpeed;
        float radiusX = maxRadius * 0.5f + maxRadius * 0.5f * Mathf.Sin(theta * (1f + Mathf.Sin(theta)));
        float radiusY = maxRadius * 0.5f + maxRadius * 0.5f * Mathf.Cos(theta * (1f + Mathf.Cos(theta)));

        Vector3 position = new Vector3(
            cameraTransform.position.x + radiusX * Mathf.Cos(theta),
            this.transform.position.y, 
            cameraTransform.position.z + radiusY * Mathf.Sin(theta)
        );

        this.transform.position = position;
        lastFever = stateManager.enableFever;
    }

     void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.name == "User" && stateManager.isTrackingUser && !stateManager.isGameOver && stateManager.feverAlpha > 0.75f)
        {
            stateManager.enableFever = true;
        }
    }


}
