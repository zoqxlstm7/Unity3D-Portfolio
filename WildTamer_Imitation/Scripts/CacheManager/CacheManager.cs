using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐시 데이터 클래스
/// </summary>
[System.Serializable]
public class CacheData
{
    public string filePath;     // 파일 경로
    public int cacheCount;      // 캐시 수
}

public class CacheManager
{
    #region Variables
    public const int DEFAULT_CACHE_COUNT = 5;  // 기본 생성 캐시 카운트

    // 캐시 데이터 저장 변수
    Dictionary<string, Queue<GameObject>> caches = new Dictionary<string, Queue<GameObject>>();
    #endregion Variables

    #region Other Methods
    /// <summary>
    /// 캐시 생성 함수
    /// </summary>
    /// <param name="filePath">파일 경로</param>
    /// <param name="cacheCount">캐시 생성 수</param>
    /// <param name="prefab">프리팹 오브젝트</param>
    /// <param name="parent">부모가 될 오브젝트 위치</param>
    public void Generate(string filePath, int cacheCount, GameObject prefab, Transform parent = null)
    {
        // 키 값이 있는지 검사
        if (!caches.ContainsKey(filePath))
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            // 캐시 생성
            for (int i = 0; i < cacheCount; i++)
            {
                GameObject go = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(parent);
                queue.Enqueue(go);
                go.SetActive(false);
            }

            // 캐시 등록
            caches.Add(filePath, queue);
        }
        else
        {
            // 캐시가 부족할 경우 추가 생성
            for (int i = 0; i < cacheCount; i++)
            {
                GameObject go = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(parent);
                caches[filePath].Enqueue(go);
                go.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 보관되어 있는 캐시데이터를 쓰는 함수
    /// </summary>
    /// <param name="filePath">파일 경로</param>
    /// <param name="appearPos">나타날 위치</param>
    /// <returns>캐시 데이터</returns>
    public GameObject Archive(string filePath, Vector3 appearPos)
    {
        GameObject go = null;

        // 키 값이 없다면 리턴
        if (!caches.ContainsKey(filePath))
            return null;

        // 남아있는 캐시가 없다면 리턴
        if (caches[filePath].Count <= 0)
        {
            return null;
        }

        // 캐시 데이터 불러옴
        go = caches[filePath].Dequeue();
        go.transform.position = appearPos;
        go.SetActive(true);

        return go;
    }

    /// <summary>
    /// 사용이 끝난 캐시 데이터 반환
    /// </summary>
    /// <param name="filePath">파일 경로</param>
    /// <param name="cacheObject">반환될 캐시 오브젝트</param>
    /// <returns>반환 여부</returns>
    public bool Restore(string filePath, GameObject cacheObject)
    {
        // 키 값이 없다면 리턴
        if (!caches.ContainsKey(filePath))
            return false;

        // 캐시 데이터 반환
        caches[filePath].Enqueue(cacheObject);
        cacheObject.transform.position = Vector3.zero;
        cacheObject.SetActive(false);

        return true;
    }
    #endregion Other Methods
}
