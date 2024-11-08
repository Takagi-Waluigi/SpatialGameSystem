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
    public StateManager stateManager;
    public GameObject playAreaObject;
    public float lifeTime;    
    bool isTracking = false;
    float beginTime = 0f;
    NavMeshAgent agent;
    public float distanceThreshold = 0.15f;
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

        //開始時間の代入
        beginTime = Time.time;
        //Debug.Log("[ENEMY] time:" + beginTime);
    }

    // Update is called once per frame
    void Update()
    {
        //なににも当たらず自死する場合
        if(Time.time - beginTime > lifeTime) GameObject.Destroy(this.gameObject);
        //どこかしらに落下したらTrueになる処理
        if(isOnSomeWhere)
        {
            //落下したのがプレイエリアの場合
            if(isOnPlayArea)
            {
                //目的地の設定
                var destination = new Vector3();
                destination = characterObject.transform.position;
                
                if(this.GetComponent<NavMeshAgent>().enabled) agent.SetDestination(destination);                

                float distanceToCharacter = Vector3.Distance(this.transform.position, characterObject.transform.position);

                if(distanceToCharacter < distanceThreshold && stateManager.isTrackingUser)
                {
                    stateManager.hitPoint ++;
                    
                    GameObject.Destroy(this.gameObject);
                }
                
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
