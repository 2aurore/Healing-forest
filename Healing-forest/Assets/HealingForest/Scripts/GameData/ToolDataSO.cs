using UnityEngine;

namespace HF
{
    [CreateAssetMenu(fileName = "ToolData", menuName = "ScriptableObjects/ToolData")]
    public class ToolDataSO : ScriptableObject
    {
        [SerializeField] private string toolId;
        [SerializeField] private string toolName;
        [SerializeField] private ToolType toolType; // 툴 타입 (도끼, 삽, 낚시대, 그물 등)
        [SerializeField] private string toolAnimatorKey;    // animation 에서 사용하는 키키
        [SerializeField] private GameObject visualPrefab; // 툴 프리팹
        [SerializeField] private Sprite icon;            // 아이템 아이콘

        public string ToolId => toolId;
        public ToolType ToolType => toolType;
        public string ToolAnimatorKey => toolAnimatorKey;
        public string ToolName => toolName;
        public GameObject VisualPrefab => visualPrefab;
        public Sprite Icon => icon;


    }
}
