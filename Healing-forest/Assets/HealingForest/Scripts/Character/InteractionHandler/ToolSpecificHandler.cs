using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    /// <summary> 도구별 상호작용 처리 핸들러 </summary>
    public class ToolSpecificHandler : IInteractionHandler
    {
        public int Priority => 5;

        public bool CanHandle(Collider collider, CharacterBase character)
        {
            if (character.currentToolData == null) return false;

            // 낚시대는 특별한 조건에서만 사용 가능
            if (character.currentToolData.ToolName == "FishingRod")
            {
                return !character.IsGrounded; // 땅에 서 있지 않을 때만 사용 가능
            }

            // 다른 도구들은 상호작용 가능한 오브젝트가 있을 때만
            return CanChop(collider, character) || CanHit(collider, character);
        }

        public void Handle(Collider collider, CharacterBase character)
        {
            // 낚시대 처리
            if (character.currentToolData.ToolName == "FishingRod")
            {
                HandleFishingRod(character);
                return;
            }

            // 낚시대를 제외한 도구는 collider를 바라보도록 설정
            character.SetActionLookAt(collider.transform.position);

            if (CanChop(collider, character))
            {
                var chopInterface = collider.GetComponent<IChop>();
                chopInterface.OnDamaged(character);
            }
            else if (CanHit(collider, character))
            {
                var hitInterface = collider.GetComponent<IHit>();
                hitInterface.OnDamaged(character);
            }
        }

        private void HandleFishingRod(CharacterBase character)
        {
            if (!character.IsGrounded)
            {
                Debug.Log("Can use Fishing Rod while not grounded.");
                // TODO: 낚시대 던지는 애니메이션 재생
                character.animator.Play("Action FishingRod Cast"); // 예시 애니메이션 이름
            }
            else
            {
                character.IsProgressingAction = false;
            }
        }

        private bool CanChop(Collider collider, CharacterBase character)
        {
            return character.currentToolData.CanInteractWith<IChop>() &&
                   collider.TryGetComponent(out IChop _);
        }

        private bool CanHit(Collider collider, CharacterBase character)
        {
            return character.currentToolData.CanInteractWith<IHit>() &&
                   collider.TryGetComponent(out IHit _);
        }
    }

}
