using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVisibleTracking : MonoBehaviour
{
    GameObject stateManagerObject = null;     
    // Start is called before the first frame update
    void Start()
    {
        stateManagerObject = GameObject.Find("GameStateManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnWillRenderObject()
    {
        if(Camera.current.tag == "Projectoroid1" || Camera.current.tag == "Projectoroid2")
        {
            stateManagerObject.GetComponent<StateManager>().isVisibleCharacter = true;
        }
    }

    
}
