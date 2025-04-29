using UnityEngine;

namespace HF
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
    public class ItemDataSO : ScriptableObject
    {
        [SerializeField] private string itemID;             // 아이템 고유 ID
        [SerializeField] private string itemName;        // 아이템 이름
        [SerializeField] private ItemCategory category;  // 아이템 카테고리
        [SerializeField] private int maxStack;           // 최대 중첩 개수
        [SerializeField] private float defaultDurability; // 기본 내구도
        [SerializeField] private GameObject visualPrefab; // 시각적 프리팹
        [SerializeField] private Sprite icon;            // 아이템 아이콘

        // 속성(Properties)
        public string ItemID => itemID;
        public string ItemName => itemName;
        public ItemCategory Category => category;
        public int MaxStack => maxStack;
        public float DefaultDurability => defaultDurability;
        public GameObject VisualPrefab => visualPrefab;
        public Sprite Icon => icon;
    }
}
