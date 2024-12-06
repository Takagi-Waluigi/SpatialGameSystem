using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CardRandomLayout : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    [SerializeField] List<GameObject> layoutedCardObjects = new List<GameObject>();
    int index = 0;
    bool lastIsGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        if(layoutedCardObjects.Count > 0)
        {            
            for(int i = 0; i < this.transform.childCount; i ++)
            {
                index = stateManager.conditionID;
                Transform child = this.transform.GetChild(i);
                child.transform.position = layoutedCardObjects[index].transform.GetChild(i).position;
                child.transform.rotation = layoutedCardObjects[index].transform.GetChild(i).rotation;
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {
    }
}
