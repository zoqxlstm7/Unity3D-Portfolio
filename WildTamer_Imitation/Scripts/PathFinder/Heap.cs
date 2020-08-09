using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapNode<T>
{
    #region Variables
    T[] heap;                               // 힙 배열
    int currentHeapCount;                   // 현재 힙 인덱스

    public int Count => currentHeapCount;   // 힙 길이 반환
    #endregion Variables

    #region Methods
    public Heap(int maxHeapSize)
    {
        // 힙 배열 초기화
        heap = new T[maxHeapSize];
    }

    /// <summary>
    /// 객체가 동일한지 반환하는 함수
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool Contains(T node)
    {
        return Equals(heap[node.HeapIndex], node);
    }

    /// <summary>
    /// 힙에 노드를 추가하는 함수
    /// </summary>
    /// <param name="node">노드</param>
    public void Add(T node)
    {
        // 노드 삽입
        node.HeapIndex = currentHeapCount;
        heap[currentHeapCount] = node;
        // 힙인덱스 증가
        currentHeapCount++;

        // 위쪽으로 거슬러 올라가며 f,h값이 작은 순서로 정렬
        SortUp(node);
    }

    /// <summary>
    /// 리프노드부터 루트노드까지 검사하는 함수
    /// </summary>
    /// <param name="node">검사할 노드</param>
    void SortUp(T node)
    {
        // 힙 배열에 0부터 저장되므로 그에 따른 부모인덱스 설정
        int parenIndex = (node.HeapIndex - 1) / 2;

        while (true)
        {
            // 비교할 부모 노드
            T parentNode = heap[parenIndex];

            // 현재노드가 부모노드보다 더 적은 비용을 가졌다면 스왑
            if (node.Compare(parentNode))
            {
                Swap(node, parentNode);
            }
            else
            {
                break;
            }

            parenIndex = (parenIndex - 1) / 2;
        }
    }

    /// <summary>
    /// 루트노드를 반환하고 제거하는 함수
    /// </summary>
    /// <returns>루트노드</returns>
    public T RemoveFirst()
    {
        // 루트 노드 저장
        T firstItem = heap[0];
        // 힙인덱스 감소
        currentHeapCount--;

        // 가장 마지막 노드를 루트노드로 설정
        heap[0] = heap[currentHeapCount];
        heap[0].HeapIndex = 0;

        // 리프노드부터 아래쪽으로 내려가며 f,h값이 작은 순서로 정렬
        SortDown(heap[0]);

        return firstItem;
    }

    /// <summary>
    /// 루트노드부터 리프노드까지 정렬하는 함수
    /// </summary>
    /// <param name="node">검사할 노드</param>
    void SortDown(T node)
    {
        while (true)
        {
            // 힙 배열에 0부터 저장되므로 그에 따른 자식인덱스 설정
            int leftChildIndex = node.HeapIndex * 2 + 1;
            int rightChildIndex = node.HeapIndex * 2 + 2;

            T childNode;

            // 왼쪽 자식노드가 존재한다면
            if(leftChildIndex < currentHeapCount)
            {
                childNode = heap[leftChildIndex];

                // 오른쪽 자식노드가 존재한다면
                if(rightChildIndex < currentHeapCount)
                {
                    // 오른쪽 자식과 왼쪽 자식을 비교하여 더 적은 비용을 지닌 노드 선택
                    if (heap[rightChildIndex].Compare(heap[leftChildIndex]))
                    {
                        childNode = heap[rightChildIndex];
                    }
                }

                // 자식노드가 현재노드보다 더 적은 비용을 가졌다면 스왑
                if (childNode.Compare(node))
                {
                    Swap(node, childNode);
                }
                else
                {
                    break;
                }
            }
            else
            {
                // 자식 노드가 존재하지 않는다면 루프 탈출
                break;
            }
        }
    }

    /// <summary>
    /// 노드 스왑 함수
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    void Swap(T nodeA, T nodeB)
    {
        heap[nodeA.HeapIndex] = nodeB;
        heap[nodeB.HeapIndex] = nodeA;

        int heapIndex = nodeA.HeapIndex;
        nodeA.HeapIndex = nodeB.HeapIndex;
        nodeB.HeapIndex = heapIndex;
    }
    #endregion Methods
}

public interface IHeapNode<T>
{
    int HeapIndex { get; set; }     // 힙 인덱스

    bool Compare(T node);           // 비교함수
}
