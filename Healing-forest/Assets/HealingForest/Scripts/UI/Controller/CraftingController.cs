using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CraftingController : MonoBehaviour
    {
        private CraftingUI craftingUI;
        private bool isInitialized = false;

        private void Start()
        {
            EventSystem.OnPlayerCrafting += OnCraftingUI;
        }

        private void OnEnable()
        {
            UserDataModel.Singleton.OnInventoryDataChanged += OnInventoryDataChanged;
            InputSystem.Singleton.OnEscapeInput += HideCrafting;
        }

        private void OnDisable()
        {
            if (UserDataModel.Singleton != null)
            {
                UserDataModel.Singleton.OnInventoryDataChanged -= OnInventoryDataChanged;
                InputSystem.Singleton.OnEscapeInput -= HideCrafting;
            }
        }

        public void EnsureInitialized()
        {
            if (isInitialized) return;

            craftingUI = UIManager.Singleton.GetUI<CraftingUI>(UIList.CraftingUI);
            if (craftingUI == null)
            {
                Debug.LogError("CraftingController: CraftingUI is not initialized.");
                return;
            }

            craftingUI.Initialize();
            isInitialized = true;
        }

        public void OnCraftingUI()
        {
            EnsureInitialized();

            if (craftingUI != null)
            {
                if (craftingUI.gameObject.activeSelf)
                {
                    HideCrafting();
                }
                else
                {
                    ShowCrafting();
                }
            }
        }

        private void ShowCrafting()
        {
            if (craftingUI != null)
            {
                craftingUI.Show();
                // 제작 UI가 열릴 때 모든 재료 수량을 최신 데이터로 새로고침
                craftingUI.RefreshAllMaterialAmounts();
            }
        }

        public void HideCrafting()
        {
            if (craftingUI != null)
            {
                craftingUI.CloseCrafting();
            }
        }

        private void OnInventoryDataChanged(UserItemDataDTO changedItem)
        {
            // 인벤토리 데이터가 변경되었을 때 제작 UI가 활성화되어 있다면 해당 아이템의 수량 업데이트
            if (isInitialized && craftingUI != null && craftingUI.gameObject.activeSelf)
            {
                Debug.Log($"CraftingController: Updating material amount for item {changedItem.itemID}, new count: {changedItem.itemCount}");
                craftingUI.UpdateMaterialAmounts(changedItem.itemID, changedItem.itemCount);
            }
        }
    }
}
