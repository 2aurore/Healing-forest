using UnityEngine;

namespace HF
{
    [System.Serializable]
    public class RequiredItem
    {
        [SerializeField] private string itemId; // 아이템 ID
        [SerializeField] private string itemName; // 아이템 이름
        [SerializeField] private int quantity; // 필요한 수량

        public string ItemId => itemId;
        public string ItemName => itemName;
        public int Quantity => quantity;

        public RequiredItem(string itemId, string itemName, int quantity)
        {
            this.itemId = itemId;
            this.itemName = itemName;
            this.quantity = quantity;
        }
    }


    [CreateAssetMenu(fileName = "ReciptData", menuName = "ScriptableObjects/ReciptData")]
    public class ReciptDataSO : ScriptableObject
    {
        [SerializeField] private string reciptId; // 레시피 ID
        [SerializeField] private string reciptName; // 레시피 이름
        [SerializeField] private RequiredItem[] requiredItems; // 필요한 아이템 ID 목록
        [SerializeField] private string resultItemId; // 결과 아이템 ID
        [SerializeField] private int craftingTime; // 제작 시간 (초 단위)

        public string ReciptId => reciptId;
        public string ReciptName => reciptName;
        public RequiredItem[] RequiredItems => requiredItems;
        public string ResultItemId => resultItemId;
        public int CraftingTime => craftingTime;
    }
}
