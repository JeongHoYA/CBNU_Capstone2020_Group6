using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject                                                            // 아이템 클래스       
{
    new public string name = "New Item";                                                        // 아이템의 이름
    public Sprite icon = null;                                                                  // 아이템 아이콘
    public bool isDefaultItem = false;                                                          // 아이템이 비어있나 여부


    public virtual void Use()
    {
        Debug.Log("Using " + name);
    }                                                                // 아이템 사용 시 호출되는 버추얼 함수

    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }                                                        // 아이템을 인벤토리에서 없애는 함수
}
