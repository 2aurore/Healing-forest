using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HF
{
    public class GameDataModel : SingletonBase<GameDataModel>
    {

        [field: SerializeField] public ToolDataDTO ToolDataDTO { get; private set; } = new ToolDataDTO();

        public int ToolDataCount => ToolDataDTO.toolDatas.Count;
        public int currentToolIndex = 0; // 툴 데이터 인덱스
        public List<string> toolOrder = new List<string>(); // 도구 순서 관리용 리스트


        public void Initialize()
        {
            ToolDataSO[] loadedDatas = Resources.LoadAll<ToolDataSO>("Tools/Data/");

            for (int i = 0; i < loadedDatas.Length; i++)
            {
                string toolID = loadedDatas[i].Tool_ID;
                if (string.IsNullOrEmpty(toolID))
                {
                    Debug.LogError($"Tool ID is null or empty for ToolDataSO at index {i}, {loadedDatas[i]}");
                    continue;
                }

                if (!ToolDataDTO.toolDatas.ContainsKey(toolID))
                {
                    ToolDataDTO.toolDatas.Add(toolID, loadedDatas[i]);
                }
                else
                {
                    Debug.LogError($"Duplicate Tool ID found: {toolID}");
                }
            }

            InitializeToolOrder();
        }

        public void InitializeToolOrder()
        {
            toolOrder.Clear();
            foreach (var toolData in ToolDataDTO.toolDatas)
            {
                toolOrder.Add(toolData.Key);
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


    }
}
