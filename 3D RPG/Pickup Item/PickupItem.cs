using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item;                       // 픽업 시 획득할 아이템

    [SerializeField] float range = 3f;      // 픽업가능 감지 범위
    bool wasRange = false;                  // 픽업 가능 범위에 들어왔는가

    private void Start()
    {
        item = Instantiate(item);
    }

    private void Update()
    {
        // 플레이어와 오브젝트 사이의 거리를 계산
        float distance = Vector3.Distance(transform.position, PlayerManager.instance.player.position);

        // 플레이어가 감지 범위에 들어왔다면 픽업리스트에 추가
        if(distance <= range && !wasRange)
        {
            wasRange = true;
            PickupItemCtrl.instance.AddFromPickupList(GetComponent<PickupItem>());
        }// 감지 범위에서 벗어났다면 픽업리스트에서 삭제
        else if(distance > range && wasRange)
        {
            wasRange = false;
            PickupItemCtrl.instance.RemoveFromPickupList(GetComponent<PickupItem>());
        }
    }

    // 오브젝트를 선택했을 때의 기즈모 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
