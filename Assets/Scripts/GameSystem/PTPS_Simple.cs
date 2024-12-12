using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTPS_Simple : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    [SerializeField] Transform screen_1;
    [SerializeField] Transform screen_2;
    [SerializeField] Transform userTransform;
    [SerializeField] Material maskMaterial;
    Vector4[] detectedPositions = new Vector4[1];
    List<Vector4> dp_dynamicArray = new List<Vector4>();
    float largeScale; 
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<MeshRenderer>().enabled = stateManager.enablePTPS;
        //配列の初期化
        dp_dynamicArray.Clear();

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

        maskMaterial.SetInt("_ActiveUserNum", 1);   
        detectedPositions[0] = new Vector4(
            userTransform.position.x,
            0f,
            userTransform.position.z,
            0f);

        maskMaterial.SetVectorArray("_UserPositions", detectedPositions);        
    }
}
