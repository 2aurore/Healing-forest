using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CharacterBase : MonoBehaviour
    {



        public bool IsRunning { get; set; }
        public bool IsCrouching { get; set; }
        public bool IsProgressingAction { get; set; }
        public bool IsGrounded { get; set; }


        public Animator animator;
        public float moveSpeed = 2f;    // 이동 속도
        public float rotationSpeed = 10f; // 회전 속도

        public GameObject toolPosition; // 툴을 장착할 위치

        [SerializeField] private Transform groundCheckPoint; // 바닥 체크를 위한 위치
        [SerializeField] private float rayDistance = 0.2f; // 레이 길이
        [SerializeField] private LayerMask groundLayer; // Ground 레이어 마스크

        private float animationParameterSpeed;
        private float animationParameterHorizontal;
        private float animationParameterVertical;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            // 매 프레임마다 지면 체크
            IsGrounded = CheckGround();

            // 디버그 용도로 레이 그리기
            Debug.DrawRay(groundCheckPoint.position, Vector3.down * rayDistance, IsGrounded ? Color.green : Color.red);

            animator.SetFloat("Speed", animationParameterSpeed);
            animator.SetFloat("Horizontal", animationParameterHorizontal);
            animator.SetFloat("Vertical", animationParameterVertical);
            animator.SetBool("IsRunning", IsRunning);

        }


        // 지면 체크 메소드
        public bool CheckGround()
        {
            // groundCheckPoint에서 아래 방향으로 레이 발사
            RaycastHit hit;

            // 레이캐스트를 발사하고 Ground 레이어와 충돌했는지 확인
            if (Physics.Raycast(groundCheckPoint.position, Vector3.down, out hit, rayDistance, groundLayer))
            {
                return true; // Ground 레이어와 충돌함
            }

            return false; // 충돌하지 않음
        }

        public void Move(Vector2 input)
        {
            if (IsProgressingAction)
            {
                // 액션 진행 중에는 이동을 하지 않음
                return;
            }


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

                // 캐릭터가 Grounded 상태일 때만 이동
                if (IsGrounded)
                {
                    // 현재 바라보는 방향(전방)으로 이동
                    transform.position += transform.forward * dynamicMoveSpeed * Time.deltaTime;
                }
            }

        }

        public void Action(float yAxis)
        {
            if (IsProgressingAction)
            {
                // 이미 액션을 진행 중인 경우에는 아무것도 하지 않음
                return;
            }

            IsProgressingAction = true;
            // TODO: 들고있는 도구에 따라 다른 애니메이터 실행하도록 구현 필요
            transform.rotation = Quaternion.Euler(0f, yAxis, 0f);
            animator.Play("Pickax(PropA)");
        }
    }
}
