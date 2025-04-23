using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    // 아이템 카테고리를 위한 enum
    public enum ItemCategory
    {
        Material,   // 재료 아이템
        Crafting,   // 제작 아이템
        Equipment   // 장비 아이템
    }

    // 아이템 기본 데이터 클래스
    [System.Serializable]
    public class ItemData
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

        // 생성자
        public ItemData(string id, string name, ItemCategory cat, int stack, float durability, GameObject prefab, Sprite itemIcon)
        {
            itemID = id;
            itemName = name;
            category = cat;
            maxStack = stack;
            defaultDurability = durability;
            visualPrefab = prefab;
            icon = itemIcon;
        }
    }

}
