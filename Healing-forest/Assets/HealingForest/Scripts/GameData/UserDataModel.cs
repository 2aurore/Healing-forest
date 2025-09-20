using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class UserDataModel : SingletonBase<UserDataModel>
    {
        [field: SerializeField] public InventoryDTO InventoryData { get; private set; } = new InventoryDTO();
        [field: SerializeField] public int MaxInventorySlots = 40;  // 인벤토리 최대 슬롯 수
        [field: SerializeField] public string CurrentEquipItemID { get; private set; } = string.Empty; // 현재 장착된 아이템 ID

        public event Action<UserItemDataDTO> OnInventoryDataChanged;
        public event Action<string, string> OnEquipmentChanged; // 장비 변경 이벤트 (previous, current)

        private Vector3 initPosition = new Vector3(0, 1.05f, 0); // 기본 캐릭터 위치
        [field: SerializeField] public Vector3 CharacterPosition { get; private set; } = Vector3.zero; // 현재 캐릭터 위치
        [field: SerializeField] public Vector3 LastFieldPosition { get; private set; } = Vector3.zero; // Field에서의 마지막 위치 (Home에서 복귀용)

        public void Initialize()
        {
            InventoryData = new InventoryDTO(); // 인벤토리 데이터 초기화

            // TODO: 기존에 save된 UserData가 있다면 불러오기

            // 게임 시작 시 기본 위치로 초기화
            CharacterPosition = initPosition;
            LastFieldPosition = Vector3.zero; // 저장된 Field 위치는 초기화

            // Default Tool Add To Inventory
            foreach (var toolDataFair in GameDataModel.Singleton.ToolDataDTO.toolDatas)
            {
                AddItemToInventory(toolDataFair.Value.ToolId, 1, 100, out int failedCount); // tool 아이템 추가
            }
        }

        public void SetCharacterPosition(Vector3 position)
        {
            CharacterPosition = new Vector3(position.x, 1.05f, position.z);
            // Debug.Log($"[UserDataModel] 캐릭터 위치 설정: {CharacterPosition}");
        }

        /// <summary>Field에서의 위치를 저장합니다. (Home 진입 전에 호출)</summary>
        public void SaveFieldPosition(Vector3 fieldPosition)
        {
            LastFieldPosition = new Vector3(fieldPosition.x, fieldPosition.y, fieldPosition.z);
            // Debug.Log($"[UserDataModel] Field 위치 저장: {LastFieldPosition}");
        }

        /// <summary>저장된 Field 위치를 반환합니다.</summary>
        public Vector3 GetLastFieldPosition()
        {
            // 저장된 위치가 없다면 기본 위치 반환 (Y값 1.05f로 설정)
            if (LastFieldPosition == Vector3.zero)
            {
                return initPosition;
            }
            return LastFieldPosition;
        }

        public bool IsExistTool(ToolType toolType, out UserItemDataDTO toolItemData)
        {
            for (int i = 0; i < InventoryData.InventoryItems.Count; i++)
            {
                string itemID = InventoryData.InventoryItems[i].itemID;

                List<ToolDataSO> targetToolData = new List<ToolDataSO>(GameDataModel.Singleton.ToolDataDTO.toolDatas.Values)
                    .FindAll(tool => tool.ToolType == toolType); // 도구 타입에 맞는 도구 데이터 가져오기

                if (targetToolData.Exists(tool => tool.ToolId == itemID)) // 도구 데이터가 존재하는지 확인
                {
                    toolItemData = InventoryData.InventoryItems[i]; // 해당 아이템 데이터 가져오기
                    return true; // 도구가 존재함
                }
            }
            toolItemData = null; // 도구 데이터가 존재하지 않음
            return false;
        }

        public void ChangeEquipment(string newItemID)
        {
            string previousItemID = CurrentEquipItemID;
            CurrentEquipItemID = newItemID ?? "";

            if (previousItemID != CurrentEquipItemID)
            {
                // 장비 변경 이벤트 발생
                OnEquipmentChanged?.Invoke(previousItemID, CurrentEquipItemID);
            }
        }

        public void GetCurrentEquipment(out ToolDataSO itemData)
        {
            if (string.IsNullOrEmpty(CurrentEquipItemID))
            {
                itemData = null; // 현재 장착된 아이템이 없으면 null 반환
                return;
            }

            itemData = GameDataModel.Singleton.GetToolData(CurrentEquipItemID);
        }

        /// <summary>
        /// 인벤토리 상태 디버그 출력
        /// </summary>
        public void DebugInventoryStatus()
        {
            Debug.Log($"=== 인벤토리 상태 디버그 ===");
            Debug.Log($"전체 슬롯 수: {InventoryData.InventoryItems.Count}/{MaxInventorySlots}");

            for (int i = 0; i < InventoryData.InventoryItems.Count; i++)
            {
                var item = InventoryData.InventoryItems[i];
                Debug.Log($"슬롯[{i}]: {item.itemID} x{item.itemCount} (내구도: {item.itemDurability})");
            }
            Debug.Log($"========================");
        }

        /// <summary>
        /// 인벤토리에 아이템을 추가할 수 있는지 확인
        /// </summary>
        private bool CanAddItemToInventory(string itemID, int quantity)
        {
            ItemDataSO itemGameData = GameDataModel.Singleton.GetItemData(itemID);
            if (itemGameData == null)
            {
                Debug.LogError($"CanAddItemToInventory: 아이템 데이터를 찾을 수 없습니다. ItemID: {itemID}");
                return false;
            }

            bool isStackable = itemGameData.MaxStack > 1;

            Debug.Log($"CanAddItemToInventory 확인 시작: ItemID={itemID}, Quantity={quantity}, MaxStack={itemGameData.MaxStack}, 현재슬롯수={InventoryData.InventoryItems.Count}");

            if (isStackable)
            {
                // 스택 가능한 아이템의 경우
                int remainingQuantity = quantity;

                // 기존 아이템에 스택 가능한지 확인
                var existingItems = InventoryData.InventoryItems.FindAll(item => item.itemID.Equals(itemID));
                Debug.Log($"기존 {itemID} 아이템 개수: {existingItems.Count}");

                foreach (var item in existingItems)
                {
                    int canStack = itemGameData.MaxStack - item.itemCount;
                    Debug.Log($"기존 아이템 - 현재수량: {item.itemCount}, 추가가능: {canStack}");
                    if (canStack > 0)
                    {
                        remainingQuantity -= canStack;
                        Debug.Log($"스택 후 남은 수량: {remainingQuantity}");
                        if (remainingQuantity <= 0)
                        {
                            Debug.Log("기존 슬롯에 모두 스택 가능!");
                            return true;
                        }
                    }
                }

                // 남은 수량에 대해 새로운 슬롯이 필요한지 확인
                int newSlotsNeeded = Mathf.CeilToInt((float)remainingQuantity / itemGameData.MaxStack);
                bool canAdd = InventoryData.InventoryItems.Count + newSlotsNeeded <= MaxInventorySlots;
                Debug.Log($"새 슬롯 필요: {newSlotsNeeded}, 현재+필요={InventoryData.InventoryItems.Count + newSlotsNeeded}, 최대={MaxInventorySlots}, 결과={canAdd}");
                return canAdd;
            }
            else
            {
                // 스택 불가능한 아이템의 경우
                bool canAdd = InventoryData.InventoryItems.Count < MaxInventorySlots;
                Debug.Log($"스택 불가능 아이템 - 현재슬롯: {InventoryData.InventoryItems.Count}, 최대: {MaxInventorySlots}, 결과: {canAdd}");
                return canAdd;
            }
        }

        public bool AddItemToInventory(string itemID, int quantity, float currentDurability, out int failedCount)
        {
            bool isItemAddSuccess = false; // 아이템 추가 성공 여부
            failedCount = 0; // 실패한 개수 초기화

            if (false == GameDataModel.Singleton.ItemDataDTO.itemDatas.ContainsKey(itemID))
            {
                failedCount = quantity; // 아이템이 존재하지 않으면 실패한 개수는 추가하려는 수량
                return isItemAddSuccess; // 아이템 추가 실패
            }

            ItemDataSO itemGameData = GameDataModel.Singleton.GetItemData(itemID); // 아이템 데이터 가져오기
            var isStackable = itemGameData != null ? itemGameData.MaxStack > 1 : false; // 스택 가능 여부 확인

            // 인벤토리에 공간이 있는지 확인
            if (!CanAddItemToInventory(itemID, quantity))
            {
                Debug.Log($"인벤토리에 공간이 부족합니다. 아이템: {itemID}, 수량: {quantity}");
                failedCount = quantity;
                return false;
            }
            if (isStackable)
            {
                int index = InventoryData.InventoryItems.FindIndex(item => item.itemID.Equals(itemID));
                bool isExistSameItem = index >= 0; // 같은 아이템이 존재하는지 확인

                if (isExistSameItem)
                {
                    int afterCount = InventoryData.InventoryItems[index].itemCount + quantity; // 수량 추가
                    int quotient = afterCount / itemGameData.MaxStack;
                    int remainder = afterCount % itemGameData.MaxStack;

                    if (quotient > 0)
                    {
                        for (int i = 0; i < quotient; i++)
                        {
                            UserItemDataDTO newItem = new UserItemDataDTO
                            {
                                uniqueID = InventoryData.InventoryItems.Count.ToString(),
                                itemID = itemID,
                                itemCount = itemGameData.MaxStack,
                                itemDurability = currentDurability
                            };
                            InventoryData.InventoryItems.Add(newItem); // 새로운 아이템 추가
                        }
                    }

                    // 나머지 아이템 수량 업데이트
                    UserItemDataDTO existingItem = InventoryData.InventoryItems[index]; // 같은 아이템이 존재하면 해당 아이템을 가져옴
                    existingItem.itemCount = remainder; // 수량 업데이트
                    existingItem.itemDurability = currentDurability; // 내구도 업데이트
                    InventoryData.InventoryItems[index] = existingItem; // 업데이트된 아이템으로 교체

                    isItemAddSuccess = true; // 아이템 추가 성공
                    OnInventoryDataChanged?.Invoke(existingItem); // 인벤토리 데이터 변경 이벤트 호출
                }
                else // 같은 아이템이 존재하지 않는 경우
                {
                    int afterCount = quantity; // 수량 추가
                    int quotient = afterCount / itemGameData.MaxStack;
                    int remainder = afterCount % itemGameData.MaxStack;

                    if (quotient > 0)
                    {
                        for (int i = 0; i < quotient; i++)
                        {
                            UserItemDataDTO newItem = new UserItemDataDTO
                            {
                                uniqueID = InventoryData.InventoryItems.Count.ToString(),
                                itemID = itemID,
                                itemCount = itemGameData.MaxStack,
                                itemDurability = currentDurability
                            };
                            InventoryData.InventoryItems.Add(newItem); // 새로운 아이템 추가
                        }
                    }

                    if (remainder > 0)
                    {
                        UserItemDataDTO newItem = new UserItemDataDTO
                        {
                            uniqueID = InventoryData.InventoryItems.Count.ToString(),
                            itemID = itemID,
                            itemCount = remainder,
                            itemDurability = currentDurability
                        };
                        InventoryData.InventoryItems.Add(newItem); // 나머지 수량의 아이템 추가
                        OnInventoryDataChanged?.Invoke(newItem); // 인벤토리 데이터 변경 이벤트 호출
                    }

                    isItemAddSuccess = true; // 아이템 추가 성공
                }
            }
            else // 스택 불가능한 아이템의 경우
            {
                UserItemDataDTO item = new UserItemDataDTO
                {
                    uniqueID = InventoryData.InventoryItems.Count.ToString(),
                    itemID = itemID,
                    itemCount = quantity,
                    itemDurability = currentDurability
                };

                InventoryData.InventoryItems.Add(item);

                isItemAddSuccess = true; // 아이템 추가 성공
                OnInventoryDataChanged?.Invoke(item); // 인벤토리 데이터 변경 이벤트 호출
            }

            return isItemAddSuccess;
        }

        public UserItemDataDTO GetInventoryItemData(string itemID)
        {
            UserItemDataDTO itemData = null;
            // 인벤토리에서 아이템 ID로 아이템 데이터 찾기
            itemData = InventoryData.InventoryItems.Find(item => item.itemID.Equals(itemID));
            // 인벤토리에 아이템이 없는 경우 그대로 null 반환
            return itemData;
        }

        /// <summary>
        /// 인벤토리에서 아이템을 제거합니다.
        /// </summary>
        /// <param name="itemID">제거할 아이템 ID</param>
        /// <param name="quantity">제거할 수량</param>
        /// <returns>성공적으로 제거된 수량</returns>
        public int RemoveInventoryItem(string itemID, int quantity)
        {
            if (quantity <= 0)
            {
                Debug.LogWarning($"RemoveInventoryItem: 잘못된 수량입니다. ItemID: {itemID}, Quantity: {quantity}");
                return 0;
            }

            int removedCount = 0;
            int remainingToRemove = quantity;

            // 뒤에서부터 검색하여 제거 (최신 아이템부터 제거)
            for (int i = InventoryData.InventoryItems.Count - 1; i >= 0 && remainingToRemove > 0; i--)
            {
                var item = InventoryData.InventoryItems[i];
                if (item.itemID.Equals(itemID))
                {
                    if (item.itemCount <= remainingToRemove)
                    {
                        // 아이템 전체 제거
                        removedCount += item.itemCount;
                        remainingToRemove -= item.itemCount;
                        InventoryData.InventoryItems.RemoveAt(i);

                        Debug.Log($"아이템 슬롯 완전 제거: {itemID} x{item.itemCount}");
                    }
                    else
                    {
                        // 아이템 일부만 제거
                        item.itemCount -= remainingToRemove;
                        removedCount += remainingToRemove;
                        remainingToRemove = 0;

                        // 변경된 아이템 데이터로 업데이트
                        InventoryData.InventoryItems[i] = item;
                        OnInventoryDataChanged?.Invoke(item);

                        Debug.Log($"아이템 수량 감소: {itemID} x{remainingToRemove}, 남은 수량: {item.itemCount}");
                    }
                }
            }

            if (removedCount < quantity)
            {
                Debug.LogWarning($"요청한 수량을 모두 제거할 수 없습니다. ItemID: {itemID}, 요청: {quantity}, 제거됨: {removedCount}");
            }

            return removedCount;
        }

        /// <summary>
        /// 특정 아이템이 인벤토리에 충분한지 확인합니다.
        /// </summary>
        /// <param name="itemID">확인할 아이템 ID</param>
        /// <param name="requiredQuantity">필요한 수량</param>
        /// <returns>충분하면 true, 부족하면 false</returns>
        public bool HasSufficientItem(string itemID, int requiredQuantity)
        {
            int totalCount = 0;

            foreach (var item in InventoryData.InventoryItems)
            {
                if (item.itemID.Equals(itemID))
                {
                    totalCount += item.itemCount;
                }
            }

            return totalCount >= requiredQuantity;
        }

        /// <summary>
        /// 특정 아이템의 총 보유 수량을 반환합니다.
        /// </summary>
        /// <param name="itemID">확인할 아이템 ID</param>
        /// <returns>총 보유 수량</returns>
        public int GetTotalItemCount(string itemID)
        {
            int totalCount = 0;

            foreach (var item in InventoryData.InventoryItems)
            {
                if (item.itemID.Equals(itemID))
                {
                    totalCount += item.itemCount;
                }
            }

            return totalCount;
        }

        /// <summary>
        /// 간편한 아이템 추가 메서드 (기본 내구도 100)
        /// </summary>
        /// <param name="itemID">추가할 아이템 ID</param>
        /// <param name="quantity">추가할 수량</param>
        /// <returns>성공 여부</returns>
        public bool AddInventoryItem(string itemID, int quantity)
        {
            return AddItemToInventory(itemID, quantity, 100f, out int failedCount);
        }
    }
}
