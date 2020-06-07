using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleManager : MonoBehaviour
{
    #region Singleton
    public static BossBattleManager instance;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }
    #endregion

    public GameObject bossAreaTrigger;  // 보스 입장 감지 트리거 객체
    [SerializeField] float radius = 5f; // 감지 범위

    bool isDetection = false;           // 보스지역으로 들어왔는지 탐지
    public bool isStart = false;        // 보스전 시작알림

    private void Update()
    {
        // 보스 구역 입장 감지
        if (!isDetection)
        {
            float distance = Vector3.Distance(bossAreaTrigger.transform.position, PlayerManager.instance.player.position);

            // 보스 구역 입장 시 카메라 이동 시작
            if (distance <= radius)
            {
                // 카메라 무빙
                StartCoroutine(CameraMove());
                isDetection = true;
            }
        }
    }

    // 보스전 카메라 무빙 처리
    IEnumerator CameraMove()
    {
        GameObject boss = GameObject.Find("Boss");
        Transform movePoint = bossAreaTrigger.transform.GetChild(0).GetComponentInChildren<Transform>();    // 카메라 이동 지점
        Camera.main.transform.GetComponent<FollowCamera>().isCameraMove = true;         // 카메라가 이동해야되는지 설정

        float t = 0f;

        // 카메라 이동 처리
        while (t <= 2f)
        {
            yield return null;

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, movePoint.position, Time.deltaTime);
            Camera.main.transform.LookAt(boss.transform);
            t += Time.deltaTime;
        }

        // 카메라 이동 후 보스의 액션 처리
        boss.GetComponent<BossFsm>().SetState(FsmState.PREPARATION);

        // 카메라 이동 종료와 보스전 시작
        yield return new WaitForSeconds(2f);
        Camera.main.transform.GetComponent<FollowCamera>().isCameraMove = false;
        isStart = true;
    }

    // 기즈모 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(bossAreaTrigger.transform.position, radius);
    }
}
