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


    [CreateAssetMenu(fileName = "RecipeData", menuName = "ScriptableObjects/RecipeData")]
    public class RecipeDataSO : ScriptableObject
    {
        [SerializeField] private string recipeId; // 레시피 ID
        [SerializeField] private string recipeName; // 레시피 이름
        [SerializeField] private RequiredItem[] requiredItems; // 필요한 아이템 ID 목록
        [SerializeField] private string resultItemId; // 결과 아이템 ID
        [SerializeField] private int craftingTime; // 제작 시간 (초 단위)

        public string RecipeID => recipeId;
        public string RecipeName => recipeName;
        public RequiredItem[] RequiredItems => requiredItems;
        public string ResultItemId => resultItemId;
        public int CraftingTime => craftingTime;
    }
}
