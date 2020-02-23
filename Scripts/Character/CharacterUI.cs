using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    PoolingManager poolingManager;

    //현재 적용중인 데미지텍스트를 담을 리스트
    List<DamageText> damageTexts = new List<DamageText>(); 
    float textOffset = 3f;      //데미지텍스트와 유닛 오브젝트간의 y오프셋
    float hpbarOffset = 1.5f;   //체력바와 유닛 오브젝트간의 y오프셋

    Image hpBarParent;
    Image hpBar;

    public bool isFollowTarget = true;  //체력바가 유닛을 따라다닐지 여부

    private void Start()
    {
        poolingManager = PoolingManager.instance;
    }

    private void FixedUpdate()
    {
        FollowTargetDamageText();
        FollowTargetHpBar();
    }

    //풀링된 체력바 오브젝트를 유닛 체력바 오브젝트에 할당
    public void CreateHpBar(Image newHpBar)
    {
        hpBarParent = newHpBar;
        hpBar = hpBarParent.transform.GetChild(0).GetComponent<Image>();
    }

    //체력바를 켜고 끄는 동작
    public void ActivatedHpBar(bool isActivate)
    {
        if(hpBarParent != null)
            hpBarParent.gameObject.SetActive(isActivate);
    }

    //데미지를 받을 시 체력바의 fillamount 업데이트
    public void UpdateHpBar(int maxHealth, int currentHealth)
    {
        if (hpBar == null)
            return;
        
        hpBar.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    //체력바를 유닛 머리위에 위치시키는 함수
    public void FollowTargetHpBar()
    {
        //체력바가 할당되지 않았거나 따라다니지 않아도 되는 경우 리턴
        if (hpBarParent == null || !isFollowTarget)
            return;

        hpBarParent.transform.position = WorldToScreenPointTransform(
            new Vector3(transform.position.x
            , transform.position.y + hpbarOffset
            , transform.position.z));
    }

    //데미지를 입었을 시 풀링된 데미지 텍스트를 불러와 실행시키는 함수
    public void CreateDamageText(int damage)
    {
        DamageText newDamageText = poolingManager.GetDamageText();
        if (newDamageText != null)
        {
            newDamageText.GetComponent<Text>().text = damage.ToString();
            newDamageText.gameObject.SetActive(true);

            damageTexts.Add(newDamageText);
        }
    }

    //데미지 텍스트를 유닛 머리위에 위치시키는 함수
    void FollowTargetDamageText()
    {
        if (damageTexts.Count == 0)
            return;

        //데미지가 여러차례 들어왔을 경우를 대비한 로직
        //데미지 텍스트가 사라지면 리스트에서 삭제
        for (int i = 0; i < damageTexts.Count; i++)
        {
            if (!damageTexts[i].gameObject.activeInHierarchy)
            {
                damageTexts.RemoveAt(i);
                i -= 1;
            }
        }

        for (int i = 0; i < damageTexts.Count; i++)
        {
            if (damageTexts[i].gameObject.activeInHierarchy)
            {
                damageTexts[i].transform.position = WorldToScreenPointTransform(
                    new Vector3(transform.position.x
                , transform.position.y + textOffset
                , transform.position.z));
            }
        }
    }

    //유닛이 죽었을 때 보관하고 있던 데미지 텍스트를 해제해준다.
    public void RemoveAllDamageText()
    {
        while(damageTexts.Count > 0)
        {
            damageTexts[0].gameObject.SetActive(false);
            damageTexts.RemoveAt(0);
        }
    }

    public Vector3 WorldToScreenPointTransform(Vector3 transPosition)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transPosition);
        pos.z = 0;

        return pos;
    }
}
