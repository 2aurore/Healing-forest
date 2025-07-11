using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CraftManager : MonoBehaviour
    {
        [SerializeField] private string reciptId; // 레시피 ID
        [SerializeField] private string reciptName; // 레시피 이름
        [SerializeField] private RecipeDataSO reciptData; // 레시피 데이터



        public void CraftItem(CharacterBase actor)
        {
            if (reciptData == null)
            {
                Debug.LogError("레시피 데이터가 설정되지 않았습니다.");
                return;
            }

            // 필요한 아이템이 충분한지 확인
            foreach (var requiredItem in reciptData.RequiredItems)
            {
                // if (!UserDataModel.Singleton.HasItem(requiredItem.ItemId, requiredItem.Quantity))
                // {
                //     Debug.LogError($"필요한 아이템 {requiredItem.ItemName}이(가) 부족합니다.");
                //     return;
                // }
            }

            // 아이템 제작 시간 대기
            // StartCoroutine(CraftCoroutine(actor));
        }
    }
}
