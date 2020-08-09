using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderAgent : MonoBehaviour
{
    #region Variables
    readonly float PATH_UPDATE_INTERVAL = 0.2f;     // 길찾기 업데이트 주기

    Vector3 destinationPoint;                       // 목표지점
    Vector3[] path;                                 // 경로
    int targetIndex;                                // 경로 인덱스
    bool isAvailableAgent;                          // 에이전트 실행 여부

    [SerializeField] float speed = 2f;              // 에이전트 속도
    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    [SerializeField] float stopDistance = 0f;       // 정지거리
    public float StopDistance
    {
        get => stopDistance;
        set => stopDistance = value;
    }

    bool isResetPath = false;                       // 경로가 재설정되는지 여부
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        StartAgent();
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 길찾기 에이전트 시작 함수
    /// </summary>
    public void StartAgent()
    {
        isAvailableAgent = false;
        // 에이전트 시작
        destinationPoint = transform.position;
        StartCoroutine(UpdatePath());
    }

    // 길찾기 에이전트 정지 함수
    public void StopAgent()
    {
        isAvailableAgent = true;
    }

    /// <summary>
    /// 목표지점 설정 함수
    /// </summary>
    /// <param name="point">목표지점</param>
    public void SetDestination(Vector3 point)
    {
        destinationPoint = point;
    }

    /// <summary>
    /// 목표지점 초기화
    /// </summary>
    public void ResetPath()
    {
        isResetPath = true;

        destinationPoint = transform.position;
    }

    /// <summary>
    /// 경로 결과 콜백함수
    /// </summary>
    /// <param name="newPath">새로운 경로</param>
    /// <param name="pathSuccessFul">길찾기 성공여부</param>
    public void OnPathFound(Vector3[] newPath, bool pathSuccessFul)
    {
        // 경로찾기에 성공했다면
        if (pathSuccessFul)
        {
            isResetPath = false;
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    /// <summary>
    /// 경로 요청 업데이트 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdatePath()
    {
        while (true)
        {
            if (isAvailableAgent)
                yield break;

            yield return new WaitForSeconds(PATH_UPDATE_INTERVAL);
            // 정지거리 밖에 있다면 길찾기 요청
            if (Vector3.Distance(transform.position, destinationPoint) > stopDistance)
            {
                PathFindManager.RequestPath(new PathRequest(transform.position, destinationPoint, OnPathFound));
            }
        }
    }

    /// <summary>
    /// 탐색된 경로로 이동하는 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        Vector3 currentWaypoin = path[0];

        while (true)
        {
            if (isResetPath)
                yield break;

            // 현재 위치가 현재 웨이포인트 위치와 같다면
            if (transform.position == currentWaypoin)
            {
                targetIndex++;
                // 경로를 모두 이동했다면 루프 탈출
                if (targetIndex >= path.Length)
                {
                    targetIndex = 0;
                    yield break;
                }
                // 현재 경로를 다음 경로로 지정
                currentWaypoin = path[targetIndex];
            }

            // 이동
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoin, speed * Time.deltaTime);
            yield return null;
        }
    }
    #endregion Other Methods

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                // 웨이포인트 지점에 기즈모 큐브 생성
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one * 0.1f);

                if (i == targetIndex)
                {
                    // 내위치에서 타겟 인덱스까지 라인 생성
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    // 다음 경로에 라인 생성
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
#endif
}
