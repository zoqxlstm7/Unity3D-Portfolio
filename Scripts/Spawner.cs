using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    PoolingManager poolingManager;

    public Transform[] spawnPoint;      //유닛 스폰 지점
    public GameObject[] orb;            //타워를 구성하는 구 오브젝트

    public bool isStartSpawn = false;   //스폰 시작의 유무
    float spawnTime = 2f;
    float spawnBeteenTime;

    private void Start()
    {
        poolingManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PoolingManager>();
    }

    private void Update()
    {
        if (isStartSpawn)
            CreateSpawn();
    }

    //풀링된 적 유닛을 스폰
    void CreateSpawn()
    {
        if(Time.time - spawnBeteenTime > spawnTime)
        {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                FSMUnit newEnemy = poolingManager.GetEnemy();
                if (newEnemy == null)
                    break;

                newEnemy.transform.position = spawnPoint[i].position;
                newEnemy.gameObject.SetActive(true);
            }

            spawnBeteenTime = Time.time;
        }
    }

    //타워의 체력이 다했을때의 처리
    public void OnDeath()
    {
        GetComponent<CapsuleCollider>().enabled = false;

        isStartSpawn = false;

        for (int i = 0; i < orb.Length; i++)
        {
            Rigidbody orbRigid = orb[i].GetComponent<Rigidbody>();

            orbRigid.isKinematic = false;
            orbRigid.useGravity = true;

            orbRigid.AddExplosionForce(100f, orbRigid.transform.position + new Vector3(0, -0.53f, 0f), 10f);
        }

        poolingManager.ActivatedEnemyAllDie();
    }
}
