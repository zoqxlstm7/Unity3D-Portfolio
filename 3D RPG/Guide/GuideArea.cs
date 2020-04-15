using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArea : MonoBehaviour
{
    GuideManager guideManager;
    [SerializeField] float radius = 3f;  // 감지 범위

    private void Start()
    {
        guideManager = (GuideManager)FindObjectOfType(typeof(GuideManager));
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, PlayerManager.instance.player.position);

        // 플에이거 가이드지역으로 들어오면 가이드 이미지 호출 후 오브젝트 삭제
        if(distance <= radius)
        {
            guideManager.SetGuide();
            Destroy(gameObject);
        }
    }

    // 기즈모 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
