using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 동물 우형
    enum AnimalType
    {
        Mouse,
        Dear,
        Rabbit,
        Alligator,
        Bear
    };

    #region Variables
    readonly float SPAWN_INTERVAL = 20.0f;      // 스폰 시간 간격

    [SerializeField] AnimalType type;           // 동물 유형
    [SerializeField] Transform[] waypoints;     // 경로지점
    [SerializeField] int sapwnCount;            // 스폰될 양
    [SerializeField] bool isRespawn = true;     // 리스폰 여부

    List<Animal> animals = new List<Animal>();  // 동물 리스트

    float spawnedtime;                          // 스폰된 시간
    string filePath = "";                       // 스폰될 동물 파일경로

    SpawnManager spawnManager;
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();

        // 파일경로 초기화
        filePath = "Prefabs/" + type.ToString();
        // 시작시 스폰 시작
        StartCoroutine(FirstSpawn());
    }

    /// <summary>
    /// 시작시 스폰 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator FirstSpawn()
    {
        yield return new WaitForSeconds(1.0f);
        Spawn();
    }

    private void Update()
    {
        // 리스폰 조건이 아닌경우 리턴
        if (!isRespawn)
            return;

        // 스폰된 경우 사망한 동물들을 제거
        if(animals.Count > 0)
        {
            for (int i = 0; i < animals.Count; i++)
            {
                if (animals[i].isDead)
                    animals.Remove(animals[i]);
            }

            if (animals.Count == 0)
                spawnedtime = Time.time;
        }
        else
        {
            // 남아있는 동물이 없는 경우 스폰간격에 맞춰 스폰
            if(Time.time - spawnedtime >= SPAWN_INTERVAL)
            {
                Spawn();
                spawnedtime = Time.time;
            }
        }
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 스폰 실행 함수
    /// </summary>
    void Spawn()
    {
        // sapwnCount만큼 스폰
        for (int i = 0; i < sapwnCount; i++)
        {
            // 랜덤한 위치에 생성
            Vector3 ranPos = Random.onUnitSphere;
            ranPos.z = 0;

            // 경로 지정
            Animal animal = spawnManager.Spawn(filePath, transform.position + ranPos);
            animal.waypoints = waypoints;
            animals.Add(animal);
        }
    }
    #endregion Other Methods
}
