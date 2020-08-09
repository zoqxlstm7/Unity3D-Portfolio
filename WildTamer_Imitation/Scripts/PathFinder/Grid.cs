using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Variables
    public bool isOnGridMapGizmo = false;           // 그리드맵 기즈모 활성 여부

    public Vector2 gridMapSize;                     // 그리드맵 사이즈
    public LayerMask obstacleMask;                  // 장애물 레이어 마스크

    public float nodeRadius = 0.5f;                 // 노드 반지름
    Node[,] grid;                                   // 그리드 배열

    float nodeDiameter;                             // 노드 지름

    int gridSizeX;                                  // 그리드 x사이즈
    int gridSizeY;                                  // 그리드 y사이즈

    public int GridSize => gridSizeX * gridSizeY;   // 그리드 전체 사이즈
    #endregion Variables

    #region Unity Methods
    private void Awake()
    {
        // 노드 지름 계산
        nodeDiameter = nodeRadius * 2;

        // 지름에 따른 그리드 x,y 갯수 계산
        gridSizeX = Mathf.RoundToInt(gridMapSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridMapSize.y / nodeDiameter);

        // 그리드맵 생성
        CreateGrid();
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 그리드맵을 생성하는 함수
    /// </summary>
    void CreateGrid()
    {
        // 그리드 배열 초기화
        grid = new Node[gridSizeX, gridSizeY];

        // 그리드를 왼쪽 바텀부터 그려주기 위해 왼쪽 바텀 좌표 계산
        Vector3 worldBottomLeft = transform.position - (Vector3.right * gridMapSize.x / 2) - (Vector3.up * gridMapSize.y / 2);
        worldBottomLeft.z = 0;

        // 그리드 생성
        for (int x = 0; x < gridMapSize.x; x++)
        {
            for (int y = 0; y < gridMapSize.y; y++)
            {
                // 그리드 월드 좌표값 계산
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                worldPoint.z = 0;
                // 장애물 여부 확인
                bool isObstacle = Physics2D.OverlapCircle(worldPoint, nodeRadius, obstacleMask);
                // 노드 생성
                grid[x, y] = new Node(worldPoint, isObstacle, x, y);
            }
        }
    }

    /// <summary>
    /// 이웃노드 리스트 반환 함수
    /// </summary>
    /// <param name="currentNode">현재노드</param>
    /// <returns>이웃노드 리스트</returns>
    public List<Node> GetNeighbourNode(Node currentNode)
    {
        // 이웃노드들을 담을 리스트
        List<Node> neighbours = new List<Node>();

        // 상하좌우 대각선의 이웃 노드들을 검사
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // 자기 자신은 무시
                if (x == 0 && y == 0)
                    continue;

                // 그리드 x, y 인덱스 계산
                int checkX = currentNode.gridX + x;
                int checkY = currentNode.gridY + y;

                // 그리드 배열 내에 좌표인지 검사
                if(0 <= checkX && checkX < gridSizeX && 0 <= checkY && checkY < gridSizeY)
                {
                    // 이웃노드 리스트에 추가
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        // 이웃노드 리스트 반환
        return neighbours;
    }

    /// <summary>
    /// 노드 좌표를 월드좌표로 변환 해주는 함수
    /// </summary>
    /// <param name="node">노드</param>
    /// <returns>변환된 노드</returns>
    public Node NodeToWorldPoint(Vector3 worldPosition)
    {
        // 노드 좌표를 월드 좌표의 퍼센트로 변환
        float persentX = (worldPosition.x + gridMapSize.x / 2) / gridMapSize.x;
        float persentY = (worldPosition.y + gridMapSize.y / 2) / gridMapSize.y;

        // 변환된 퍼센트값을 0~1사이로 변환
        persentX = Mathf.Clamp01(persentX);
        persentY = Mathf.Clamp01(persentY);

        // 그리드 x,y 인덱스 변환
        int x = Mathf.RoundToInt((gridSizeX - 1) * persentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * persentY);

        // 그리드 좌표 반환
        return grid[x, y];
    }
    #endregion Other Methods

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 그리드맵 사이즈 기즈모
        Gizmos.DrawWireCube(transform.position, new Vector2(gridMapSize.x, gridMapSize.y));

        if (!isOnGridMapGizmo)
            return;

        // 그리드 타일 기즈모
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.isObstacle ? Color.red : Color.white;
                Gizmos.DrawCube(node.worldPosition, Vector2.one * (nodeDiameter - 0.1f));
            }
        }
    }
#endif
}
