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
            Debug.Log("CraftingUI Initialized");

            ClearAllSlots();

            ReciptDataDTO reciptDataDto = GameDataModel.Singleton.ReciptDataDTO;
            if (reciptDataDto != null && reciptDataDto.reciptDatas != null && reciptDataDto.reciptDatas.Count > 0)
            {
                foreach (var recipt in reciptDataDto.reciptDatas.Values)
                {
                    GameObject slotObject = Instantiate(reciptItemPrefab, reciptItemsParent);
                    Crafting_Recipt reciptSlot = slotObject.GetComponent<Crafting_Recipt>();
                    reciptSlot.SetReciptData(recipt.ReciptID);
                    reciptSlots.Add(reciptSlot);
                }
            }
            else
            {
                Debug.LogWarning("No recipt data found.");
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
            // 캐릭터가 이동할 수 있도록 설정
            EventSystem.OnCameraSwitch?.Invoke("Player");
            EventSystem.OnPlayerConnected?.Invoke();
        }
    }
}
