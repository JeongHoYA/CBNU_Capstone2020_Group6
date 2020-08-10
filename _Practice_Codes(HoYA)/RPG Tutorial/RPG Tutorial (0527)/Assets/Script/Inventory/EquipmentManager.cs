using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton

    public static EquipmentManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    Equipment[] currentEquipment;                                                                                 // 현재 플레이어가 장착하고 있는 장비 리스트
    Inventory inventory;                                                                                          // 인벤토리 클래스 오브젝트

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);                                // 델리게이트 함수
    public OnEquipmentChanged onEquipmentChanged;

    private void Start()
    {
        inventory = Inventory.instance;

        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            UnEquipAll();
    }


    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipSlot;

        // 장착중인 아이템이 존재하면 해당 아이템을 인벤토리에 추가
        Equipment oldItem = null;
        if(currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
        }
        if (onEquipmentChanged != null)
            onEquipmentChanged.Invoke(newItem, oldItem);


        currentEquipment[slotIndex] = newItem;
    }                                                                       // 아이템을 장착하려 할 때 호출되는 함수

    public void UnEquip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);

            currentEquipment[slotIndex] = null;

            if (onEquipmentChanged != null)
                onEquipmentChanged.Invoke(null, oldItem);
        }
    }                                                                         // 해당 슬롯의 장비를 해제할 때 호출되는 함수

    public void UnEquipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
            UnEquip(i);
    }                                                                                   // 모든 슬롯의 장비를 해제할 때 호출되는 함수
}
