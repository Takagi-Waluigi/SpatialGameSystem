using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.Performance.ProfileAnalyzer;
using UnityEngine;

public class PTPS_Controller : MonoBehaviour
{
    [SerializeField] Transform screen_1;
    [SerializeField] Transform screen_2;
    [SerializeField] LaserObjectSubscriber laserObject_1;
    [SerializeField] LaserObjectSubscriber laserObject_2;

    [SerializeField] int maxDesinatedSize = 4;
    [SerializeField] Transform mapObject;
    [SerializeField] Material maskMaterial;
    [SerializeField] float maskObjectPositionOffset = 5f;
    UnityEngine.Vector4[] detectedPositions = new UnityEngine.Vector4[10];
    List<UnityEngine.Vector4> dp_dynamicArray = new List<UnityEngine.Vector4>();
    float largeScale; 
    // Start is called before the first frame update
    void Start()
    {
        //マップスケールの大きいほうを取る
        largeScale = Mathf.Max(
            mapObject.transform.localScale.x, 
            mapObject.transform.localScale.z);

        //マップの位置を合わせる
        this.transform.position =  new  UnityEngine.Vector3(
            mapObject.transform.position.x,
            mapObject.transform.position.y + maskObjectPositionOffset,
            mapObject.transform.position.z);

        this.transform.rotation = mapObject.transform.rotation;
        this.transform.localScale = new UnityEngine.Vector3(largeScale, 0, largeScale);
    }

    // Update is called once per frame
    void Update()
    {
        //配列の初期化
        dp_dynamicArray.Clear();

        //各値の代入
        maskMaterial.SetFloat("_MapScale", largeScale);

        maskMaterial.SetVector("_RobotPos_0", new UnityEngine.Vector4(
            screen_1.position.x,
            0f,
            screen_1.position.z,
            0f));

        maskMaterial.SetVector("_RobotPos_1", new UnityEngine.Vector4(
            screen_2.position.x,
            0f,
            screen_2.position.z,
            0f));

        
        //レーザーオブジェクトの数が1位以上かを判定
        //Debug.Log("[DEBUG] user count:" + laserObject_1.objectWorldPositions.Count);

        int indexCount = (int)laserObject_1.objectWorldPositions.Count + (int)laserObject_2.objectWorldPositions.Count;
        maskMaterial.SetInt("_ActiveUserNum", indexCount);
        if(indexCount > 0) Debug.Log(indexCount);
        if(laserObject_1.objectWorldPositions.Count > 0)
        {
            for(int i = 0; i < laserObject_1.objectWorldPositions.Count; i ++)
            {
                var vec4Pos = new UnityEngine.Vector4(
                    laserObject_1.objectWorldPositions[i].x,
                    0f,
                    laserObject_1.objectWorldPositions[i].z,
                    0f);

                dp_dynamicArray.Add(vec4Pos);
            }
        }  

        if(laserObject_2.objectWorldPositions.Count > 0)
        {
            for(int i = 0; i < laserObject_2.objectWorldPositions.Count; i ++)
            {
                var vec4Pos = new UnityEngine.Vector4(
                    laserObject_2.objectWorldPositions[i].x,
                    0f,
                    laserObject_2.objectWorldPositions[i].z,
                    0f);

                dp_dynamicArray.Add(vec4Pos);
            }
        }  

        for(int i = 0; i < detectedPositions.Length; i ++)
        {
            if(i < dp_dynamicArray.Count)
            {
                detectedPositions[i] = dp_dynamicArray[i];
            }
            else
            {
                detectedPositions[i] = new UnityEngine.Vector4(0f, 0f, 0f, 0f);
            }
        }

        if(dp_dynamicArray.Count > 0) maskMaterial.SetVectorArray("_UserPositions", detectedPositions);
    }
}
