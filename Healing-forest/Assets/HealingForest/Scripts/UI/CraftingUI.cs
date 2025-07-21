using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HF
{
    public class CraftingUI : UIBase
    {

        [SerializeField] private Transform reciptItemsParent;
        [SerializeField] private GameObject reciptItemPrefab;

        [Header("Confirm Dialog")]
        [SerializeField] private GameObject confirmDialog; // 확인 대화상자
        [SerializeField] private Button confirmOkButton; // OK 버튼
        [SerializeField] private Button confirmCancelButton; // Cancel 버튼
        [SerializeField] private TextMeshProUGUI confirmText; // 확인 텍스트


        private List<Crafting_Recipe> reciptSlots = new List<Crafting_Recipe>();
        private string selectedRecipeID; // 선택된 레시피 ID
        private float craftingTime; // 제작 시간 (초 단위)

        private void OnEnable()
        {
            EventSystem.OnCraftingCompleted += CraftItem; // 제작 완료 시 UI 닫기
        }
        private void OnDisable()
        {
            EventSystem.OnCraftingCompleted -= CraftItem; // 제작 완료 시 UI 닫기
        }

        public void Initialize()
        {

            ClearAllSlots();

            RecipeDataDTO reciptDataDto = GameDataModel.Singleton.RecipeDataDTO;
            if (reciptDataDto != null && reciptDataDto.reciptDatas != null && reciptDataDto.reciptDatas.Count > 0)
            {
                foreach (var recipe in reciptDataDto.reciptDatas.Values)
                {
                    GameObject slotObject = Instantiate(reciptItemPrefab, reciptItemsParent);
                    Crafting_Recipe reciptSlot = slotObject.GetComponent<Crafting_Recipe>();
                    reciptSlot.SetRecipeData(recipe.RecipeID);
                    reciptSlots.Add(reciptSlot);
                }
            }
            else
            {
                Debug.LogWarning("No recipe data found.");
            }
        }
        /// <summary>
        /// 레시피 선택 시 확인 대화상자 표시
        /// </summary>
        public void ShowConfirmDialog(string reciptID)
        {
            selectedRecipeID = reciptID;
            RecipeDataSO reciptData = GameDataModel.Singleton.GetRecipeData(reciptID);

            if (reciptData != null && confirmDialog != null)
            {
                // 확인창 정보 설정
                ItemDataSO resultItem = GameDataModel.Singleton.GetItemData(reciptData.ResultItemId);
                if (resultItem != null)
                {
                    confirmText.text = $"{resultItem.ItemName}을(를) 제작하시겠습니까?";
                    craftingTime = reciptData.CraftingTime;
                }

                // 확인창 활성화
                confirmDialog.SetActive(true);
            }
        }

        /// <summary>
        /// 확인 버튼 클릭 시 호출
        /// </summary>
        public void OnConfirmOk()
        {
            if (!string.IsNullOrEmpty(selectedRecipeID))
            {
                // CraftItem();
                EventSystem.OnCraftingStarted?.Invoke(craftingTime);
            }

            // 확인창 닫기
            confirmDialog.SetActive(false);
        }

        /// <summary>
        /// 취소 버튼 클릭 시 호출
        /// </summary>
        public void OnConfirmCancel()
        {
            // 확인창 닫기
            confirmDialog.SetActive(false);
            selectedRecipeID = null;
        }

        /// <summary>
        /// 아이템 제작 실행
        /// </summary>
        private void CraftItem()
        {
            RecipeDataSO reciptData = GameDataModel.Singleton.GetRecipeData(selectedRecipeID);
            if (reciptData == null)
            {
                Debug.LogError($"Recipe data not found for ID: {selectedRecipeID}");
                return;
            }

            // 재료 소모
            foreach (RequiredItem material in reciptData.RequiredItems)
            {
                UserDataModel.Singleton.RemoveInventoryItem(material.ItemId, material.Quantity);
            }

            // 결과 아이템 추가
            UserDataModel.Singleton.AddInventoryItem(reciptData.ResultItemId, 1);

            // UI 업데이트
            RefreshAllMaterialAmounts();

            Debug.Log($"Successfully crafted: {reciptData.RecipeName}");

            // 인벤토리 UI 업데이트 (필요시)
            EventSystem.OnInventoryChanged?.Invoke();
        }


        /// <summary>
        /// 특정 아이템 수량이 변경되었을 때 모든 레시피의 해당 재료 슬롯 업데이트
        /// </summary>
        public void UpdateMaterialAmounts(string itemID, int newAmount)
        {
            foreach (var reciptSlot in reciptSlots)
            {
                reciptSlot.UpdateMaterialAmount(itemID, newAmount);
            }
        }

        /// <summary>
        /// 모든 레시피의 재료 수량을 최신 데이터로 새로고침
        /// </summary>
        public void RefreshAllMaterialAmounts()
        {
            foreach (var reciptSlot in reciptSlots)
            {
                reciptSlot.RefreshAllMaterialAmounts();
            }
        }


        private void ClearAllSlots()
        {
            foreach (Transform child in reciptItemsParent)
            {
                Destroy(child.gameObject);
            }
            reciptSlots.Clear();
        }

        public void CloseCrafting()
        {
            UIManager.Hide<CraftingUI>(UIList.CraftingUI);
            UIManager.Show<IngameUI>(UIList.IngameUI);

            // 캐릭터가 이동할 수 있도록 설정
            EventSystem.OnCameraSwitch?.Invoke("Player");
            EventSystem.OnPlayerConnected?.Invoke();
        }
    }
}
