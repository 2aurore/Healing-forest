using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CraftingUI : UIBase
    {

        [SerializeField] private Transform reciptItemsParent;
        [SerializeField] private GameObject reciptItemPrefab;

        private List<Crafting_Recipt> reciptSlots = new List<Crafting_Recipt>();


        public void Initialize()
        {
            // TODO : 초기화 작업
            // 예를 들어, UI 요소들을 초기 상태로 설정하거나 필요한 데이터를 로드하는 작업 등을 수행할 수 있습니다.
            // 현재는 단순히 로그를 출력하는 것으로 대체합니다.
            Debug.Log("CraftingUI Initialized");
        }


        public void CloseCrafting()
        {
            UIManager.Hide<CraftingUI>(UIList.CraftingUI);
            // TODO : 캐릭터가 이동할 수 있도록 설정
            EventSystem.OnCameraSwitch?.Invoke("Player");
            EventSystem.OnPlayerConnected?.Invoke();
        }
    }
}
