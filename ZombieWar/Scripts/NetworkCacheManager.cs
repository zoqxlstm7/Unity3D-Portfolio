using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkCacheManager
{
    public const int DEFAUT_CACHE_COUNT = 10;  // 추가 생성시 기본값

    /// <summary>
    /// 캐시 생성
    /// </summary>
    /// <param name="filePath">생성할 캐시 파일 경로</param>
    /// <param name="gameObject">생성할 캐시 오브젝트</param>
    /// <param name="cacheCount">생성할 양</param>
    /// <param name="parentTransform">부모 트랜스폼</param>
    public void Generate(string filePath, GameObject gameObject, int cacheCount, Transform parentTransform = null)
    {
        // 마스터 클라이언트만 실행
        if (!GameManager.Instance.NetworkManager.IsMasterClient)
            return;

        // 캐시가 생성되지 않았다면 캐시 생성
        if (!GameManager.Instance.NetworkManager.Caches.ContainsKey(filePath))
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            // 캐시 카운트만큼 캐시 생성
            for (int i = 0; i < cacheCount; i++)
            {
                GameObject go = PhotonNetwork.Instantiate(filePath, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(parentTransform);
                queue.Enqueue(go);
                SetActivated(go);
            }

            // 캐시 적재
            GameManager.Instance.NetworkManager.Caches.Add(filePath, queue);
        }
        else
        {
            // 캐시가 부족할 경우 추가 생성
            for (int i = 0; i < cacheCount; i++)
            {
                GameObject go = PhotonNetwork.Instantiate(filePath, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(parentTransform);
                GameManager.Instance.NetworkManager.Caches[filePath].Enqueue(go);
                SetActivated(go);
            }
        }
    }

    /// <summary>
    /// 캐시 보관 및 사용
    /// </summary>
    /// <param name="filePath">사용할 캐시 파일 경로</param>
    /// <param name="position">생성할 지점</param>
    /// <returns></returns>
    public GameObject Archive(string filePath, Vector3 position)
    {
        GameObject go = null;

        // 남아있는 캐시가 없다면
        if (GameManager.Instance.NetworkManager.Caches[filePath].Count == 0)
        {
            Debug.Log("Archive not remain. filePath: " + filePath);
            return null;
        }

        // 캐시가 저장되어 있다면 저장된 캐시 오브젝트 반환
        if (GameManager.Instance.NetworkManager.Caches.ContainsKey(filePath))
        {
            go = GameManager.Instance.NetworkManager.Caches[filePath].Dequeue();
            go.transform.position = position;
            go.SetActive(true);
            //SetActivated(go);

            return go;
        }

        return go;
    }

    /// <summary>
    /// 사용한 캐시 반환
    /// </summary>
    /// <param name="filePath">반환할 캐시 파일 경로</param>
    /// <param name="gameObject">반환할 오브젝트</param>
    /// <returns>반환 성공 유무</returns>
    public bool Restore(string filePath, GameObject gameObject)
    {
        // 마스터 클라이언트만 실행
        //if (!GameManager.Instance.NetworkManager.IsMasterClient)
        //    return false;

        // 캐시가 저장되어 있다면 캐시 오브젝트 반환 처리
        if (GameManager.Instance.NetworkManager.Caches.ContainsKey(filePath))
        {
            GameManager.Instance.NetworkManager.Caches[filePath].Enqueue(gameObject);
            gameObject.SetActive(false);
            //SetActivated(gameObject);

            return true;
        }

        return false;
    }

    /// <summary>
    /// 객체에 따른 활성/비활성 함수 호출
    /// </summary>
    /// <param name="go">사용할 객체</param>
    void SetActivated(GameObject go)
    {
        // 에너미 객체인 경우 
        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.SetActivated();
            return;
        }

        // 불렛 객체인 경우
        Bullet bullet = go.GetComponent<Bullet>();
        if (bullet != null)
        {
            //bullet.SetActivated();
            return;
        }
    }
}
