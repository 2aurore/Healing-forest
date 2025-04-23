using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class DropItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData itemData;  // 아이템 데이터
        [SerializeField] private int quantity = 1;   // 아이템 수량
        [SerializeField] private float currentDurability; // 현재 내구도

        public void Interact(CharacterBase actor)
        {
            // 아이템을 줍는 로직을 여기에 작성합니다.
            // 예를 들어, 플레이어의 인벤토리에 아이템을 추가하는 등의 작업을 수행할 수 있습니다.
            Debug.Log($"아이템 {gameObject.name}을(를) 주웠습니다.");
            // TODO: 아이템을 인벤토리에 추가하는 로직을 구현합니다.
            // ItemDataManager.Singleton.AddItemData(item); // 아이템을 인벤토리에 추가합니다.
            Destroy(gameObject); // 아이템을 줍고 나면 오브젝트를 파괴합니다.
        }
    }
}

