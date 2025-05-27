using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    [System.Serializable]
    public class GameDataDTO { }


    [System.Serializable]
    public class ToolDataDTO : GameDataDTO
    {
        public SerializableDictionary<string, ToolDataSO> toolDatas = new SerializableDictionary<string, ToolDataSO>();
    }

    // 아이템 기본 데이터 클래스
    [System.Serializable]
    public class GameItemDataDTO : GameDataDTO
    {
        public SerializableDictionary<string, ItemDataSO> itemDatas = new SerializableDictionary<string, ItemDataSO>();
    }

    [System.Serializable]
    public class NPCDataDTO : GameDataDTO
    {
        public SerializableDictionary<string, NPCDataSO> npcDatas = new SerializableDictionary<string, NPCDataSO>();
    }

}
