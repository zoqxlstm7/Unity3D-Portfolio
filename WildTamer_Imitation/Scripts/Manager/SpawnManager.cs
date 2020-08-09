using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Variables
    readonly float SPAWN_RANGE = 5.0f;

    readonly string MOUSE_FILE_PATH = "Prefabs/Mouse";
    readonly string DEAR_FILE_PATH = "Prefabs/Dear";
    readonly string RABBIT_FILE_PATH = "Prefabs/Rabbit";

    InGameSceneManager inGameSceneManager;
    #endregion Variables

    #region Unity Methods
    private void Awake()
    {
        inGameSceneManager = FindObjectOfType<InGameSceneManager>();
    }

    private void Start()
    {
        //StartCoroutine(TestSpawn());
    }
    IEnumerator TestSpawn()
    {
        yield return new WaitForSeconds(1.0f);
        inGameSceneManager = FindObjectOfType<InGameSceneManager>();
        for (int i = 0; i < 10; i++)
        {
            float ranRange = Random.Range(-SPAWN_RANGE, SPAWN_RANGE);
            Vector3 ranPos = Random.onUnitSphere;
            ranPos *= ranRange;
            ranPos.z = 0;

            inGameSceneManager.animalManager.Generate(DEAR_FILE_PATH, ranPos);
        }
    }
    #endregion Unity Methods

    #region Other Methods
    public Animal Spawn(string filePath, Vector3 generatePos)
    {
        return inGameSceneManager.animalManager.Generate(filePath, generatePos);
    }
    #endregion Other Methods
}
