using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{

    public World world;
    public GameObject slotPrefab;
    public UIITemSlot cursorSlot;
    public TextMeshProUGUI text;

    List<ItemSlot> slots = new List<ItemSlot>();


    private void Start()
    {
        for(int i = 1; i < world.blocks.Length; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, transform);
            ItemSlot slot = new ItemSlot(i, newSlot.GetComponent<UIITemSlot>());
            slot.isInventory = true;
        }
    }

    private void Update()
    {
        if (cursorSlot.itemSlot.blockID != 0)
            text.text = world.blocks[cursorSlot.itemSlot.blockID].blockName.ToString();
        else
            text.text = "".ToString();
    }
}
