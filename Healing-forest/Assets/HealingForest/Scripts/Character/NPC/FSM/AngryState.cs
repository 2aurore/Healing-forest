using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class AngryState : IState
    {
        private const float angryDuration = 5f; // 화난 상태 지속 시간
        private float angryTimer = 0f; // 화난 상태 타이머

        public void Enter(AIBrain brain)
        {
            angryTimer = 0f; // 타이머 초기화
            // 캐릭터[AI]가 화를 낸다. 애니메이션을 바꾼다
            brain.Character.animator.SetTrigger("Angry Trigger");

            // 캐릭터[AI]가 플레이어틑 계속 쳐다본다.
            var playerCharacter = CharacterController.Instance.linkedCharacter;
            Vector3 targetPoint = playerCharacter.transform.position;
            targetPoint.y = brain.Character.transform.position.y; // 캐릭터의 높이와 일치시킴
            brain.Character.transform.LookAt(targetPoint);

            // TODO: 현재 AI 캐릭터가 다른 애니메이션을 하지 않도록 초기화 시켜둔다
        }

        public void Exit(AIBrain brain)
        {
            // 화난 상태가 끝나면 stack을 초기화한다
            brain.ResetDamageStack();
        }

        public void Update(AIBrain brain)
        {
            angryTimer += Time.deltaTime;

            // 캐릭터[AI]가 플레이어틑 계속 쳐다본다.
            var playerCharacter = CharacterController.Instance.linkedCharacter;
            Vector3 targetPoint = playerCharacter.transform.position;
            targetPoint.y = brain.Character.transform.position.y; // 캐릭터의 높이와 일치시킴
            brain.Character.transform.LookAt(targetPoint);

            if (angryTimer >= angryDuration)
            {
                // 화난 상태가 끝나면 기본 상태로 돌아간다
                brain.ChangeState(new DefaultState());
            }
        }
    }
}
