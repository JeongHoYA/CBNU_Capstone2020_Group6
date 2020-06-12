using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIITemSlot : MonoBehaviour
{
    public World world;                                                                                       // 월드 오브젝트

    public bool isLinked = false;                                                                             // 연결 여부
    public ItemSlot itemSlot;                                                                                 // 아이템슬롯 클래스
    public Image slotImage;                                                                                   // 슬롯 이미지(자기 자신)
    public Image slotIcon;                                                                                    // 슬롯 아이콘


    public bool HasItem
    {
        get
        {
            if (itemSlot == null)
                return false;
            else
                return itemSlot.HasItem;
        }
    }                                                                                    // 아이템 보유 여부

    public void Link(ItemSlot _itemSlot)
    {
        itemSlot = _itemSlot;
        isLinked = true;
        itemSlot.LinkUISlot(this);
        UpdateSlot();
    }                                                                   // ItemSlot과 연결

    public void UnLink()
    {
        itemSlot.unLinkUISlot();
        itemSlot = null;
        UpdateSlot();
    }                                                                                   // ItemSlot과 연결 해제

    public void UpdateSlot()
    {
        if (itemSlot != null && itemSlot.HasItem)
        {
            slotIcon.sprite = world.blocks[itemSlot.blockID].icon;
            slotIcon.enabled = true;
        }
        else
            Clear();
    }                                                                               // 슬롯 업데이트

    public void Clear()
    {
        slotIcon.sprite = null;
        slotIcon.enabled = false;
    }                                                                                    // 슬롯 클리어

    private void OnDestroy()
    {
        if (isLinked)
            itemSlot.unLinkUISlot();
    }                                                                               // 슬롯 onDestroy
}



public class ItemSlot
{
    public int blockID = 0;                                                                                    // 블록ID
    private UIITemSlot uiItemSlot = null;                                                                      // UIItemSlot 클래스
    public bool isInventory;                                                                                   // 인벤토리 슬롯 여부


    public ItemSlot (UIITemSlot _uiItemSlot)
    {
        blockID = 0;
        uiItemSlot = _uiItemSlot;
        uiItemSlot.Link(this);
    }                                                                // 생성자

    public ItemSlot(int _blockID, UIITemSlot _uiItemSlot)
    { 
        blockID = _blockID;
        uiItemSlot = _uiItemSlot;
        uiItemSlot.Link(this);
    }                                                   // 생성자


    public void LinkUISlot(UIITemSlot uiSlot)
    {
        uiItemSlot = uiSlot;
    }                                                               // UIItemSlot 연결

    public void unLinkUISlot()
    {
        uiItemSlot = null;
    }                                                                              // UIItemSlot 연결 해제

    public void Insert(int _blockID)
    {
        blockID = _blockID;
        uiItemSlot.UpdateSlot();
    }                                                                        // 슬롯에 아이템 넣기

    public int Take()
    {
        int handover = blockID;
        EmptySlot();
        return handover;
    }                                                                                       // 슬롯에서 아이템 빼기

    public void EmptySlot()
    {
        blockID = 0;
        if (uiItemSlot != null)
            uiItemSlot.UpdateSlot();
    }                                                                                 // 슬롯 비우기

    public bool HasItem
    {
        get
        {
            if (blockID != 0)
                return true;
            else
                return false;
        }
    }                                                                                     // 아이템 보유 여부
}