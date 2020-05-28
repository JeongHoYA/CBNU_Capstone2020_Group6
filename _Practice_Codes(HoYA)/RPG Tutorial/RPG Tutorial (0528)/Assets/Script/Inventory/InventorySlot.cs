using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;                                                                              // 아이콘 이미지
    Item item;                                                                                      // 아이템 오브젝트 클래스

    public Button removeButton;                                                                     // 아이템 삭제 버튼


    public void AddItem (Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }                                                           // 추가된 아이템의 아이콘을 보여주는 함수

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }                                                                      // 사라진 아이템의 아이콘을 없애는 함수

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }                                                                 // Remove버튼 클릭시 해당 아이템 삭제

    public void UseItem()
    {
        if(item != null)
        {
            item.Use();
        }
    }                                                                        // 아이템 버튼 클릭시 사용
}
