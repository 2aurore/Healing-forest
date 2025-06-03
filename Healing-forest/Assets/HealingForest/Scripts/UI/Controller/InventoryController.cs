using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    // InventoryController.cs
    public class InventoryController : MonoBehaviour
    {

        private void OnEnable()
        {
            // UserDataModel의 변경사항을 구독
            UserDataModel.Singleton.OnInventoryDataChanged += OnInventoryDataChanged;
        }

        private void OnDisable()
        {
            UserDataModel.Singleton.OnInventoryDataChanged -= OnInventoryDataChanged;
        }

        public void Initialize()
        {
            Debug.Log("InventoryController Start");
            InitializeInventory();
        }

        private void InitializeInventory()
        {
            InventoryDTO inventoryData = UserDataModel.Singleton.InventoryData;
            int maxSlots = UserDataModel.Singleton.MaxInventorySlots;

            // InventoryUI에게 슬롯 생성 요청
            List<Inventory_Item> inventorySlots = InventoryUI.Instance.CreateInventorySlots(maxSlots);

            // 각 슬롯에 데이터 설정
            int slotIndex = 0;

            // 아이템이 있는 슬롯들 초기화
            foreach (var itemData in inventoryData.InventoryItems)
            {
                if (slotIndex < inventorySlots.Count)
                {
                    var itemDataSO = GameDataModel.Singleton.GetItemData(itemData.itemID);
                    inventorySlots[slotIndex].Initialize(itemData.itemID, itemData.itemCount, itemDataSO.Icon);

                    // 장착된 아이템 표시
                    inventorySlots[slotIndex].UpdateEqquippedImage(itemData.itemID);

                    slotIndex++;
                }
            }

            // 나머지 빈 슬롯들 초기화
            for (int i = slotIndex; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].InitializeEmpty();
            }
        }

        // UserDataModel에서 데이터가 변경되면 호출됨
        private void OnInventoryDataChanged(UserItemDataDTO changedItem)
        {
            // UI에서 해당 아이템 ID를 가진 슬롯 찾기
            Inventory_Item targetSlot = InventoryUI.Instance.FindSlotByItemID(changedItem.itemID);

            if (targetSlot != null)
            {
                // 아이템 개수 업데이트
                targetSlot.UpdateItemCount(changedItem.itemCount);

                // 장착 상태 업데이트
                targetSlot.UpdateEqquippedImage(changedItem.itemID);
            }
            else
            {
                // 새로운 아이템이 추가된 경우 - 빈 슬롯을 찾아서 설정
                Inventory_Item emptySlot = InventoryUI.Instance.FindEmptySlot();
                if (emptySlot != null)
                {
                    var itemDataSO = GameDataModel.Singleton.GetItemData(changedItem.itemID);
                    emptySlot.Initialize(changedItem.itemID, changedItem.itemCount, itemDataSO.Icon);
                    emptySlot.UpdateEqquippedImage(changedItem.itemID);
                }
            }
        }

        //  장착 아이템이 변경되는 경우
        private void OnEquipmentChanged(string newEquipItemID)
        {
            InventoryUI.Instance.UpdateAllEquipmentStatus();
        }
    }
}
