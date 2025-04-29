using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    [System.Serializable]
    public class UserDataDTO { }

    [System.Serializable]
    public class UserItemDataDTO : UserDataDTO
    {
        public string uniqueID; // UserData - ID
        public string itemID;   // GameData - ID
        public int itemCount;
        public float itemDurability;   // 아이템 내구도
    }

    [System.Serializable]
    public class InventoryDTO : UserDataDTO
    {
        public List<UserItemDataDTO> InventoryItems = new List<UserItemDataDTO>(40); // 유저 아이템 데이터 리스트
    }
}
