using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    /// <summary> 상호작용 처리 인터페이스 </summary>
    public interface IInteractionHandler
    {
        bool CanHandle(Collider collider, CharacterBase character);
        void Handle(Collider collider, CharacterBase character);
        int Priority { get; } // 우선순위 (낮을수록 먼저 처리)
    }
}
