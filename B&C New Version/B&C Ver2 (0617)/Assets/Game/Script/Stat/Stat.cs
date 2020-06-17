using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat                                                                                       // 스탯 클래스
{
    [SerializeField]
    private float baseValue;                                                                            // 스탯값

    private List<float> modifiers = new List<float>();                                                  // 장비에 붙은 추가 스탯 리스트


    public float GetValue()
    {
        float finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);
        return finalValue;
    }                                                                            // 기본 스탯 + 장비 스탯값 반환 함수


    public void AddModifier(float modifier)
    {
        if (modifier != 0)
            modifiers.Add(modifier);
    }                                                           // 스탯값 추가 함수

    public void RemoveModifier(float modifier)
    {
        if (modifier != 0)
            modifiers.Remove(modifier);
    }                                                         // 스탯값 제거 함수
}
