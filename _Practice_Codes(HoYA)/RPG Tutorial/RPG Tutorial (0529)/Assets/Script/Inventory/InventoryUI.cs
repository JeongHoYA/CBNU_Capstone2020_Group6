using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;                                                                           // 아이템들을 모아놓는 창의 트랜스폼
    public GameObject inventoryUI;                                                                          // 인벤토리UI 오브젝트

    Inventory inventory;                                                                                    // 인벤토리 클래스

    InventorySlot[] slots;                                                                                  // 인벤토리슬롯 리스트

    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    void Update()
    {
        // I버튼 클릭시 인벤토리 on/off
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }


    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }                                                                                      // 인벤토리 UI내용 업데이트
}
