using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HF
{
    public class GameDataModel : SingletonBase<GameDataModel>
    {

        [field: SerializeField] public ToolDataDTO ToolDataDTO { get; private set; } = new ToolDataDTO();
        [field: SerializeField] public GameItemDataDTO ItemDataDTO { get; private set; } = new GameItemDataDTO();
        [field: SerializeField] public NPCDataDTO NPCDataDTO { get; private set; } = new NPCDataDTO();
        [field: SerializeField] public ReciptDataDTO ReciptDataDTO { get; private set; } = new ReciptDataDTO();


        public void Initialize()
        {
            ToolDataSO[] loadedToolDatas = Resources.LoadAll<ToolDataSO>("Tools/Data/");
            ItemDataSO[] loadedItemDatas = Resources.LoadAll<ItemDataSO>("Items/Data/");
            NPCDataSO[] loadedNPCDatas = Resources.LoadAll<NPCDataSO>("NPCs/Data/");
            ReciptDataSO[] loadedReciptDatas = Resources.LoadAll<ReciptDataSO>("Recipts/");

            for (int i = 0; i < loadedToolDatas.Length; i++)
            {
                if (!ToolDataDTO.toolDatas.ContainsKey(loadedToolDatas[i].ToolId))
                {
                    ToolDataDTO.toolDatas.Add(loadedToolDatas[i].ToolId, loadedToolDatas[i]);
                }
            }
            for (int i = 0; i < loadedItemDatas.Length; i++)
            {
                if (!ItemDataDTO.itemDatas.ContainsKey(loadedItemDatas[i].ItemID))
                {
                    ItemDataDTO.itemDatas.Add(loadedItemDatas[i].ItemID, loadedItemDatas[i]);
                }
            }
            for (int i = 0; i < loadedNPCDatas.Length; i++)
            {
                if (!NPCDataDTO.npcDatas.ContainsKey(loadedNPCDatas[i].NpcID))
                {
                    NPCDataDTO.npcDatas.Add(loadedNPCDatas[i].NpcID, loadedNPCDatas[i]);
                }
            }
            for (int i = 0; i < loadedReciptDatas.Length; i++)
            {
                if (!ReciptDataDTO.reciptDatas.ContainsKey(loadedReciptDatas[i].ReciptID))
                {
                    ReciptDataDTO.reciptDatas.Add(loadedReciptDatas[i].ReciptID, loadedReciptDatas[i]);
                }
            }

        }

        public ToolDataSO GetToolData(string toolID)
        {
            if (ToolDataDTO.toolDatas.TryGetValue(toolID, out ToolDataSO toolData))
            {
                return toolData;
            }
            else
            {
                Debug.LogError($"Tool ID not found: {toolID}");
                return null;
            }
        }

        public ItemDataSO GetItemData(string itemID)
        {
            if (ItemDataDTO.itemDatas.TryGetValue(itemID, out ItemDataSO itemData))
            {
                return itemData;
            }
            else
            {
                Debug.LogError($"Item ID not found: {itemID}");
                return null;
            }
        }

        public NPCDataSO GetNPCData(string npcID)
        {
            if (NPCDataDTO.npcDatas.TryGetValue(npcID, out NPCDataSO npcData))
            {
                return npcData;
            }
            else
            {
                Debug.LogError($"NPC ID not found: {npcID}");
                return null;
            }
        }

        public ReciptDataSO GetReciptData(string reciptID)
        {
            if (ReciptDataDTO.reciptDatas.TryGetValue(reciptID, out ReciptDataSO reciptData))
            {
                return reciptData;
            }
            else
            {
                Debug.LogError($"Recipt ID not found: {reciptID}");
                return null;
            }
        }
    }
}
