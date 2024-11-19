using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRandomLayout : MonoBehaviour
{
    [SerializeField] List<GameObject> cardObjects = new List<GameObject>();
    [SerializeField] Transform baseTransform;
    [SerializeField] float xRange = 3f;
    [SerializeField] float zRange = 2f;
    [SerializeField] int maxAttempt = 100;

    // Start is called before the first frame update
    void Start()
    {
        cardSpawner();
    }

    void cardSpawner()
    {
        foreach(GameObject card in cardObjects)
        {
            card.transform.position = new Vector3(0f, 10f, 0f);
        }

        foreach(GameObject card in cardObjects)
        {
            bool isSucceeded = false;
            int numAttempt = 0;

            while(!isSucceeded)
            {
                numAttempt ++;

                Vector3 randomPosition = new Vector3(
                baseTransform.position.x + Random.Range(-xRange * 0.5f, xRange * 0.5f),
                0.01f,
                baseTransform.position.z + Random.Range(-zRange * 0.5f, zRange * 0.5f)
                );

                //Debug.Log("attempt:" + randomPosition);

                float randomAngle = Random.Range(-180f, 180f);

                if(!Physics.CheckSphere(randomPosition, 2f))
                {
                    card.transform.position = randomPosition;
                    card.transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);

                    isSucceeded = true;
                }

                if(numAttempt > maxAttempt)
                {
                    Debug.LogWarning("Over Max Atempt");
                    isSucceeded = true;
                }
            }            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
