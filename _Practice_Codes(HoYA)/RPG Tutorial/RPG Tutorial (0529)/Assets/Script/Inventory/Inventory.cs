using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Inventory : MonoBehaviour
{
    // 인벤토리 클래스 싱글톤화
    #region Singletone

    public static Inventory instance;                                                                           // 인벤토리의 싱글톤을 위한 변수

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found");
            return;
        }

        instance = this;
    }

    #endregion                                                                                                  // 인벤토리 싱글톤화                                                                                                  // 인벤토리 싱글톤화

    public int space = 20;                                                                                      // 인벤토리 아이템 갯수 제한
    public List<Item> items = new List<Item>();                                                                 // 인벤토리 내 아이템 리스트


    public delegate void OnItemChanged();                                                                       // 인벤토리 델리게이트 함수
    public OnItemChanged onItemChangedCallback;




    public bool Add (Item item)
    {
        if(!item.isDefaultItem)
        {
            if(items.Count >= space)
            {
                Debug.Log("Not enough space");
                return false;
            }
            items.Add(item);
            
            if(onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }
        return true;
    }                                                                              // 아이템 추가 함수

    public void Remove(Item item)
    {
        items.Remove(item);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }                                                                            // 아이템 삭제 함수
}
