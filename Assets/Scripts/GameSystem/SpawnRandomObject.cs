using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomObject : MonoBehaviour
{
    [SerializeField] GameObject baseObject;
    [SerializeField] Vector2 maxSpreadArea;
    [SerializeField] int numObject;
    List<GameObject> rewardObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < numObject; i ++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-maxSpreadArea.x * 0.5f, maxSpreadArea.x * 0.5f),
                0.15f,
                Random.Range(-maxSpreadArea.y * 0.5f, maxSpreadArea.y * 0.5f)
            );

            var go = GameObject.Instantiate(baseObject, Vector3.zero, Quaternion.identity, this.transform);
            go.transform.localPosition = spawnPosition;

            rewardObjects.Add(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
