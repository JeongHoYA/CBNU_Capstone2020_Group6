using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIITemSlot : MonoBehaviour
{
    public World world;

    public bool isLinked = false;
    public ItemSlot itemSlot;
    public Image slotImage;
    public Image slotIcon;



    public bool HasItem
    {
        get
        {
            if (itemSlot == null)
                return false;
            else
                return itemSlot.HasItem;
        }
    }

    public void Link(ItemSlot _itemSlot)
    {
        itemSlot = _itemSlot;
        isLinked = true;
        itemSlot.LinkUISlot(this);
        UpdateSlot();
    }

    public void UnLink()
    {
        itemSlot.unLinkUISlot();
        itemSlot = null;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (itemSlot != null && itemSlot.HasItem)
        {
            slotIcon.sprite = world.blocks[itemSlot.blockID].icon;
            slotIcon.enabled = true;
        }
        else
            Clear();
    }

    public void Clear()
    {
        slotIcon.sprite = null;
        slotIcon.enabled = false;
    }

    private void OnDestroy()
    {
        if (isLinked)
            itemSlot.unLinkUISlot();
    }
}



public class ItemSlot
{
    public int blockID = 0;
    private UIITemSlot uiItemSlot = null;

    public ItemSlot (UIITemSlot _uiItemSlot)
    {
        blockID = 0;
        uiItemSlot = _uiItemSlot;
        uiItemSlot.Link(this);
    }

    public ItemSlot(int _blockID, UIITemSlot _uiItemSlot)
    { 
        blockID = _blockID;
        uiItemSlot = _uiItemSlot;
        uiItemSlot.Link(this);
    }

    public void LinkUISlot(UIITemSlot uiSlot)
    {
        uiItemSlot = uiSlot;
    }

    public void unLinkUISlot()
    {
        uiItemSlot = null;
    }

    public void EmptySlot()
    {
        blockID = 0;
        if (uiItemSlot != null)
            uiItemSlot.UpdateSlot();
    }

    public bool HasItem
    {
        get
        {
            if (blockID != 0)
                return true;
            else
                return false;
        }
    }
}