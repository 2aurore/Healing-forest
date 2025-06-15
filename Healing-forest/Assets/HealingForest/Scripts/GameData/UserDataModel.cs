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


        // TODO: home <-> field 전환 시 캐릭터 Vector3 위치 저장
        public Vector3 CharacterPosition { get; private set; } = Vector3.zero; // 캐릭터 위치

        public void Initialize()
        {
            Debug.Log("UserDataModel Initialize");
            InventoryData = new InventoryDTO(); // 인벤토리 데이터 초기화

            // TODO: 기존에 save된 UserData가 있다면 불러오기

            // Default Tool Add To Inventory
            foreach (var toolDataFair in GameDataModel.Singleton.ToolDataDTO.toolDatas)
            {
                AddItemToInventory(toolDataFair.Value.ToolId, 1, 100, out int failedCount); // tool 아이템 추가
            }
        }

        public void SetCharacterPosition(Vector3 position)
        {
            // 캐릭터 위치 설정
            // Y 좌표는 1.05로 고정하여 캐릭터가 땅 위에 서 있도록 설정
            CharacterPosition = new Vector3(position.x, 1.05f, position.z);
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

            // TODO: 인벤토리 추가 가능할때 add 하도록 추가
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
    }
}
