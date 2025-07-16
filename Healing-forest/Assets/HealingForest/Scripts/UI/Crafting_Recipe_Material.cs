using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HF
{
    public class Crafting_Recipe_Material : MonoBehaviour
    {
        [SerializeField] private Image icon; // 아이콘을 설정할 수 있는 필드
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private TextMeshProUGUI requiredCountText;

        private string itemID; // 현재 슬롯의 아이템 ID
        private int requiredCount; // 필요한 수량

        public string ItemID => itemID;

        public void SetMaterialSlot(string itemID, int amount, int requiredCount)
        {
            this.itemID = itemID;
            this.requiredCount = requiredCount;

            ItemDataSO itemData = GameDataModel.Singleton.GetItemData(itemID);
            if (itemData != null)
            {
                icon.sprite = itemData.Icon;
                nameText.text = itemData.ItemName;
                requiredCountText.text = requiredCount.ToString();

                UpdateAmount(amount);
            }
        }

        /// <summary>
        /// 보유 수량만 업데이트하는 메서드
        /// </summary>
        public void UpdateAmount(int amount)
        {
            amountText.text = amount.ToString();

            if (amount >= requiredCount)
            {
                // 재료가 충분할 때의 처리
                amountText.color = Color.white; // 기본 색상으로 설정 (예: 흰색)
            }
            else
            {
                // 재료가 부족할 때의 처리
                amountText.color = Color.red; // 빨간색으로 변경
            }
        }

    }
}
