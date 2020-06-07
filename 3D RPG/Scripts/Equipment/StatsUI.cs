using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public Text statsText;      // 스탯 정보를 출력할 텍스트 객체

    private void Start()
    {
        // 콜백 함수 연결
        EquipmentManager.instance.onStatsUIChanged += UpdateUI;
    }

    // 화면에 표시될 때 스탯 정보 업데이트
    private void OnEnable()
    {
        UpdateUI();
    }

    // 스탯 정보 UI 업데이트
    void UpdateUI()
    {
        int damage = PlayerManager.instance.player.GetComponent<PlayerStats>().damage.GetValue();
        int armor = PlayerManager.instance.player.GetComponent<PlayerStats>().armor.GetValue();
        statsText.text = "데미지: " + damage + " 방어력: " + armor;
    }
}
