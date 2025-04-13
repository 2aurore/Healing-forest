using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    [CreateAssetMenu(fileName = "ToolData", menuName = "ScriptableObjects/ToolData", order = 1)]
    public class ToolDataSO : ScriptableObject
    {

        public string Tool_ID;
        public string Tool_Type;
        public string Tool_Name;
        public GameObject Visual_Prefab; // 툴 프리팹

    }
}
