using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreativeInventory : MonoBehaviour
{
    World world;
    public GameObject slotPrefab;

    List<ItemSlot> slots = new List<ItemSlot>();

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        for (int i = 1; i < world.blocktypes.Length; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, transform);

            ItemStack stack = new ItemStack((byte)i, 64);
            ItemSlot slot = new ItemSlot(newSlot.GetComponent<UIItemSlot>(), stack);
            slot.isCreative = true;
        }
    }
}
