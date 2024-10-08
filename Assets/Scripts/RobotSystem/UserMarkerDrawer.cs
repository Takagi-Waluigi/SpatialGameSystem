using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMarkerDrawer : MonoBehaviour
{
    [SerializeField] GameObject baseObject;
    [SerializeField] int numObject = 5;
    [SerializeField] LaserObjectSubscriber laserObject;
    
    GameObject[] markers = new GameObject[5]; 
    

    // Start is called before the first frame update
    void Start()
    {

        for(int i = 0; i < markers.Length; i ++)
        {
            markers[i] = GameObject.Instantiate(baseObject, this.transform);
            markers[i].name = "marker_" + i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(laserObject.objectWorldPositions.Count > 0)
        {
            for(int i = 0; i < markers.Length; i ++)
            {
                if(i < laserObject.objectWorldPositions.Count)
                {
                    markers[i].transform.localPosition = laserObject.objectWorldPositions[i];
                    markers[i].transform.localScale = baseObject.transform.localScale;
                }
                else
                {
                    markers[i].transform.position = Vector3.zero;
                    markers[i].transform.localScale = Vector3.zero;
                }

                float yaw = (Time.time + i * 0.25f) * 10f;

                markers[i].transform.rotation = Quaternion.Euler(0, yaw, 0);
                
            } 

        }
        else
        {
            for(int i = 0; i < markers.Length; i ++)
            {
                markers[i].transform.position = Vector3.zero;
                markers[i].transform.localScale = Vector3.zero;
            }
        }
    }
}
