using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Vector3 offset;    // 기본 오프셋

    Transform target;
    public Transform Target
    {
        set => target = value;
    }

    private void LateUpdate()
    {
        // 게임 매니저가 없으면 리턴
        if (!FindObjectOfType<GameManager>())
            return;

        // 게임 오버시 리턴
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().IsGameOver)
            return;

        // 타겟이 없으면 리턴
        if (target == null)
            return;

        transform.position = Vector3.Lerp(target.position, transform.position + offset, 0.5f);

        // 스테이지 끝에 맞춰 카메라 위치 고정
        Vector3 pos = transform.position;

        if (pos.x <= -40)
            pos.x = -40;

        if (pos.x >= 40)
            pos.x = 40;

        if (pos.z <= -53)
            pos.z = -53;

        if (pos.z >= 25)
            pos.z = 25;

        transform.position = pos;
    }
}
