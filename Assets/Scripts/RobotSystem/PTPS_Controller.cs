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
    [SerializeField] LaserObjectSubscriber laserObject_2;

    [SerializeField] int maxDesinatedSize = 4;
    [SerializeField] Transform mapObject;
    [SerializeField] Material maskMaterial;
    [SerializeField] float maskObjectPositionOffset = 5f;
    List<Vector4> detectedPositions = new List<Vector4>();

    float largeScale; 
    // Start is called before the first frame update
    void Start()
    {
        //マップスケールの大きいほうを取る
        largeScale = Mathf.Max(
            mapObject.transform.localScale.x, 
            mapObject.transform.localScale.z);

        //マップの位置を合わせる
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
        //配列の初期化
        detectedPositions.Clear();

        //各値の代入
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

        
        //レーザーオブジェクトの数が1位以上かを判定
        //Debug.Log("[DEBUG] user count:" + laserObject_1.objectWorldPositions.Count);

        int indexCount = (int)laserObject_1.objectWorldPositions.Count + (int)laserObject_2.objectWorldPositions.Count;
        maskMaterial.SetInt("_ActiveUserNum", indexCount);

        if(laserObject_1.objectWorldPositions.Count > 0)
        {
            for(int i = 0; i < laserObject_1.objectWorldPositions.Count; i ++)
            {
                var vec4Pos = new Vector4(
                    laserObject_1.objectWorldPositions[i].x,
                    0f,
                    laserObject_1.objectWorldPositions[i].z,
                    0f);

                detectedPositions.Add(vec4Pos);
            }
        }  

        if(laserObject_2.objectWorldPositions.Count > 0)
        {
            for(int i = 0; i < laserObject_2.objectWorldPositions.Count; i ++)
            {
                var vec4Pos = new Vector4(
                    laserObject_2.objectWorldPositions[i].x,
                    0f,
                    laserObject_2.objectWorldPositions[i].z,
                    0f);

                detectedPositions.Add(vec4Pos);
            }
        }  

        if(detectedPositions.Count > 0) maskMaterial.SetVectorArray("_UserPositions", detectedPositions);
    }
}
