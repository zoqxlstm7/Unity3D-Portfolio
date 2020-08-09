using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    #region Variables
    [SerializeField] Transform target;      // 타겟

    Vector3 originPos;                      // 시작시 위치
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        // 시작 위치 저장
        originPos = transform.position;
    }

    private void LateUpdate()
    {
        // 카메라 이동 업데이트
        transform.position = Vector3.Lerp(transform.position, target.position + originPos, 1.0f);
    }
    #endregion Unity Methods
}
