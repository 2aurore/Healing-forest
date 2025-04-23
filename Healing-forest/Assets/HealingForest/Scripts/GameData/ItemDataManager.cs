using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class ItemDataManager : SingletonBase<ItemDataManager>
    {
        // 아이템 데이터 리스트
        [SerializeField] private List<ItemData> itemDataList = new List<ItemData>();

        private string FILE_PATH = "Assets/Resources/Data/item.txt"; // 아이템 데이터 파일 경로


        public void Initialize()
        {
            LoadItemData(); // 아이템 데이터 불러오기
        }


        // TODO: 아이템 데이터 불러오기
        public void LoadItemData()
        {
            if (FileManager.ReadFileData(FILE_PATH, out string itemDataString))
            {
                List<ItemData> datas = JsonUtility.FromJson<List<ItemData>>(itemDataString);
                itemDataList = datas;
            }
        }


        // TODO: 아이템 데이터 저장하기
        public void SaveItemData()
        {
            string json = JsonUtility.ToJson(itemDataList, true);
            FileManager.WriteFileFromString(FILE_PATH, json);
        }

        // 아이템 데이터 가져오기
        public ItemData GetItemData(string itemID)
        {
            return itemDataList.Find(item => item.ItemID == itemID);
        }

        // 아이템 데이터 추가하기
        public void AddItemData(ItemData newItemData)
        {
            itemDataList.Add(newItemData);

            SaveItemData();
        }

        // 아이템 데이터 삭제하기
        public void RemoveItemData(string itemID)
        {
            ItemData itemToRemove = GetItemData(itemID);
            if (itemToRemove != null)
            {
                itemDataList.Remove(itemToRemove);
            }

            SaveItemData();
        }
    }

}
