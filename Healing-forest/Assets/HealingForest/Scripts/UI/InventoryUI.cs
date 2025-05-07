using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class InventoryUI : UIBase
    {

        [SerializeField] private Transform inventoryItemsParent; // UI 항목들의 부모 요소
        [SerializeField] private GameObject inventoryItemPrefab; // 인벤토리 아이템 UI 프리팹

        private void OnEnable()
        {
            // 이벤트 구독

            UserDataModel.Singleton.OnInventoryDataChanged += UpdateItemUI;
        }

        private void OnDisable()
        {
            // 이벤트 구독 해제

            UserDataModel.Singleton.OnInventoryDataChanged -= UpdateItemUI;
        }

        private void UpdateItemUI(UserItemDataDTO itemData)
        {
            // 인벤토리 UI 업데이트 로직

        }
    }
}
