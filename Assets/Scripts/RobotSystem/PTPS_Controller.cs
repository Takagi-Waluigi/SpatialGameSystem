using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Performance.ProfileAnalyzer;
using UnityEngine;

public class PTPS_Controller : MonoBehaviour
{
    [SerializeField] Transform screen_1;
    [SerializeField] Transform screen_2;
    [SerializeField] LaserObjectSubscriber laserObject_1;
   // [SerializeField] LaserObjectSubscriber laserObject_2;

    [SerializeField] int maxDesinatedSize = 4;
    [SerializeField] Transform mapObject;
    [SerializeField] Material maskMaterial;
    [SerializeField] float maskObjectPositionOffset = 5f;

    List<Vector3> detectedPositions = new List<Vector3>();

    float largeScale; 
    // Start is called before the first frame update
    void Start()
    {
        largeScale = Mathf.Max(
            mapObject.transform.localScale.x, 
            mapObject.transform.localScale.z);

        this.transform.position =  new  Vector3(
            mapObject.transform.position.x,
            mapObject.transform.position.y + maskObjectPositionOffset,
            mapObject.transform.position.z);

        this.transform.rotation = mapObject.transform.rotation;
        this.transform.localScale = new Vector3(largeScale, 0, largeScale);
    }

    // Update is called once per frame
    void Update()
    {
        maskMaterial.SetFloat("_MapScale", largeScale);

        maskMaterial.SetVector("_RobotPos_0", new Vector4(
            screen_1.position.x,
            0f,
            screen_1.position.z,
            0f));

        maskMaterial.SetVector("_RobotPos_1", new Vector4(
            screen_2.position.x,
            0f,
            screen_2.position.z,
            0f));

        

        if(laserObject_1.objectWorldPositions.Count > 0)
        {
            Debug.Log("[DEBUG] user count:" + laserObject_1.objectWorldPositions.Count);
            int maxIndexNum = Mathf.Min(laserObject_1.objectWorldPositions.Count, maxDesinatedSize);

            for(int i = 0; i < maxIndexNum; i ++)
            {
                maskMaterial.SetVector("_UserPosition_" + i, new Vector4(
                    laserObject_1.objectWorldPositions[i].x,
                    0f,
                    laserObject_1.objectWorldPositions[i].z,
                    0f));
            }

        }

        // if(user.Length > 0)
        // {
        //     int minDistanceId = 0;
        //     float minDistance = 10000;
        //     for(int i = 0; i < user.Length; i ++)
        //     {
        //         maskMaterial.SetVector("_UserPosition_" + i, new Vector4(
        //             user[i].position.x,
        //             0f,
        //             user[i].position.z,
        //             0f));

        //         float distance = Vector3.Distance(screen_1.position, user[i].position);
        //         if(distance < minDistance)
        //         {
        //             minDistance = distance;
        //             minDistanceId = i;
        //         }

        //     }

            //maskMaterial.SetFloat("_Disntance", Vector3.Distance(screen_1.position, user[minDistanceId].position));
        //  }    
    }
}
