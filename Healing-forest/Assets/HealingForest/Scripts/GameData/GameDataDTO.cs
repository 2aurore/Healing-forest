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
        public Dictionary<string, ToolDataSO> toolDatas = new Dictionary<string, ToolDataSO>();
    }


}
