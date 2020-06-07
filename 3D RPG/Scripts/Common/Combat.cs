using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CharacterStats otherStats = other.GetComponent<CharacterStats>();

        // 콜라이더 객체에 CharacterStats스크립트가 있는 경우 공격 처리 수행
        if(otherStats != null)
        {
            Debug.Log(otherStats.name);
            CharacterStats myStats = GetComponentInParent<CharacterStats>();
            otherStats.TakeDamage(myStats.damage.GetValue());

            // 데미지 처리 후 combat 콜라이더 숨김 처리
            transform.gameObject.SetActive(false);
        }
    }
}
