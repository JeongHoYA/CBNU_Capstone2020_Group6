using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Stat                                                                                       // 스탯 클래스
{
    [SerializeField]
    private int baseValue;                                                                              // 스탯값

    private List<int> modifiers = new List<int>();


    public int GetValue()
    {
        int finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);
        return finalValue;
    }                                                                            // 기본 스탯 + 장비 스탯값 반환 함수


    public void AddModifier (int modifier)
    {
        if (modifier != 0)
            modifiers.Add(modifier);
    }                                                           // 스탯값 추가 함수

    public void RemoveModifier(int modifier)
    {
        if (modifier != 0)
            modifiers.Remove(modifier);
    }                                                         // 스탯값 제거 함수
}
