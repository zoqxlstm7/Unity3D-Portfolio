using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AroundPointFinder : MonoBehaviour
{
    #region Variables
    List<Vector3> movePositionList = new List<Vector3>();       // 이동 지점 리스트
    [SerializeField]
    float[] aroundDistance = new float[] { 2 };                 // 주변지점 거리 정의
    [SerializeField]
    int[] aroundPositionCount = new int[] { 10 };               // 주변지점 숫자 정의

    int positionIndex = 0;                                      // 이동지점 리스트인덱스
    Player player;                                              // 플레이어
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        player = FindObjectOfType<Player>();
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 이동지점을 찾아 반환하는 함수 - 테이밍 객체가 사용
    /// </summary>
    /// <param name="animal">애니멀 객체</param>
    /// <returns></returns>
    public Vector3 FindMovePoint(Animal animal)
    {
        // 리스트 인덱스대로 이동지점 반환
        int index = player.tamingList.IndexOf(animal);
        return movePositionList[index];
    }

    /// <summary>
    /// 이동지점을 찾아 반환하는 함수 - 애니멀 객체가 사용
    /// </summary>
    /// <returns></returns>
    public Vector3 FindMovePoint()
    {
        // 주변지점 리스트 초기화
        SetAroundPosition();

        // 인덱스대로 순서대로 반환
        int index = positionIndex;
        positionIndex = (positionIndex + 1) % movePositionList.Count;
        return movePositionList[index];
    }

    /// <summary>
    /// 주변지점 배열을 초기화하는 함수
    /// </summary>
    public void SetAroundPosition()
    {
        movePositionList.Clear();

        Vector3 startPos = transform.position;
        movePositionList.Add(startPos);

        for (int i = 0; i < aroundPositionCount.Length; i++)
        {
            List<Vector3> movePointList = SetAroundPosition(startPos, aroundDistance[i], aroundPositionCount[i]);
            movePositionList.AddRange(movePointList);
        }
    }

    /// <summary>
    /// 주변지점을 초기화하는 함수
    /// </summary>
    /// <param name="distance">거리 간격</param>
    /// <param name="positionCount">생성할 지점 갯수</param>
    /// <returns></returns>
    List<Vector3> SetAroundPosition(Vector3 startPos, float distance, int positionCount)
    {
        List<Vector3> movePointList = new List<Vector3>();

        for (int i = 0; i < positionCount; i++)
        {
            // 갯수대로 각도를 계산
            float angle = i * (360 / positionCount);
            // 방향을 계산하여 생성방향으로 이동지점을 생성
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.up;
            Vector3 position = startPos + dir * distance;
            // 이동지점 리스트에 추가
            movePointList.Add(position);
        }

        return movePointList;
    }
    #endregion Other Methods

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //for (int i = 0; i < movePositionList.Count; i++)
        //{
        //    Gizmos.DrawWireSphere(movePositionList[i], 0.3f);
        //}
    }
#endif
}
