using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDropHandler : MonoBehaviour
{
    World world;

    [SerializeField] private UIItemSlot cursorSlot = null;                              // 아이템 선택을 위한 슬롯
    private ItemSlot cursorItemSlot;

    [SerializeField] private GraphicRaycaster m_RayCaster = null;                       // 마우스 선택을 위한 RayCaster
    private PointerEventData m_PointerEventData;

    [SerializeField] private EventSystem m_EventSystem = null;                          // 마우스 이벤트 검출을 위한 변수


    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        cursorItemSlot = new ItemSlot(cursorSlot);
    }

    private void Update()
    {
        if (!world.inUI)
            return;

        cursorSlot.transform.position = Input.mousePosition;
        cursorSlot.ShowBlockName();

        if (Input.GetMouseButtonDown(0))
        {
            HandleSlotClick(CheckForSlot());
        }
    }


    private void HandleSlotClick (UIItemSlot clickedSlot)
    {
        // clickedSlot = 플레이어가 클릭한 아이템 / cursorItemSlot = 현재 커서에 묶여있는 아이템

        if (clickedSlot == null)
        {
            cursorItemSlot.EmptySlot();
            return;
        }
            
        if (!cursorSlot.HasItem && !clickedSlot.HasItem)
            return;
        
        if (clickedSlot.itemSlot.isCreative)
        {
            cursorItemSlot.EmptySlot();
            cursorItemSlot.InsertStack(clickedSlot.itemSlot.stack);
        }

        if (!cursorSlot.HasItem && clickedSlot.HasItem)
        {
            cursorItemSlot.InsertStack(clickedSlot.itemSlot.TakeAll());
            cursorSlot.UpdateSlot();
            return;
        }

        if (cursorSlot.HasItem && !clickedSlot.HasItem)
        {
            clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll());
            clickedSlot.UpdateSlot();
            return;
        }

        if (cursorSlot.HasItem && clickedSlot.HasItem)
        {
            ItemStack oldCursorSlot = cursorSlot.itemSlot.TakeAll();
            ItemStack oldSlot = clickedSlot.itemSlot.TakeAll();

            clickedSlot.itemSlot.InsertStack(oldCursorSlot);
            cursorSlot.itemSlot.InsertStack(oldSlot);
        }
    }                           // 마우스 클릭시 할 행동 함수


    private UIItemSlot CheckForSlot()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        m_RayCaster.Raycast(m_PointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.tag == "UIItemSlot")
                return result.gameObject.GetComponent<UIItemSlot>();
        }
        return null;
    }                                               // 아이템 좌표 파악 함수
}
