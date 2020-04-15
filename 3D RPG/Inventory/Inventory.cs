using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    #region Sington
    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChanged;                           // 아이템을 획득했을 때 호출될 콜백함수

    public Image inventoryPanel;                                  // 인벤토리 패널
    public int inventorySpace = 25;                               // 인벤토리 최대 소지 수
    public List<Item> items = new List<Item>();                   // 아이템을 담을 인벤토리 리스트
    public bool useInventory = false;                         // 인벤토리 사용 유무

    const int maxStackable = 10;                                  // 소모성 아이템 최대로 쌓을 수 있는 갯수

    private void Update()
    {
        //인벤토리 UI on/off
        if (Input.GetKeyDown(KeyCode.I))
        {
            

            //인벤토리 오픈
            if (!inventoryPanel.gameObject.activeInHierarchy)
            {
                inventoryPanel.gameObject.SetActive(true);
                useInventory = true;

                // 인벤토리 오픈 시 마우스 커서 잠금해제
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                inventoryPanel.gameObject.SetActive(false);
                useInventory = false;

                // 인벤토리 닫을 시 마우스 커서 잠금
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    // 아이템 획득 시 호출되어 인벤토리 리스트에 아이템 추가
    public bool Add(Item item)
    {
        // 최대 소지 수 초과 시 아이템 획득 불가 처리
        if(items.Count >= 25)
        {
            Debug.Log("Inventory Full.");
            return false;
        }

        // 소모성 아이템인 경우
        if (item.isConsumable)
        {
            bool isStackable = false;   // 아이템 갯수를 쌓을 수 있는지 체크 
            for (int i = 0; i < items.Count; i++)
            {
                // 인벤토리 내에 같은 이름을 가지 아이템이 있다면
                if (items[i].itemName == item.itemName)
                {
                    // 갯수가 10개 이하라면 해당 아이템 갯수 +1
                    if (items[i].num < maxStackable)
                    {
                        isStackable = true;
                        items[i].num++;
                        break;
                    }
                    // 갯수가 10개 초과라면 다시 탐색
                    continue;
                }
            }
            // 아이템 갯수를 쌓을 수 없다면 인벤토리에 새로 할당
            if (!isStackable)
                items.Add(item);
        }
        else
        {
            // 소모성 아이템이 아닌 경우 인벤토리에 바로 추가 
            items.Add(item);
        }

        //items.Add(item);

        //콜백함수가 있다면 콜백함수 호출
        if (onItemChanged != null)
            onItemChanged.Invoke();

        return true;
    }

    // 아이템 소진 시 호출되어 인벤토리 리스트에서 아이템 삭제
    public void Remove(Item item)
    {
        if(items.Count > 0)
        {
            // 소모성 아이템 이라면
            if (item.isConsumable)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    // 인벤토리 내에 같은 이름을 가지 아이템이 있다면
                    if (items[i].itemName == item.itemName)
                    {
                        // 갯수가 2개 이상이면 -1, 1개 이하라면 바로 삭제
                        if (items[i].num > 1)
                            items[i].num--;
                        else
                        {
                            items[i].num--;
                            items.Remove(items[i]);
                        }

                        break;
                    }
                }
            }
            else
            {
                // 소모성 아이템이 아니면 인벤토리에서 바로 삭제
                items.Remove(item);
            }

            //콜백함수가 있다면 콜백함수 호출
            if (onItemChanged != null)
                onItemChanged.Invoke();
        }
    }
}
