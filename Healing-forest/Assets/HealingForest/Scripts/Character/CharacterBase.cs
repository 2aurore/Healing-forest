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
        public float moveSpeed = 2f;

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

        }

        public void Move(Vector2 input)
        {

            animationParameterSpeed = input.sqrMagnitude > 0f ? IsRunning ? 3f : 0.5f : 0f;
            animationParameterHorizontal = input.x;
            animationParameterVertical = input.y;

            // 캐릭터가 달리는 중일 때 속도를 높이게 하고, 자세를 숙인 상태에서 이동속도 절반으로 줄임
            float dynamicMoveSpeed = IsRunning ? moveSpeed * 2 : IsCrouching ? moveSpeed / 2 : moveSpeed;

            Vector3 movement = new Vector3(input.x, 0f, input.y);
            transform.Translate(movement * dynamicMoveSpeed * Time.deltaTime, Space.Self);


        }
    }
}
