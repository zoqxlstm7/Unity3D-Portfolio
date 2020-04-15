using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    CharacterStats myStats; // 유닛 스탯정보 객체

    public Image hpBar;     // 체력을 게이지로 표시할 이미지

    private void Start()
    {
        myStats = GetComponent<CharacterStats>();
    }

    private void Update()
    {
        // 체력 게이지 업데이트
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, myStats.currentHealth / myStats.maxHealth, Time.deltaTime * 5f);
    }
}
