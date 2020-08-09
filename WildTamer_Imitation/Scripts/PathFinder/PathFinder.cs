using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class PathFinder : MonoBehaviour
{
    #region Variables
    Grid grid;                              // 그리드
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        // 그리드 객체 초기화
        grid = GetComponent<Grid>();
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 길찾기 함수
    /// </summary>
    /// <param name="request">경로 요청 구조체</param>
    /// <param name="callback">콜백함수</param>
    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        // 경로 및 탐색 성공여부 초기화
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        // 월드 지점을 노드 지점으로 변환
        Node startNode = grid.NodeToWorldPoint(request.pathStart);
        Node targetNode = grid.NodeToWorldPoint(request.pathEnd);

        // 열린리스트와 닫힌 리스트 초기화
        Heap<Node> openSet = new Heap<Node>(grid.GridSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        // 시작 노드를 열린 리스트에 추가
        openSet.Add(startNode);

        // 열린 리스트가 0이라며 목표지점으로 가는 길이 없는 것
        while (openSet.Count > 0)
        {
            // 현재 노드에 열린리스트의 첫번째 값 할당
            Node currentNode = openSet.RemoveFirst();

            // 닫힌 리스트에 현재노드 추가
            closedSet.Add(currentNode);

            // 목표를 찾았다면
            if(currentNode == targetNode)
            {
                sw.Stop();
                //print("Find: " + sw.ElapsedMilliseconds + " ms");

                // 경로 탐색 완료 처리
                pathSuccess = true;
                break;
            }

            foreach (Node neighbour in grid.GetNeighbourNode(currentNode))
            {
                // 갈 수 없는 지역이거나 닫힌 리스트에 포함되어있다면 무시
                if (neighbour.isObstacle || closedSet.Contains(neighbour))
                    continue;

                // 현재노드를 거쳐 이웃노드로 가는 g 비용을 계산
                int moveCostToNeighbour = currentNode.gCost + GetCost(currentNode, neighbour);
                // 현재노드를 거쳐가는 g 비용이 이웃노드의 g 비용보다 작거나 열린리스트에 이웃노드가 포함되어 있지 않다면
                if(moveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // 이웃노드 비용 재계산
                    neighbour.gCost = moveCostToNeighbour;
                    neighbour.hCost = GetCost(neighbour, targetNode);
                    // 이웃노드의 부모를 현재노드로 변경
                    neighbour.parentNode = currentNode;

                    // 열린 리스트에 이웃노드가 포함되어 있지 않다면 추가
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // 탐색한 경로를 반환받음
        if (pathSuccess)
            waypoints = RetracePath(startNode, targetNode);
        // 경로가 있는 경우만 성공처리
        pathSuccess = waypoints.Length > 0;
        // 경로 탐색이 끝났다는 콜백함수 호출
        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    /// <summary>
    /// 이동 비용을 계산하는 함수
    /// </summary>
    /// <param name="nodeA">노드A</param>
    /// <param name="nodeB">노드B</param>
    /// <returns>비용 값</returns>
    int GetCost(Node nodeA, Node nodeB)
    {
        // 거리 계산
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        // x축 거리가 더 길때의 대각선 포함 이동 비용
        if (distanceX > distanceY)
            return distanceY * 14 + (distanceX - distanceY) * 10;

        // y축 거리가 더 길때의 대각선 포함 이동 비용
        return distanceX * 14 + (distanceY - distanceX) * 10;
    }

    /// <summary>
    /// 최종 경로를 계산하는 함수
    /// </summary>
    /// <param name="startNode">시작노드</param>
    /// <param name="targetNode">목표노드</param>
    /// <returns>최종 경로</returns>
    Vector3[] RetracePath(Node startNode, Node targetNode)
    {
        // 최종 경로를 담을 리스트
        List<Node> finalPath = new List<Node>();
        Node currentNode = targetNode;

        // 시작 노드까지 역으로 리스트에 담음
        while (currentNode != startNode)
        {
            finalPath.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        Vector3[] waypoints = PathToSimple(finalPath);
        // 시작지점부터 시작되도록 리버싱
        Array.Reverse(waypoints);

        return waypoints;
    }

    Vector3[] PathToSimple(List<Node> path)
    {
        // 이동지점 리스트
        List<Vector3> waypoints = new List<Vector3>();
        // 이전방향
        Vector2 oldDirection = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            // 현재지점부터 다음지점까지 방향 계산
            Vector2 newDirection = new Vector2(path[i].gridX - path[i - 1].gridX, path[i].gridY - path[i - 1].gridY);
            // 이전방향과 같지 않다면 웨이포인트 추가
            if (oldDirection != newDirection)
            {
                waypoints.Add(path[i].worldPosition);
            }
            // 이전 방향 재설정
            oldDirection = newDirection;
        }

        // 배열로 변환하여 반환
        return waypoints.ToArray();
    }
    #endregion Other Methods
}
