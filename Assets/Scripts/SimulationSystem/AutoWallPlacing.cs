using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutoWallPlacing : MonoBehaviour
{
    [SerializeField] GameObject baseObject;
    [SerializeField] float spreadSize = 3f;
    [SerializeField] int spreadNumber = 4;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
[ContextMenu("Create Wall")]
void CreateWalls()
{
    if(baseObject != null)
    {
        float interval = spreadSize / (float) spreadNumber;
        for(int x = 0; x < spreadSize; x ++)
        {
            for(int z = 0; z < spreadSize; z ++)
            {
                Vector3 localPosition = new Vector3(
                   ((float)x - (float)spreadSize * 0.5f) * interval + interval * 0.5f,
                   baseObject.transform.localScale.y * 0.5f,
                   ((float)z - (float)spreadSize * 0.5f) * interval + interval * 0.5f
                );

                var instancedObject = GameObject.Instantiate(baseObject, this.transform);
                instancedObject.transform.localPosition = localPosition;
            }
        }
    }
}
#endif
}
