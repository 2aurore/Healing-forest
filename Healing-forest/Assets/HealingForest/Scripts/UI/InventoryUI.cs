using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HF
{
    public class InventoryUI : UIBase
    {
        public static InventoryUI Instance => UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);

        [SerializeField] private Transform inventoryItemsParent;
        [SerializeField] private GameObject inventoryItemPrefab;

        [SerializeField] private List<Inventory_Item> inventorySlots = new List<Inventory_Item>();

        /// <summary>
        /// 인벤토리 슬롯들을 생성하고 리스트로 반환
        /// </summary>
        public List<Inventory_Item> CreateInventorySlots(int slotCount)
        {
            // 기존 슬롯들 정리
            ClearAllSlots();

            // 새로운 슬롯들 생성
            for (int i = 0; i < slotCount; i++)
            {
                GameObject slotObject = Instantiate(inventoryItemPrefab, inventoryItemsParent);
                Inventory_Item inventorySlot = slotObject.GetComponent<Inventory_Item>();
                inventorySlots.Add(inventorySlot);
            }

            return inventorySlots;
        }

        /// <summary>
        /// 특정 아이템 ID를 가진 슬롯을 찾기
        /// </summary>
        public Inventory_Item FindSlotByItemID(string itemID)
        {
            return inventorySlots.Find(slot => slot.ItemID == itemID);
        }

        /// <summary>
        /// 빈 슬롯 찾기
        /// </summary>
        public Inventory_Item FindEmptySlot()
        {
            return inventorySlots.Find(slot => string.IsNullOrEmpty(slot.ItemID));
        }

        /// <summary>
        /// 모든 슬롯 정리
        /// </summary>
        private void ClearAllSlots()
        {
            foreach (Transform child in inventoryItemsParent)
            {
                Destroy(child.gameObject);
            }
            inventorySlots.Clear();
        }

        /// <summary>
        /// 인벤토리 닫기
        /// </summary>
        public void CloseInventory()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 모든 슬롯의 장착 상태 업데이트 (장착 아이템이 변경되었을 때)
        /// </summary>
        public void UpdateAllEquipmentStatus()
        {
            foreach (var slot in inventorySlots)
            {
                if (!string.IsNullOrEmpty(slot.ItemID))
                {
                    slot.UpdateEqquippedImage(slot.ItemID);
                }
            }
        }
    }
}
