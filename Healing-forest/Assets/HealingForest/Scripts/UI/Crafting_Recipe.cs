using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HF
{
    public class Crafting_Recipe : MonoBehaviour
    {
        [SerializeField] private Image itemIcon; // 레시피 아이콘을 설정할 수 있는 필드
        [SerializeField] private TextMeshProUGUI reciptNameText; // 레시피 이름을 표시할 텍스트
        [SerializeField] private GameObject materialItemsParent; // 레시피 재료 아이템들을 담을 부모 오브젝트
        [SerializeField] private GameObject materialSlotPrefab; // 재료 슬롯 프리팹
        private List<Crafting_Recipe_Material> materialSlots = new List<Crafting_Recipe_Material>();
        private string currentRecipeID; // 현재 레시피 ID

        public void SetRecipeData(string reciptID)
        {
            currentRecipeID = reciptID;
            RecipeDataSO reciptData = GameDataModel.Singleton.GetRecipeData(reciptID);
            if (reciptData != null)
            {
                // 레시피 아이콘과 이름 설정
                ItemDataSO itemDataSO = GameDataModel.Singleton.GetItemData(reciptData.ResultItemId);
                itemIcon.sprite = itemDataSO.Icon;
                reciptNameText.text = reciptData.RecipeName;

                // 재료 슬롯 초기화
                foreach (Transform child in materialItemsParent.transform)
                {
                    Destroy(child.gameObject);
                }
                materialSlots.Clear();

                // 재료 슬롯 생성
                foreach (RequiredItem material in reciptData.RequiredItems)
                {
                    // 현재 내가 가지고 있는 재료 아이템
                    UserItemDataDTO userItemData = UserDataModel.Singleton.GetInventoryItemData(material.ItemId);
                    GameObject slotObject = Instantiate(materialSlotPrefab, materialItemsParent.transform);
                    Crafting_Recipe_Material materialSlot = slotObject.GetComponent<Crafting_Recipe_Material>();
                    materialSlot.SetMaterialSlot(material.ItemId, userItemData != null ? userItemData.itemCount : 0, material.Quantity);
                    materialSlots.Add(materialSlot);
                }
            }
        }
        /// <summary>
        /// 레시피 아이템 클릭 시 호출되는 메서드
        /// </summary>
        public void OnRecipeClicked()
        {
            // 재료가 충분한지 확인
            if (CanCraft())
            {
                // CraftingUI에 선택된 레시피 정보 전달하고 확인창 활성화
                CraftingUI craftingUI = GetComponentInParent<CraftingUI>();
                craftingUI?.ShowConfirmDialog(currentRecipeID);
            }
            else
            {
                Debug.Log("재료가 부족합니다!");
                // 재료 부족 알림 UI 표시 (필요시)
            }
        }

        /// <summary>
        /// 제작 가능한지 확인
        /// </summary>
        public bool CanCraft()
        {
            RecipeDataSO reciptData = GameDataModel.Singleton.GetRecipeData(currentRecipeID);
            if (reciptData == null) return false;

            foreach (RequiredItem material in reciptData.RequiredItems)
            {
                UserItemDataDTO userItemData = UserDataModel.Singleton.GetInventoryItemData(material.ItemId);
                int currentAmount = userItemData != null ? userItemData.itemCount : 0;

                if (currentAmount < material.Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 특정 아이템의 수량이 변경되었을 때 해당 재료 슬롯만 업데이트
        /// </summary>
        public void UpdateMaterialAmount(string itemID, int newAmount)
        {
            Crafting_Recipe_Material targetSlot = materialSlots.Find(slot => slot.ItemID == itemID);
            if (targetSlot != null)
            {
                targetSlot.UpdateAmount(newAmount);
            }
        }

        /// <summary>
        /// 모든 재료 슬롯의 수량을 최신 데이터로 업데이트
        /// </summary>
        public void RefreshAllMaterialAmounts()
        {
            foreach (var materialSlot in materialSlots)
            {
                UserItemDataDTO userItemData = UserDataModel.Singleton.GetInventoryItemData(materialSlot.ItemID);
                int currentAmount = userItemData != null ? userItemData.itemCount : 0;
                materialSlot.UpdateAmount(currentAmount);
            }
        }


    }
}
