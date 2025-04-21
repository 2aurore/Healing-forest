using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class ItemDataManager : SingletonBase<ItemDataManager>
    {
        // 아이템 데이터 리스트
        [SerializeField] private List<ItemData> itemDataList = new List<ItemData>();

        // TODO: 아이템 데이터 불러오기

        // TODO: 아이템 데이터 저장하기

        // 아이템 데이터 가져오기
        public ItemData GetItemData(int itemID)
        {
            return itemDataList.Find(item => item.ItemID == itemID);
        }

        // 아이템 데이터 추가하기
        public void AddItemData(ItemData newItemData)
        {
            itemDataList.Add(newItemData);
        }

        // 아이템 데이터 삭제하기
        public void RemoveItemData(int itemID)
        {
            ItemData itemToRemove = GetItemData(itemID);
            if (itemToRemove != null)
            {
                itemDataList.Remove(itemToRemove);
            }
        }
    }

}
