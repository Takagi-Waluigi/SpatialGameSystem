using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{
    Rigidbody rigidbody;
    bool isOnPlayArea = false;
    bool isOnSomeWhere = false;
    public GameObject characterObject;
    public GameObject userObject;
    public GameObject playAreaObject;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float distanceThreshold = 1.0f;
    Vector3[] bufferPositions = new Vector3[10];
    // Start is called before the first frame update
    void Start()
    {
        // characterObject = GameObject.Find("Character");
        // userObject = GameObject.Find("UserObject");

        agent.enabled = false;
        rigidbody = this.gameObject.GetComponent<Rigidbody>();

        for(int i = 0; i < bufferPositions.Length; i ++)
        {
            bufferPositions[i] = new Vector3();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isOnSomeWhere)
        {
            if(isOnPlayArea)
            {
                agent.SetDestination(characterObject.transform.position);

                float distanceToCharacter = Vector3.Distance(this.transform.position, characterObject.transform.position);

                if(distanceToCharacter < distanceThreshold) GameObject.Destroy(this.gameObject);

                float distanceToUser = Vector3.Distance(this.transform.position, userObject.transform.position);

                if(distanceToUser < distanceThreshold) GameObject.Destroy(this.gameObject);
                
            }
            else
            {
                for(int i = bufferPositions.Length - 1; i > 0; i --)
                {
                    bufferPositions[i] = bufferPositions[i - 1];
                }

                bufferPositions[0] = this.transform.position;

                int sum = 0;

                for(int i = 0; i < bufferPositions.Length; i ++)
                {
                    if(bufferPositions[i] == this.transform.position) sum ++;
                }

                if(sum == bufferPositions.Length) GameObject.Destroy(this.gameObject);
            }
        }
    }

     void OnCollisionEnter(Collision collision)
    {
        var colliededName = collision.gameObject.name;

        //キャラクターに当たったら削除
        if(colliededName == playAreaObject.name)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            isOnPlayArea = true;
            agent.enabled = true;
            isOnSomeWhere = true;
        }
        else
        {
            isOnSomeWhere = true;
        }        
    }
}
