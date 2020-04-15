using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupItemCtrl : MonoBehaviour
{
    #region Singleton
    public static PickupItemCtrl instance;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }
    #endregion

    public Image pressImage;                                      // 픽업 가능시 노출될 UI
    public List<PickupItem> pickupList = new List<PickupItem>();  // 픽업 가능 리스트 

    private void Update()
    {
        // 픽업 가능 목록이 존재하면 UI 노출 및 키 입력 대기
        if(pickupList.Count > 0)
        {
            if(!pressImage.gameObject.activeInHierarchy)
                pressImage.gameObject.SetActive(true);

            // E 키를 눌렀을 때 아이템 픽업 처리
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 인벤토리에 픽업 가능 아이템이 추가 가능한지 확인
                bool wasPickup = Inventory.instance.Add(pickupList[0].item);
                if (wasPickup)
                {
                    // 픽업 UI 숨김
                    pressImage.gameObject.SetActive(false);

                    // 픽업한 아이템은 리스트에서 삭제 및 게임오브젝트 삭제 처리
                    PickupItem oldItem = pickupList[0];
                    RemoveFromPickupList(oldItem);
                    Destroy(oldItem.gameObject);
                }
            }
        }
        else
        {
            if (pressImage.gameObject.activeInHierarchy)
                pressImage.gameObject.SetActive(false);
        }
    }

    // 픽업 가능 아이템 리스트에 추가
    public void AddFromPickupList(PickupItem item)
    {
        pickupList.Add(item);
    }

    // 픽업 불가능 아이템 리스트에서 삭제
    public void RemoveFromPickupList(PickupItem item)
    {
        if(pickupList.Count > 0)
        {
            pickupList.Remove(item);
        }
    }
}
