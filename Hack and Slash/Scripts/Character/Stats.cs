using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [SerializeField]
    int baseValue = 0;
    [SerializeField]
    int finalValue = 0;

    List<int> modifiers = new List<int>();

    //리스트에 저장된 수치를 모두 더하여 반환
    public int GetValue()
    {
        finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);

        return finalValue;
    }

    //스탯 수치 추가
    public void AddModifier(int modifier)
    {
        if(modifier != 0)
            modifiers.Add(modifier);

        GetValue();
    }

    //스탯 수치 삭제
    public void RemoveModifier(int modifier)
    {
        if(modifier != 0)
            modifiers.Remove(modifier);

        GetValue();
    }
}
