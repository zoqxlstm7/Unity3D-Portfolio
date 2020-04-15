using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public static EquipmentManager instance;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }
    #endregion

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;       // 장비 탈/장착시 호출될 콜백함수 
    public delegate void OnStatsUIChanged();
    public OnStatsUIChanged onStatsUIChanged;           // 장비 탈/장착시 Stats UI 업데이트 시 사용될 콜백함수

    public SkinnedMeshRenderer targetMesh;      // 부모가 될 메쉬
    Equipment[] currentEquipment;               // 현재 착용중인 장비를 담을 배열
    SkinnedMeshRenderer[] currentMesh;          // 현재 착용중인 장비의 메쉬를 담을 배열

    private void Start()
    {
        // EquipmentSlot의 길이만큼 장비와 메쉬 배열에 할당
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
        currentMesh = new SkinnedMeshRenderer[numSlots];
    }

    // 장비 장착 함수
    public void Equip(Equipment newItem)
    {
        // enum정보를 이용하여 인덱스를 얻어옴
        int slotIndex = (int)newItem.equipmentSlot;

        // 장착된 장비가 있다면 해제
        Equipment oldItem = null;
        if (currentEquipment[slotIndex] != null)
        {
            // 장비 메쉬 삭제
            if (currentMesh[slotIndex] != null)
            {
                Destroy(currentMesh[slotIndex].gameObject);
                currentMesh[slotIndex] = null;
            }

            oldItem = currentEquipment[slotIndex];
            Inventory.instance.Add(oldItem);
        }

        // 장비 교체 콜백함수가 있다면 콜백함수 호출
        if (onEquipmentChanged != null)
            onEquipmentChanged.Invoke(newItem, oldItem);

        // 스탯 UI 콜백함수가 있다면 콜백함수 호출
        if (onStatsUIChanged != null)
            onStatsUIChanged.Invoke();

        // 해당 인덱스에 장비 착용
        currentEquipment[slotIndex] = newItem;

        // 장비 메쉬 적용
        SkinnedMeshRenderer newMesh = Instantiate(newItem.mesh);
        newMesh.transform.parent = targetMesh.transform;

        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;
        currentMesh[slotIndex] = newMesh;
    }

    // 장비 장착 해제 함수
    public void Unequip(int slotIndex)
    {
        Equipment oldItem = null;
        // 장착된 장비가 있다면 해제하고 인벤토리에 아이템 추가
        if (currentEquipment[slotIndex] != null)
        {
            // 장비 메쉬 삭제
            if(currentMesh[slotIndex] != null)
            {
                Destroy(currentMesh[slotIndex].gameObject);
                currentMesh[slotIndex] = null;
            }

            oldItem = currentEquipment[slotIndex];
            Inventory.instance.Add(oldItem);

            // 콜백함수가 있다면 콜백함수 호출
            if (onEquipmentChanged != null)
                onEquipmentChanged.Invoke(null, oldItem);

            // 콜백함수가 있다면 콜백함수 호출
            if (onStatsUIChanged != null)
                onStatsUIChanged.Invoke();

            currentEquipment[slotIndex] = null;
        }
    }
}
