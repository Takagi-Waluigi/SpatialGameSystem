using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemySpawner : MonoBehaviour
{
    [Header("オブジェクトの登録")]
    [SerializeField] GameObject baseObject;
    [SerializeField] GameObject shipObject;
    [SerializeField] GameObject characterObject;
    [SerializeField] GameObject userObject;
    [SerializeField] GameObject playAreaObject;

    [Header("各種設定")]
    [SerializeField] float spawnDuration = 1f;
    [SerializeField] bool pursuitMode = true;
    [SerializeField] List<Transform>  goalTransforms;

    float lastTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform shipTransform = shipObject.GetComponent<Patrol>().shipObject.transform;
        
        if(Time.time - lastTime > spawnDuration)
        {
            var latestEnemy = GameObject.Instantiate(
                baseObject,
                new Vector3(
                    shipTransform.position.x, 
                    shipTransform.position.y - 0.5f, 
                    shipTransform.position.z),
                shipTransform.rotation, this.transform);

            var enemyNavi = latestEnemy.GetComponent<EnemyNavigation>();
            enemyNavi.characterObject = characterObject;
            enemyNavi.userObject = userObject;
            enemyNavi.playAreaObject = playAreaObject;
            enemyNavi.pursuitMode = pursuitMode;
            enemyNavi.goalTransforms = goalTransforms;

            lastTime = Time.time;
        }
    }
}
