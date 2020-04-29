using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/* 화면에 보여지는 인벤토리와 툴바를 다루는 클래스 */
public class UIItemSlot : MonoBehaviour
{
    World world;                                                        // 월드 오브젝트

    public bool isLinked = false;                                       // ItemSlot과 연결되었나 여부
    public ItemSlot itemSlot;                                           // ItemSlot 클래스
    public Image slotImage;                                             // 슬롯칸 이미지
    public Image slotIcon;                                              // 슬롯 내 아이템 아이콘 이미지
    public Text slotAmount;                                             // 슬롯 내 아이템의 총량 텍스트


    private void Awake()
    {
        world = GameObject.Find("World").GetComponent<World>();
    }


    public void Link (ItemSlot _itemSlot)
    {
        itemSlot = _itemSlot;
        isLinked = true;
        itemSlot.LinkUISlot(this);
        UpdateSlot();
    }                             // ItemSlot 클래스와 링크

    public void UnLink()
    {
        itemSlot.unLinkUISlot();
        itemSlot = null;
        UpdateSlot();
    }                                              // 링크 해제

   

    /* 인벤토리의 슬롯 다루는 함수 */
    public void UpdateSlot()
    {
        if (itemSlot != null && itemSlot.HasItem)
        {
            slotIcon.sprite = world.blockTypes[itemSlot.stack.id].icon;
            slotAmount.text = itemSlot.stack.amount.ToString();
            slotIcon.enabled = true;
            slotAmount.enabled = true;
        }
        else
            Clear();
    }                                          // 슬롯에 아이템 업데이트

    public void Clear()
    {
        slotIcon.sprite = null;
        slotAmount.text = "";
        slotIcon.enabled = false;
        slotAmount.enabled = false;
    }                                               // 슬롯 내 아이템 삭제

    private void OnDestroy()
    {
        if (itemSlot != null)
            itemSlot.unLinkUISlot();
    }                                          // 아이템 슬롯이 비었을 시 링크 해제

    public bool HasItem
    {
        get
        {
            if (itemSlot == null)
                return false;
            else
                return itemSlot.HasItem;
        }
    }                                               // 아이템 보유 여부 반환
}


/* 인벤토리와 툴바 내 아이템의 데이터를 다루는 클래스 */
public class ItemSlot
{
    public ItemStack stack = null;
    private UIItemSlot uiItemSlot = null;

    public bool isCreative;


    public ItemSlot(UIItemSlot _uiItemSlot)
    {
        stack = null;
        uiItemSlot = _uiItemSlot;
        uiItemSlot.Link(this);
    }                           // UIItemSlot 클래스와 링크

    public ItemSlot(UIItemSlot _uiItemSlot, ItemStack _stack)
    {
        stack = _stack;
        uiItemSlot = _uiItemSlot;
        uiItemSlot.Link(this);
    }         // UIItemSlot 클래스와 링크

    public void LinkUISlot (UIItemSlot uiSlot)
    {
        uiItemSlot = uiSlot;
    }                        // UIItemSlot 클래스와 링크

    public void unLinkUISlot()
    {
        uiItemSlot = null;
    }                                        // UIItemSlot 클래스와 링크 해제

    public void EmptySlot()
    {
        stack = null;
        if (uiItemSlot != null)
            uiItemSlot.UpdateSlot();
    }                                           // 슬롯을 비우는 함수



    public void InsertStack (ItemStack _stack)
    {
        stack = _stack;
        uiItemSlot.UpdateSlot();
    }                        // UI에 아이템을 넣는 함수

    public ItemStack TakeAll()
    {
        ItemStack handOver = new ItemStack(stack.id, stack.amount);
        EmptySlot();
        return handOver;
    }                                        // UI에서 선택한 아이템을 전부 빼는 함수

    public int Take(int amt)
    {
        if (amt > stack.amount)
        {
            int _amt = stack.amount;
            EmptySlot();
            return _amt;
        }
        else if (amt < stack.amount)
        {
            stack.amount -= amt;
            uiItemSlot.UpdateSlot();
            return amt;
        }
        else
        {
            EmptySlot();
            return amt;
        }
    }                                          // UI에서 선택한 아이템을 amt 양 만큼 빼는 함수
    
    public bool HasItem
    {
        get
        {
            if (stack != null)
                return true;
            else
                return false;
        }
    }                                               // 아이템 보유 여부 반환
}
