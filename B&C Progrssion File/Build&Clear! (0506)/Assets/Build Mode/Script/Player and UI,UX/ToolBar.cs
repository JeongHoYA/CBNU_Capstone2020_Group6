using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBar : MonoBehaviour
{
    World world;    
    public Player player;                                                   // 플레이어 오브젝트

    public UIItemSlot[] slots;                                              // 툴바의 아이템슬롯
    public RectTransform highlight;                                         // 하이라이트의 위치
    public int slotIndex = 0;                                               // 슬롯 인덱스



    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        int index = 1;
        foreach(UIItemSlot s in slots)
        {
            ItemStack stack = new ItemStack(index, 99);
            ItemSlot slot = new ItemSlot(slots[index - 1], stack);
            index++;
        }
    }                                                 // 슬롯 초기화

    private void Update()
    {
        if (!world.inUI)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0)
            {
                if (scroll > 0)
                    slotIndex--;
                else
                    slotIndex++;

                if (slotIndex > slots.Length - 1)
                    slotIndex = 0;
                if (slotIndex < 0)
                    slotIndex = slots.Length - 1;

                highlight.position = slots[slotIndex].slotIcon.transform.position;
            }
        }
    }                                                // 마우스 스크롤로 슬롯 제어
}
