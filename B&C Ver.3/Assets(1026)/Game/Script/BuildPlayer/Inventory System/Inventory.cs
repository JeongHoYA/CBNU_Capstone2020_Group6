using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{

    public World world;
    public GameObject slotPrefab;
    public UIITemSlot cursorSlot;
    public TextMeshProUGUI blockname;

    List<ItemSlot> slots = new List<ItemSlot>();

    int type;               // 저장되어 있는 타입

    private void Start()
    {
        type = 1;
        InstantBlockinType(type);
    }

    private void Update()
    {
        PresentBlockName();
    }

    public void HandleInputData(int n)
    {
        type = n + 1;
        InstantBlockinType(type);
    }

    void InstantBlockinType(int n)
    {
        int childs = transform.childCount;
        for (int i = childs - 1; i > -1; i--)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }

        for (int i = 1; i < world.blocks.Length; i++)
        {
            if (world.blocks[i].blockType == n)
            {
                GameObject newSlot = Instantiate(slotPrefab, transform);
                ItemSlot slot = new ItemSlot(i, newSlot.GetComponent<UIITemSlot>());
                slot.isInventory = true;
            }
        }
    }

    void PresentBlockName()
    {
        if (cursorSlot.itemSlot.blockID != 0)
            blockname.text = world.blocks[cursorSlot.itemSlot.blockID].blockName.ToString();
        else
            blockname.text = "".ToString();
    }
}
