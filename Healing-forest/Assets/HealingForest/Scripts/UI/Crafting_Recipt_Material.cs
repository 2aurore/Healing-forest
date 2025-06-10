using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HF
{
    public class Crafting_Recipt_Material : MonoBehaviour
    {
        [SerializeField] private Sprite icon; // 아이콘을 설정할 수 있는 필드
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private TextMeshProUGUI requiredCountText;

        public void SetMaterialSlot(string itemID, int amount, int requiredCount)
        {
            ItemDataSO itemData = GameDataModel.Singleton.GetItemData(itemID);
            if (itemData != null)
            {
                icon = itemData.Icon;
                nameText.text = itemData.ItemName;
                amountText.text = amount.ToString();
                requiredCountText.text = requiredCount.ToString();

                if (amount >= requiredCount)
                {
                    // 재료가 충분할 때의 처리
                    amountText.color = Color.green; // 예시로 초록색으로 변경
                }
                else
                {
                    // 재료가 부족할 때의 처리
                    amountText.color = Color.red; // 예시로 빨간색으로 변경
                }
            }
        }

    }
}
