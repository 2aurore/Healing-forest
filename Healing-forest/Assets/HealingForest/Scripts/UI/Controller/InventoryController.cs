using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class InventoryController : MonoBehaviour
    {
        private InventoryUI inventoryUI;
        private bool isInitialized = false;

        private void Start()
        {
            InputSystem.Singleton.OnTab += OnInventoryUI;
        }

        private void OnEnable()
        {
            UserDataModel.Singleton.OnInventoryDataChanged += OnInventoryDataChanged;
            UserDataModel.Singleton.OnEquipmentChanged += OnEquipmentChanged;

        }

        private void OnDisable()
        {
            if (UserDataModel.Singleton != null)
            {
                UserDataModel.Singleton.OnInventoryDataChanged -= OnInventoryDataChanged;
                UserDataModel.Singleton.OnEquipmentChanged -= OnEquipmentChanged;
            }
        }

        private void OnInventoryUI()
        {
            EnsureInitialized();

            if (inventoryUI != null)
            {
                if (inventoryUI.gameObject.activeSelf)
                {
                    HideInventory();
                }
                else
                {
                    ShowInventory();
                }
            }

        }

        /// <summary> 장착 아이템이 변경되는 경우 (UserDataModel 이벤트로 호출) </summary>
        private void OnEquipmentChanged(string previousItemID, string newItemID)
        {
            if (isInitialized && inventoryUI != null)
            {
                Debug.Log($"InventoryController: Equipment changed from {previousItemID} to {newItemID}");
                inventoryUI.UpdateAllEquipmentStatus();
            }
        }

        /// <summary> 인벤토리 UI가 필요할 때 초기화 (Lazy Initialization) </summary>
        public void EnsureInitialized()
        {
            if (isInitialized) return;

            // UI가 아직 생성되지 않았다면 생성
            inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI); // UIList에 맞게 수정
            if (inventoryUI == null)
            {
                Debug.LogError("InventoryController: InventoryUI is not initialized.");
                return;
            }

            InitializeInventory();
            isInitialized = true;

        }

        private void ShowInventory()
        {
            if (inventoryUI != null)
            {
                inventoryUI.Show();
                RefreshInventoryUI(); // 최신 데이터로 갱신
            }

            UIManager.Hide<IngameUI>(UIList.IngameUI);
        }

        private void HideInventory()
        {
            if (inventoryUI != null)
            {
                inventoryUI.Hide();
            }

            UIManager.Show<IngameUI>(UIList.IngameUI);
        }

        private void InitializeInventory()
        {
            InventoryDTO inventoryData = UserDataModel.Singleton.InventoryData;
            int maxSlots = UserDataModel.Singleton.MaxInventorySlots;

            // InventoryUI에게 슬롯 생성 요청
            List<Inventory_Item> inventorySlots = inventoryUI.CreateInventorySlots(maxSlots);

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

        /// <summary>
        /// UI 데이터 새로고침 (슬롯 재생성 없이)
        /// </summary>
        private void RefreshInventoryUI()
        {
            if (!isInitialized || inventoryUI == null) return;

            InventoryDTO inventoryData = UserDataModel.Singleton.InventoryData;

            // 모든 슬롯의 장착 상태 업데이트
            inventoryUI.UpdateAllEquipmentStatus();
        }

        // UserDataModel에서 데이터가 변경되면 호출됨
        private void OnInventoryDataChanged(UserItemDataDTO changedItem)
        {
            if (!isInitialized || inventoryUI == null) return;

            // UI에서 해당 아이템 ID를 가진 슬롯 찾기
            Inventory_Item targetSlot = inventoryUI.FindSlotByItemID(changedItem.itemID);

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
                Inventory_Item emptySlot = inventoryUI.FindEmptySlot();
                if (emptySlot != null)
                {
                    var itemDataSO = GameDataModel.Singleton.GetItemData(changedItem.itemID);
                    emptySlot.Initialize(changedItem.itemID, changedItem.itemCount, itemDataSO.Icon);
                    emptySlot.UpdateEqquippedImage(changedItem.itemID);
                }
            }
        }

        // 장착 아이템이 변경되는 경우
        private void OnEquipmentChanged(string newEquipItemID)
        {
            if (isInitialized && inventoryUI != null)
            {
                inventoryUI.UpdateAllEquipmentStatus();
            }
        }
    }
}