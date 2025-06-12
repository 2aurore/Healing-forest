using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CraftingController : MonoBehaviour
    {
        private CraftingUI craftingUI;
        private bool isInitialized = false;

        private void OnEnable()
        {
            UserDataModel.Singleton.OnInventoryDataChanged += OnInventoryDataChanged;

        }

        private void OnDisable()
        {
            if (UserDataModel.Singleton != null)
            {
                UserDataModel.Singleton.OnInventoryDataChanged -= OnInventoryDataChanged;
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


        private void OnInventoryDataChanged(UserItemDataDTO changedItem)
        {
            // 인벤토리 데이터가 변경되었을 때의 처리
            // 예를 들어, UI 업데이트나 필요한 로직을 여기에 추가할 수 있습니다.
            Debug.Log("CraftingController: Inventory data changed");
        }
    }
}
