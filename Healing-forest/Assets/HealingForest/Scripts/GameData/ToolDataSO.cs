using System;
using UnityEngine;

namespace HF
{
    [CreateAssetMenu(fileName = "ToolData", menuName = "ScriptableObjects/ToolData")]
    public class ToolDataSO : ScriptableObject
    {
        [SerializeField] private string toolId;
        [SerializeField] private string toolName;
        [SerializeField] private ToolType toolType; // 툴 타입 (도끼, 삽, 낚시대, 그물 등)
        [SerializeField] private ToolInteractionType[] toolInteractions; // 호환되는 인터페이스 타입들 (도끼는 나무를 베는 인터페이스와 호환됨)
        [SerializeField] private string toolAnimatorKey;    // animation 에서 사용하는 키키
        [SerializeField] private GameObject visualPrefab; // 툴 프리팹
        [SerializeField] private Sprite icon;            // 아이템 아이콘

        public string ToolId => toolId;
        public ToolType ToolType => toolType;
        public ToolInteractionType[] ToolInteractions => toolInteractions;
        public string ToolAnimatorKey => toolAnimatorKey;
        public string ToolName => toolName;
        public GameObject VisualPrefab => visualPrefab;
        public Sprite Icon => icon;


        // 이 도구가 특정 인터페이스 타입과 상호작용 가능한지 확인
        public bool CanInteractWith<T>() where T : IToolInteraction
        {
            Type interfaceType = typeof(T);
            foreach (var interactionType in toolInteractions)
            {
                if (interactionType.GetInterfaceType() == interfaceType)
                {
                    return true;
                }
            }
            return false;
        }

        // 도구의 기본 상호작용 인터페이스 타입 얻기 (우선순위가 가장 높은 것)
        public Type GetPrimaryInteractionType()
        {
            if (toolInteractions != null && toolInteractions.Length > 0)
            {
                return toolInteractions[0].GetInterfaceType();
            }
            return null;
        }
    }

    [System.Serializable]
    public class ToolInteractionType
    {
        [SerializeField] private InteractionType interfaceType;

        public Type GetInterfaceType()
        {
            switch (interfaceType)
            {
                case InteractionType.Chop:
                    return typeof(IChop);
                case InteractionType.Hit:
                    return typeof(IHit);
                // 필요한 매핑 추가
                default:
                    return null;
            }
        }
    }
}
