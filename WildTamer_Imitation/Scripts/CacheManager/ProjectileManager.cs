using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    #region Variables
    // 캐시할 파일 정보를 담을 변수
    [SerializeField] CacheData[] cacheDatas;
    // 로드된 파일캐시 정보를 저장
    Dictionary<string, GameObject> fileCaches = new Dictionary<string, GameObject>();
    // 씬 매니저
    InGameSceneManager inGameSceneManager;
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        inGameSceneManager = FindObjectOfType<InGameSceneManager>();
        PrepareCache();
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 캐시를 준비하는 함수
    /// </summary>
    void PrepareCache()
    {
        // 캐시데이터만큼 반복하여 캐시 생성
        for (int i = 0; i < cacheDatas.Length; i++)
        {
            GameObject go = Load(cacheDatas[i].filePath);
            if (go)
                inGameSceneManager.cacheManager.Generate(cacheDatas[i].filePath, cacheDatas[i].cacheCount, go, transform);
        }
    }

    /// <summary>
    /// 리소스 데이터 로드 함수
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    GameObject Load(string filePath)
    {
        GameObject go = null;

        // 키 값이 없다면 리소스 생성
        if (!fileCaches.ContainsKey(filePath))
        {
            go = Resources.Load<GameObject>(filePath);
            if (!go)
            {
#if UNITY_EDITOR
                Debug.LogWarning("파일 캐시를 불러오지 못했습니다. filePath: " + filePath);
#endif
                return null;
            }

            fileCaches.Add(filePath, go);
        }
        else
        {
            // 키 값이 존재하면 기존 데이터 반환
            go = fileCaches[filePath];
        }

        return go;
    }

    /// <summary>
    /// 생성된 캐시를 불러오는 함수
    /// </summary>
    /// <param name="filePath">파일경로</param>
    /// <param name="appearPos">나타날 위치</param>
    public Projectile Generate(string filePath, Vector3 appearPos)
    {
        // 생성된 캐시를 불러옴
        GameObject go = inGameSceneManager.cacheManager.Archive(filePath, appearPos);
        if (go)
        {
            // 캐시 데이터 설정
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.FilePath = filePath;

            return projectile;
        }

        // 반환받을 객체가 없는 경우 추가 생성
        inGameSceneManager.cacheManager.Generate(filePath, CacheManager.DEFAULT_CACHE_COUNT, Load(filePath), transform);
        return Generate(filePath, appearPos);
    }

    /// <summary>
    /// 생성된 캐시를 반환하는 함수
    /// </summary>
    /// <param name="filePath">파일경로</param>
    /// <param name="cacheObject">반환될 캐시 오브젝트</param>
    /// <returns></returns>
    public bool Remove(string filePath, Projectile cacheObject)
    {
        // 키 값이 존재하는지 확인
        if (!fileCaches.ContainsKey(filePath))
        {
#if UNITY_EDITOR
            Debug.Log("삭제할 캐시데이터 경로가 잘못되었습니다. filePath: " + filePath);
#endif
            return false;
        }

        // 캐시 반환
        return inGameSceneManager.cacheManager.Restore(filePath, cacheObject.gameObject);
    }
    #endregion Other Methods
}
