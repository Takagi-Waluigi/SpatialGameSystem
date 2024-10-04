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
    public List<Transform> goalTransforms;
    NavMeshAgent agent;
    [SerializeField] float distanceThreshold = 1.0f;
    public bool pursuitMode = true;
    bool isNearToGoal = true;
    Vector3[] bufferPositions = new Vector3[10];
    // Start is called before the first frame update
    void Start()
    {
        //自分のオブジェクトから参照するコンポーネント
        rigidbody = this.gameObject.GetComponent<Rigidbody>();
        agent = this.gameObject.GetComponent<NavMeshAgent>();

        //初期状態ではNavmeshAgentは無効化 勝手に地面に貼りついてしまうのを防ぐ
        agent.enabled = false;
        
        //バッファの初期化
        for(int i = 0; i < bufferPositions.Length; i ++)
        {
            bufferPositions[i] = new Vector3();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //どこかしらに落下したらTrueになる処理
        if(isOnSomeWhere)
        {
            //落下したのがプレイエリアの場合
            if(isOnPlayArea)
            {
                //目的地の設定
                var destination = new Vector3();
                if(pursuitMode)
                {
                    destination = characterObject.transform.position;
                    agent.SetDestination(destination);
                }
                else
                {
                    if(goalTransforms.Count > 0)
                    {
                        if(isNearToGoal)
                        {
                            int goalId = (int)Random.Range(0, goalTransforms.Count);
                            destination = goalTransforms[goalId].position;
                            agent.SetDestination(destination);
                            isNearToGoal = false;
                        }
                        else
                        {
                            //float distanceToGoal = Vector3.Distance(this.transform.position, destination);
                            float distanceToGoal = agent.remainingDistance;
                            if(distanceToGoal < 0.05f) isNearToGoal = true;

                        }                        
                    }

                }

                

                float distanceToCharacter = Vector3.Distance(this.transform.position, characterObject.transform.position);

                if(distanceToCharacter < distanceThreshold) GameObject.Destroy(this.gameObject);

                float distanceToUser = Vector3.Distance(this.transform.position, userObject.transform.position);

                if(distanceToUser < distanceThreshold) GameObject.Destroy(this.gameObject);
                
            }
            //プレイエリア以外に落下した場合
            else
            {
                //しばらく（10フレーム）その場にとどまっていると判定されたらオブジェクトを削除
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
