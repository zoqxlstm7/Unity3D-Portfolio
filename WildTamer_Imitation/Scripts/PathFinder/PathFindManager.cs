using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class PathFindManager : MonoBehaviour
{
    #region Singtone
    static PathFindManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion Singtone

    #region Variables
    Queue<PathResult> resultQueue = new Queue<PathResult>();    // 경로 결과 정보 저장 큐
    PathFinder pathFinder;                                      // 경로탐색 객체
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        pathFinder = GetComponent<PathFinder>();
    }

    private void Update()
    {
        CheckResultQueue();
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 경로 탐색 결과가 있는지 확인하는 함수
    /// </summary>
    void CheckResultQueue()
    {
        // 경로 탐색 결과가 있다면
        if(resultQueue.Count > 0)
        {
            int resultCount = resultQueue.Count;
            // 다른 쓰레드가 접근하지 못하도록 locking
            lock (resultQueue)
            {
                // 콜백함수 호출
                for (int i = 0; i < resultCount; i++)
                {
                    PathResult result = resultQueue.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    /// <summary>
    /// 경로 요청 함수
    /// </summary>
    /// <param name="request">요청할 데이터</param>
    public static void RequestPath(PathRequest request)
    {
        // 경로 탐색 쓰레드 시작
        ThreadStart threadStart = delegate
        {
            instance.pathFinder.FindPath(request, instance.OnFinishPathing);
        };
        threadStart.Invoke();
    }

    /// <summary>
    /// 경로 탐색이 끝나면 호출되는 함수
    /// </summary>
    /// <param name="result">경로 결과</param>
    public void OnFinishPathing(PathResult result)
    {
        // 다른 쓰레드가 접근하지 못하도록 locking
        lock (resultQueue)
        {
            // 경로 탐색이 성공했다면 경로 결과 저장
            if (result.success)
                resultQueue.Enqueue(result);
        }
    }
    #endregion Other Methods
}

/// <summary>
/// 경로 요청 구조체
/// </summary>
public struct PathRequest
{
    public Vector3 pathStart;                   // 시작지점
    public Vector3 pathEnd;                     // 목표지점
    public Action<Vector3[], bool> callback;    // 콜백될 함수

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}

/// <summary>
/// 경로 결과 구조체
/// </summary>
public struct PathResult
{
    public Vector3[] path;                      // 최종 경로
    public bool success;                        // 경로 찾기 성공 여부
    public Action<Vector3[], bool> callback;    // 콜백될 함수

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}
