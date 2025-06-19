using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class DefaultState : IState
    {
        private float wanderDuration = 5f;
        private float wanderTimer = 0f;

        private const float wanderDurationMin = 3f; // 이동 간격
        private const float wanderDurationMax = 10f; // 이동 간격

        public void Enter(AIBrain brain)
        {
            // 초기 상태 진입 시 작업
            wanderTimer = 0f; // 타이머 초기화
        }

        public void Exit(AIBrain brain)
        {
            // 상태 종료 시 작업
        }

        public void Update(AIBrain brain)
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderDuration)
            {
                // TODO: wanderDuration을 랜덤하게 지정
                wanderDuration = Random.Range(wanderDurationMin, wanderDurationMax);
                wanderTimer = 0f; // 타이머 초기화

                // 랜덤한 좌표로 목표지점 이동
                Vector2 random = Random.insideUnitCircle;
                brain.SetDestination(brain.transform.position + new Vector3(random.x, 0, random.y) * 10f);

            }


        }
    }

}
