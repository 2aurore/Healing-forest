using System;
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

        private void Start()
        {
            // 인벤토리 UI 초기화
            InitializeInventoryUI();
        }

        private void InitializeInventoryUI()
        {
            // 인벤토리 UI 초기화 로직
            // 기존 자식 오브젝트 모두 제거
            foreach (Transform child in inventoryItemsParent)
            {
                Destroy(child.gameObject);
            }

            int slotCount = UserDataModel.Singleton.MaxInventorySlots;
            var inventoryItems = UserDataModel.Singleton.InventoryData.InventoryItems;

            // 아이템이 들어있는 슬롯 먼저 생성
            foreach (var itemData in inventoryItems)
            {
                GameObject itemUI = Instantiate(inventoryItemPrefab, inventoryItemsParent);
                Inventory_Item inventoryItem = itemUI.GetComponent<Inventory_Item>();
                ItemDataSO itemDataSO = GameDataModel.Singleton.GetItemData(itemData.itemID);
                inventoryItem.Initialize(itemData.itemID, itemData.itemCount, itemDataSO.Icon);
            }

            // 남은 빈 슬롯 생성
            int emptySlotCount = slotCount - inventoryItems.Count;
            for (int i = 0; i < emptySlotCount; i++)
            {
                GameObject itemUI = Instantiate(inventoryItemPrefab, inventoryItemsParent);
                Inventory_Item inventoryItem = itemUI.GetComponent<Inventory_Item>();
                inventoryItem.InitializeEmpty(); // 빈 슬롯으로 초기화
            }
        }

        private void UpdateItemUI(UserItemDataDTO itemData)
        {
            // 인벤토리 UI 업데이트 로직

            Inventory_Item[] inventory_Items = inventoryItemsParent.GetComponentsInChildren<Inventory_Item>();
            foreach (var inventoryItem in inventory_Items)
            {
                // 인벤토리 아이템 UI에서 아이템 ID와 일치하는 아이템을 찾습니다.
                if (inventoryItem.ItemID == itemData.itemID)
                {
                    // 아이템 수량 업데이트
                    inventoryItem.UpdateItemCount(itemData.itemCount);
                    break;
                }
            }
        }

        public void CloseInventory()
        {
            // 인벤토리 UI 닫기
            gameObject.SetActive(false);
        }
    }
}
