using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    World world;                                                    // 월드 오브젝트
    public Player player;                                           // 플레이어 오브젝트

    public RectTransform highlight;                                 // 하이라이트 아이콘 위치
    public ItemSlot[] itemSlots;

    int slotIndex = 0;

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        foreach(ItemSlot slot in itemSlots)
        {
            slot.icon.sprite = world.blockTypes[slot.itemID].icon;
            slot.icon.enabled = true;
        }
        player.selectedBlockIndex = itemSlots[slotIndex].itemID;
    }                                         // 슬롯 초기화

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(scroll != 0)
        {
            if (scroll > 0)
                slotIndex--;
            else
                slotIndex++;

            if (slotIndex > itemSlots.Length - 1)
                slotIndex = 0;
            if (slotIndex < 0)
                slotIndex = itemSlots.Length - 1;

            highlight.position = itemSlots[slotIndex].icon.transform.position;
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
        }
    }                                        // 마우스 스크롤로 슬롯 제어
}


/* 아이템슬롯 클래스 */
[System.Serializable]
public class ItemSlot
{
    public byte itemID;
    public Image icon;
}
