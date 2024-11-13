using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinInstancing : MonoBehaviour
{
    [SerializeField] GameObject coinObject;
    [SerializeField] Transform parentTransform;
    [SerializeField] float placeHeight = 0.02f;
    [SerializeField] Vector2 courseSize;
   
    #if UNITY_EDITOR
    [ContextMenu("Object Instancing")]
    void ObjectInstance()
    {
        for(float x = 0; x < courseSize.x; x += 0.1f)
        {
            for(float y = 0; y < courseSize.y; y += 0.1f)
            {
                Vector3 defaultPosition = new Vector3();
                defaultPosition.x = x - courseSize.x * 0.5f;
                defaultPosition.y = placeHeight;
                defaultPosition.z = y - courseSize.y * 0.5f;

                var coin =  GameObject.Instantiate(coinObject, parentTransform);
                coin.transform.localPosition = defaultPosition;

            }
        }
    }

    #endif
}