using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot equipSlot;                                                                     // 착용 부위 설정
    public EquipmentMeshRegion[] coveredMeshRegions;                                                    // 플레이어 보디 블렌드 리스트

    public SkinnedMeshRenderer mesh;                                                                    // 해당 장비의 메시

    public int armorModifier;                                                                           // 방어력 점수
    public int damageModifier;                                                                          // 공격력 점수

    public override void Use()
    {
        base.Use();
        EquipmentManager.instance.Equip(this);
        RemoveFromInventory();
    }                                                                       // 해당 장비를 장착(사용)하는 오버라이드 함수
}

public enum EquipmentSlot { Head, Chest, Legs, Weapon, Shield, Feet}                                    // 아이템 장착부위 enum

public enum EquipmentMeshRegion { Legs, Arms, Torso };                                                  // 플레이어 보디 블렌드쉐이프에 반응하는 enum