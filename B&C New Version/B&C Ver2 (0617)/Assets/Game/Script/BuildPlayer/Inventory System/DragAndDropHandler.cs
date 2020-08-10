using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDropHandler : MonoBehaviour
{
    public World world;

    [SerializeField] private UIITemSlot cursorSlot = null;                              // 아이템 선택을 위한 슬롯
    private ItemSlot cursorItemslot;

    [SerializeField] private GraphicRaycaster m_Raycaster = null;                       // 마우스 선택을 위한 RayCaster
    private PointerEventData m_PointerEventData;

    [SerializeField] private EventSystem m_EvnetSystem = null;                          // 마우스 이벤트 검출을 위한 변수

    private void Start()
    {
        cursorItemslot = new ItemSlot(cursorSlot);
    }

    private void Update()
    {
        if (world.inUI && !world.inPause)
        {
            cursorSlot.transform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                HandleSlotClick(CheckForslot());
            }
        }
    }





    private void HandleSlotClick(UIITemSlot clickedSlot)
    {
        // clickedSlot = 플레이어가 클릭한 아이템 / cursorItemSlot = 현재 커서에 묶여있는 아이템

        if (clickedSlot == null)
        {
            cursorItemslot.EmptySlot();
            return;
        }

        if (clickedSlot.itemSlot.isInventory)
        {
            cursorItemslot.EmptySlot();
            cursorItemslot.Insert(clickedSlot.itemSlot.blockID);
        }

        if (!cursorSlot.HasItem && !clickedSlot.HasItem)
            return;

        if (!cursorSlot.HasItem && clickedSlot.HasItem)
        {
            cursorItemslot.Insert(clickedSlot.itemSlot.Take());
            return;
        }

        if (cursorSlot.HasItem && !clickedSlot.HasItem)
        {
            clickedSlot.itemSlot.Insert(cursorItemslot.Take());
            return;
        }

        if (cursorSlot.HasItem && clickedSlot.HasItem)
        {
            if (cursorSlot.itemSlot.blockID != clickedSlot.itemSlot.blockID)
            {
                int temp1 = cursorSlot.itemSlot.Take();
                int temp2 = clickedSlot.itemSlot.Take();

                clickedSlot.itemSlot.Insert(temp1);
                cursorSlot.itemSlot.Insert(temp2);
            }
        }
    }                            // 마우스 클릭시 할 행동 함수


    private UIITemSlot CheckForslot()
    {
        m_PointerEventData = new PointerEventData(m_EvnetSystem);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        foreach(RaycastResult result in results)
        {
            if (result.gameObject.tag == "UIItemSlot")
                return result.gameObject.GetComponent<UIITemSlot>();
        }

        return null;
    }                                               // 아이템 좌표 파악 함수


}
