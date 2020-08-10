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

    public SkinnedMeshRenderer targetMesh;                                                                        // 장비를 장착할 부위의 변경할 타겟 메쉬

    public Equipment[] defaultItems;                                                                              // 게임 시작시 플레이어가 기본으로 장착하고 있는 아이템

    Equipment[] currentEquipment;                                                                                 // 현재 플레이어가 장착하고 있는 장비 리스트
    SkinnedMeshRenderer[] currentMeshes;                                                                          // 현재 플레이어가 장착하고 있는 장비의 메시

    Inventory inventory;                                                                                          // 인벤토리 클래스 오브젝트

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);                                // 델리게이트 함수
    public OnEquipmentChanged onEquipmentChanged;

    private void Start()
    {
        inventory = Inventory.instance;

        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
        currentMeshes = new SkinnedMeshRenderer[numSlots];

        EquipDefaultItems();
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
        UnEquip(slotIndex);
        Equipment oldItem = UnEquip(slotIndex);
        
        if (onEquipmentChanged != null)
            onEquipmentChanged.Invoke(newItem, oldItem);

        // 해당 부위 블렌드쉐이프 조정 후 장착중인 장비 리스트에 추가
        SetEquipmentBlendShapes(newItem, 100);
        currentEquipment[slotIndex] = newItem;

        // 메쉬 업데이트 후 플레이어의 트랜스폼과 본에 고정
        SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.mesh);
        newMesh.transform.parent = targetMesh.transform;
        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;
        currentMeshes[slotIndex] = newMesh;
    }                                                                       // 아이템을 장착하려 할 때 호출되는 함수

    public Equipment UnEquip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            if (currentMeshes[slotIndex] != null)
                Destroy(currentMeshes[slotIndex].gameObject);

            // 블렌드쉐이프 복구 후 인벤토리에 추가
            Equipment oldItem = currentEquipment[slotIndex];
            SetEquipmentBlendShapes(oldItem, 0);

            inventory.Add(oldItem);
            currentEquipment[slotIndex] = null;

            if (onEquipmentChanged != null)
                onEquipmentChanged.Invoke(null, oldItem);

            return oldItem;
        }
        return null;
    }                                                                         // 해당 슬롯의 장비를 해제할 때 호출되는 함수

    public void UnEquipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
            UnEquip(i);

        EquipDefaultItems();
    }                                                                                   // 모든 슬롯의 장비를 해제할 때 호출되는 함수

    void SetEquipmentBlendShapes(Equipment item, int weight)
    {
        foreach (EquipmentMeshRegion blendShape in item.coveredMeshRegions)
        {
            targetMesh.SetBlendShapeWeight((int)blendShape, weight);
        }
    }                                                   // 아이템에 맞춰 블렌드쉐이프를 조정하는 함수

    void EquipDefaultItems()
    {
        foreach (Equipment item in defaultItems)
            Equip(item);
    }                                                                                   // 기본 장비를 장착하는 함수

}
