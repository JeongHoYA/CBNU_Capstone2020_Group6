using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;                                                                                           // 아이템 오브젝트

    public override void Interact()
    {
        base.Interact();

        PickUp();
    }

    void PickUp()
    {
        Debug.Log("Picking up " + item.name);

        bool wasPickedUp = Inventory.instance.Add(item);
        if(wasPickedUp)
            Destroy(gameObject);
    }                                                                                            // 아이템을 인벤토리에 추가하고 해당 오브젝트를 destroy하는 함수
}
