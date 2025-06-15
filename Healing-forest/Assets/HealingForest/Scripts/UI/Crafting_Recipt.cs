using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HF
{
    public class Crafting_Recipt : MonoBehaviour
    {
        [SerializeField] private Image itemIcon; // 레시피 아이콘을 설정할 수 있는 필드
        [SerializeField] private TextMeshProUGUI reciptNameText; // 레시피 이름을 표시할 텍스트
        [SerializeField] private GameObject materialItemsParent; // 레시피 재료 아이템들을 담을 부모 오브젝트
        private List<Crafting_Recipt_Material> materialSlots = new List<Crafting_Recipt_Material>();

        public void SetReciptData(string reciptID)
        {
            ReciptDataSO reciptData = GameDataModel.Singleton.GetReciptData(reciptID);
            if (reciptData != null)
            {
                // 레시피 아이콘과 이름 설정
                ItemDataSO itemDataSO = GameDataModel.Singleton.GetItemData(reciptData.ResultItemId);
                itemIcon.sprite = itemDataSO.Icon;
                reciptNameText.text = reciptData.ReciptName;

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
                    GameObject materialSlotObject = new GameObject("MaterialSlot");
                    Crafting_Recipt_Material materialSlot = materialSlotObject.AddComponent<Crafting_Recipt_Material>();
                    materialSlot.SetMaterialSlot(material.ItemId, userItemData.itemCount, material.Quantity);
                    materialSlot.transform.SetParent(materialItemsParent.transform);
                    materialSlots.Add(materialSlot);
                }
            }
        }
    }
}
