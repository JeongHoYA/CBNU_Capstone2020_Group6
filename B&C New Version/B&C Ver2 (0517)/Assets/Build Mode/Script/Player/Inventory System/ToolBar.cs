using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class ToolBar : MonoBehaviour
{
    public World world;                                                                         // 월드 오브젝트
    public PlayerControl player;                                                                // 플레이어 오브젝트
    public RectTransform highLight;                                                             // 하이라이트 스프라이트 트랜스폼
    public TextMeshProUGUI text;                                                                // 블록 이름 텍스트

    public UIITemSlot[] slots;                                                                  // UIItemSlot 리스트
    
    public int slotIndex = 0;                                                                   // 슬롯 인덱스

    private void Start()
    {
        byte index = 1;
        foreach (UIITemSlot s in slots)
        {
            ItemSlot slot = new ItemSlot(index, slots[index - 1]);
            index++;
        }
    }

    private void Update()
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
        }
        highLight.position = slots[slotIndex].slotIcon.transform.position;
        text.text = world.blocks[slots[slotIndex].itemSlot.blockID].blockName.ToString();
    }
}
