using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    const float OFFSET_Y = 2.5f;            // Y축 오프셋

    [SerializeField] Image hpBar;           // 체력바 이미지
    public float HpBar
    {
        get => hpBar.fillAmount;
        set => hpBar.fillAmount = value;
    }

    [SerializeField] Text nickNameText;     // 닉네임 텍스트
    public Text NickNameText
    {
        get => nickNameText;
    }

    Transform target;                       // 대상 객체
    bool isMove;                            // 움직임 여부

    private void Update()
    {
        UpdateMove();
    }

    /// <summary>
    /// 움직임 업데이트
    /// </summary>
    void UpdateMove()
    {
        if (!isMove)
            return;

        // 참조를 잃었다면 리턴
        if (target == null)
            return;

        // 월드포지션을 Screen Point로 변경
        Vector3 pos = Camera.main.WorldToScreenPoint(new Vector3(target.position.x,
                                                                target.position.y + OFFSET_Y,
                                                                target.position.z));
        pos.z = 0;

        // 위치 업데이트
        transform.position = pos;
    }

    /// <summary>
    /// HUD 셋팅
    /// </summary>
    /// <param name="target">대상 객체</param>
    public void SetHUD(Transform target)
    {
        this.target = target;
        isMove = true;
    }
}
