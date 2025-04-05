using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CharacterBase : MonoBehaviour
    {



        public bool IsRunning { get; set; }
        public bool IsCrouching { get; set; }

        public Animator animator;
        public float moveSpeed = 2f;    // 이동 속도
        public float rotationSpeed = 10f; // 회전 속도
        private float animationParameterSpeed;
        private float animationParameterHorizontal;
        private float animationParameterVertical;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            animator.SetFloat("Speed", animationParameterSpeed);
            animator.SetFloat("Horizontal", animationParameterHorizontal);
            animator.SetFloat("Vertical", animationParameterVertical);
            animator.SetBool("IsRunning", IsRunning);

        }

        public void Move(Vector2 input)
        {

            animationParameterSpeed = input.sqrMagnitude > 0f ? IsRunning ? 3f : 0.5f : 0f;
            animationParameterHorizontal = input.x;
            animationParameterVertical = input.y;

            // 캐릭터가 달리는 중일 때 속도를 높이게 하고, 자세를 숙인 상태에서 이동속도 절반으로 줄임
            float dynamicMoveSpeed = IsRunning ? moveSpeed * 2 : IsCrouching ? moveSpeed / 2 : moveSpeed;

            // 이동 입력이 있는 경우에만 처리
            if (input.sqrMagnitude > 0.1f)
            {
                // 입력 방향에 따른 월드 공간 방향 벡터 계산
                Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

                // 입력 방향을 바라보도록 회전
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // 현재 바라보는 방향(전방)으로 이동
                transform.position += transform.forward * dynamicMoveSpeed * Time.deltaTime;
            }

        }
    }
}
