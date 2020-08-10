using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolBar : MonoBehaviour
{
    public PlayerControl player;
    public RectTransform highLight;
    public UIITemSlot[] slots;

    public int slotIndex = 0;

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
    }
}
