using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class DropItem : FieldObjectBase, IInteractable
    {
        [SerializeField] private string itemId;  // 아이템 데이터
        [SerializeField] private int quantity = 1;   // 아이템 수량
        [SerializeField] private float currentDurability; // 현재 내구도

        public void Interact(CharacterBase actor)
        {
            Debug.Log($"DropItem Interact 시도: {gameObject.name} (ItemID: {itemId}, Quantity: {quantity})");

            // 인벤토리 상태 디버그 출력
            UserDataModel.Singleton.DebugInventoryStatus();

            bool isAddAllItem = UserDataModel.Singleton.AddItemToInventory(itemId, quantity, currentDurability, out int failedCount);

            Debug.Log($"AddItemToInventory 결과: {isAddAllItem}, 실패 수량: {failedCount}");

            if (isAddAllItem)
            {
                // 아이템을 줍고 나면 해당 셀을 사용 가능 상태로 변경합니다.
                Vector3 position = gameObject.transform.position;
                Vector3Int cellPosition = TileMapManager.Instance.GetWorldToCell(position);
                TileMapManager.Instance.ResetUsedPositions(cellPosition);

                Debug.Log($"아이템 {gameObject.name}을(를) 인벤토리에 추가했습니다.");
                Destroy(gameObject); // 아이템을 줍고 나면 오브젝트를 파괴합니다.

            }
            else
            {
                Debug.LogWarning($"아이템 {gameObject.name}을(를) 인벤토리에 추가하지 못했습니다. 남은 수량: {failedCount}");
                quantity = failedCount;
            }
        }
    }
}
