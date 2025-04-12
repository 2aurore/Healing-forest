using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    [CreateAssetMenu(fileName = "ToolData", menuName = "ScriptableObjects/ToolData", order = 1)]
    public class ToolDataSO : ScriptableObject
    {

        public string toolId;
        public string toolType;
        public string toolName;
        public GameObject visualPrefab; // 툴 프리팹


    }
}
