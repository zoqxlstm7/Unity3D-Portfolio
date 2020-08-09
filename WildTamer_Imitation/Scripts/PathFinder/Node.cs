using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapNode<Node>
{
    #region Variables
    public Vector3 worldPosition;           // 월드 좌표
    public bool isObstacle;                 // 장애물 여부
        
    public int gridX;                       // 그리드 x 인덱스
    public int gridY;                       // 그리드 y 인덱스

    public int gCost;                       // 시작지점부터 현재지점까지 이동 비용
    public int hCost;                       // 현재지점부터 목표지점까지 이동 비용
    public int fCost => (gCost + hCost);    // g, f 비용의 합

    public Node parentNode;                 // 부모 노드

    int heapIndex;                          // 힙인덱스
    #endregion Variables

    #region Methods
    public Node(Vector3 worldPosition, bool isObstacle, int gridX, int gridY)
    {
        this.worldPosition = worldPosition;
        this.isObstacle = isObstacle;
        this.gridX = gridX;
        this.gridY = gridY;
    }
    #endregion Methods

    #region IHeapNode Interface
    // 힙 인덱스 프로퍼티
    public int HeapIndex
    {
        get => heapIndex;
        set => heapIndex = value;
    }

    /// <summary>
    /// f, h 비용 정렬 함수
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool Compare(Node node)
    {
        if (fCost < node.fCost)
            return true;
        else if (fCost == node.fCost && hCost < node.hCost)
            return true;

        return false;
    }
    #endregion IHeapNode Interface
}
