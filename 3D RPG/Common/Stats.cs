using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [SerializeField] int baseValue;     // 기본 스탯값
    [SerializeField] int finalValuse;   // 최종 스탯값

    [SerializeField] List<int> modifiers = new List<int>(); // 수정 된 스탯 정보를 담을 리스트

    // 최종스탯값을 반환
    public int GetValue()
    {
        finalValuse = baseValue;
        modifiers.ForEach(x => finalValuse += x);

        return finalValuse;
    }

    // 인스펙터 내 표시 정보 업데이트
    void UpdateValue()
    {
        finalValuse = baseValue;
        modifiers.ForEach(x => finalValuse += x);
    }
    
    // 스탯 증가
    public void AddModifier(int modifier)
    {
        if (0 == modifier) return;

        modifiers.Add(modifier);
        UpdateValue();
    }

    // 스탯 감소
    public void RemoveModifier(int modifier)
    {
        if (0 == modifier) return;

        modifiers.Remove(modifier);
        UpdateValue();
    }
}
