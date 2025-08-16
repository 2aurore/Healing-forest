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

        [Header("Crafting Popup")]
        [SerializeField] private GameObject craftingPopup;  // 제작 팝업
        [SerializeField] private CanvasGroup craftCanvasGroup;

        [Header("Confirm Dialog")]
        [SerializeField] private GameObject confirmDialog; // 확인 대화상자
        [SerializeField] private Button confirmOkButton; // OK 버튼
        [SerializeField] private Button confirmCancelButton; // Cancel 버튼
        [SerializeField] private TextMeshProUGUI confirmText; // 확인 텍스트

        [Header("Progress UI")]
        [SerializeField] private Crafting_Progress craftingProgress; // 제작 진행률 UI
        [SerializeField] private CanvasGroup progressCanvasGroup;


        private List<Crafting_Recipe> reciptSlots = new List<Crafting_Recipe>();
        private string selectedRecipeID; // 선택된 레시피 ID
        private float craftingTime; // 제작 시간 (초 단위)
        private bool isCraftingInProgress = false; // 제작 진행 중 여부 플래그

        private void OnEnable()
        {
            EventSystem.OnCraftingCompleted += CraftItem; // 제작 완료 시 UI 닫기

            if (craftCanvasGroup == null)
            {
                craftCanvasGroup = craftingPopup.GetComponentInChildren<CanvasGroup>();
            }
            if (progressCanvasGroup == null)
            {
                progressCanvasGroup = craftingProgress.GetComponentInChildren<CanvasGroup>();
            }


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
            if (!string.IsNullOrEmpty(selectedRecipeID) && !isCraftingInProgress)
            {
                isCraftingInProgress = true; // 제작 시작 플래그 설정
                
                // 진행률 UI가 존재하는지 확인
                if (craftingProgress != null)
                {
                    progressCanvasGroup.alpha = 1f; // 진행률 UI 활성화
                    // 제작 시작 이벤트 발생
                    EventSystem.OnCraftingStarted?.Invoke(craftingTime);
                }
                else
                {
                    Debug.LogWarning("CraftingProgress component is not assigned in CraftingUI.");
                    // 진행률 UI가 없어도 제작은 시작
                    EventSystem.OnCraftingStarted?.Invoke(craftingTime);
                }
            }

            // 확인창 닫기
            confirmDialog.SetActive(false);
            // 제작 중에는 제작 팝업을 닫지 않도록 수정
            craftCanvasGroup.alpha = 0f; // 제작 팝업 비활성화
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
            // 중복 호출 방지
            if (!isCraftingInProgress)
            {
                Debug.LogWarning("CraftItem called but crafting is not in progress. Ignoring call.");
                return;
            }

            // selectedRecipeID가 null인 경우 에러 방지
            if (string.IsNullOrEmpty(selectedRecipeID))
            {
                Debug.LogError("Selected recipe ID is null or empty. Cannot craft item.");
                isCraftingInProgress = false;
                return;
            }

            RecipeDataSO reciptData = GameDataModel.Singleton.GetRecipeData(selectedRecipeID);
            if (reciptData == null)
            {
                Debug.LogError($"Recipe data not found for ID: {selectedRecipeID}");
                isCraftingInProgress = false;
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

            // 제작 완료 후 제작 팝업 다시 표시
            craftCanvasGroup.alpha = 1f;

            // 진행률 UI 비활성화
            progressCanvasGroup.alpha = 0f;

            // 제작 상태 초기화 (마지막에 수행)
            isCraftingInProgress = false;
            selectedRecipeID = null;
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
