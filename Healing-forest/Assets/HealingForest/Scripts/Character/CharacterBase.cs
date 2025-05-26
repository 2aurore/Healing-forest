using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        public ToolDataSO currentToolData = null; // 툴 ID
        public GameObject equippedTool = null; // 장착된 툴 데이터;


        [SerializeField] private GameObject toolPosition; // 툴을 장착할 위치
        [SerializeField] private Transform groundCheckPoint; // 바닥 체크를 위한 위치
        [SerializeField] private float rayDistance = 0.2f; // 바닥 체크 레이 길이
        [SerializeField] private LayerMask groundLayer; // Ground 레이어 마스크

        [SerializeField] private float interactRadius = 0.6f; // 상호작용 반경


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

            UserDataModel.Singleton.SetCharacterPosition(transform.position); // 캐릭터 위치 저장

        }

        /// <summary> 바닥 체크 메소드 </summary>
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

        /// <summary> 캐릭터 이동 메소드 </summary>
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

        /// <summary> 툴 장착 메소드 </summary>
        public void EquipTool(string toolID)
        {
            // toolId가 null인 경우에는 툴을 제거하고 애니메이터의 ToolType을 0으로 설정
            if (toolID == null)
            {
                Destroy(equippedTool);

                currentToolData = null;
                animator.SetInteger("ToolType", 0);
                return;
            }

            // 툴 데이터 설정
            if (equippedTool != null)
            {
                // 기존 툴 제거
                Destroy(equippedTool);
            }

            currentToolData = GameDataModel.Singleton.GetToolData(toolID);
            UserDataModel.Singleton.SetCurrentEquipItem(toolID); // 현재 장착된 아이템 ID 설정

            animator.SetInteger("ToolType", currentToolData.ToolAnimatorKey == "PropA" ? 1 : 2);
            equippedTool = Instantiate(currentToolData.VisualPrefab, toolPosition.transform);
            equippedTool.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            equippedTool.transform.SetParent(toolPosition.transform); // 툴을 툴 포지션의 자식으로 설정
        }



        public void Action(Vector3 targetPoint)
        {
            if (IsProgressingAction)
            {
                // 이미 액션을 진행 중인 경우에는 아무것도 하지 않음
                return;
            }

            if (currentToolData == null)
            {
                SetActionLookAt(targetPoint);
                DetectInteractableCast();
                return;
            }


            SetActionLookAt(targetPoint);
            if (currentToolData.ToolName == "FishingRod")
            {
                if (!IsGrounded)
                {
                    // 앞이 Ground가 아닌 상태에서만 낚시대를 던질 수 있음
                    // TODO: 낚시대 던지는 애니메이션 재생
                    Debug.Log("Can use Fishing Rod while not grounded.");
                }
                else
                {
                    IsProgressingAction = false;
                }
                return;
            }
            else
            {
                // TODO: 현재 들고 있는 도구에 따라 다른 로직을 적용
                DetectActionCast();
            }
        }

        private void SetActionLookAt(Vector3 targetPoint)
        {
            // 애니메이터의 Upper Body Layer의 Weight를 0으로 설정
            int upperBodyLayerIndex = animator.GetLayerIndex("Upper Body Layer");
            animator.SetLayerWeight(upperBodyLayerIndex, 0f);

            IsProgressingAction = true;
            targetPoint.y = transform.position.y; // y축을 현재 캐릭터의 y축으로 설정
            transform.LookAt(targetPoint); // 타겟 포인트를 바라보도록 회전
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position + Vector3.up * 0.3f + transform.forward * 0.5f, interactRadius);
        }

        private void DetectActionCast()
        {
            // 레이어 마스크 확인
            int layerMask = 1 << LayerMask.NameToLayer("Interactable");

            Collider[] overlapped = Physics.OverlapSphere(transform.position + Vector3.up * 0.3f + transform.forward * 0.5f, interactRadius, layerMask, QueryTriggerInteraction.Collide);
            foreach (Collider collider in overlapped)
            {
                // 1. 드롭 아이템인 경우 - 도구에 상관없이 상호작용
                if (collider.TryGetComponent(out DropItem itemInterface))
                {
                    // 아이템 인터페이스가 있는 경우
                    SetActionLookAt(collider.transform.position); // 충돌한 오브젝트를 바라보도록 회전
                    animator.Play("PickInPocket");
                    itemInterface.Interact(this);
                    return;
                }

                // 2. 도구별 인터페이스 처리
                bool interactionHandled = HandleToolSpecificInteraction(collider);
                if (interactionHandled)
                {
                    return;
                }
            }

            // 충돌한 오브젝트가 없는 경우
            animator.Play($"Action {currentToolData.ToolName} Failed");
        }

        private bool HandleToolSpecificInteraction(Collider collider)
        {
            // 도구별 인터페이스 처리
            if (currentToolData.CanInteractWith<IChop>() && collider.TryGetComponent(out IChop chopInterface))
            {
                SetActionLookAt(collider.transform.position);
                chopInterface.OnDamaged(this);
                return true;
            }

            if (currentToolData.CanInteractWith<IHit>() && collider.TryGetComponent(out IHit hitInterface))
            {
                SetActionLookAt(collider.transform.position);
                hitInterface.OnDamaged(this);
                return true;
            }

            // 추가적인 인터페이스 처리 확장 가능
            // if (currentToolData.CanInteractWith<IDig>() && collider.TryGetComponent(out IDig digInterface)) { ... }

            return false;
        }

        private void DetectInteractableCast()
        {
            // 레이어 마스크 확인
            int layerMask = 1 << LayerMask.NameToLayer("Interactable");

            Collider[] overlapped = Physics.OverlapSphere(transform.position + Vector3.up * 0.3f + transform.forward * 0.5f, interactRadius, layerMask, QueryTriggerInteraction.Collide);
            foreach (Collider collider in overlapped)
            {
                Debug.Log(collider.name);
                if (collider.TryGetComponent(out IInteractable interactableInterface))
                {
                    SetActionLookAt(collider.transform.position); // 충돌한 오브젝트를 바라보도록 회전
                    if (collider.TryGetComponent(out DropItem item))
                    {
                        // 아이템 인터페이스가 있는 경우
                        animator.Play("PickInPocket");
                        interactableInterface.Interact(this);
                        return;
                    }

                    if (collider.TryGetComponent(out TreeObject tree))
                    {
                        // 나무 앞에서 나무 흔들기 인터페이스가 있는 경우
                        animator.Play("Tree Shake");
                        interactableInterface.Interact(this);
                        return;
                    }

                }

            }

            // 충돌한 오브젝트가 없는 경우
            ResetAnimatorLayer();
        }

        /// <summary> 애니메이터 레이어를 초기화하는 메소드 </summary>
        private void ResetAnimatorLayer()
        {
            int upperBodyLayerIndex = animator.GetLayerIndex("Upper Body Layer");
            animator.SetLayerWeight(upperBodyLayerIndex, 1f);

            IsProgressingAction = false;
        }

        public void DetectCraftingTable()
        {
            int layerMask = 1 << LayerMask.NameToLayer("Interactable");

            Collider[] overlapped = Physics.OverlapSphere(transform.position + Vector3.up * 0.3f + transform.forward * 0.5f, interactRadius, layerMask, QueryTriggerInteraction.Collide);
            foreach (Collider collider in overlapped)
            {
                if (collider.TryGetComponent(out CraftingTable craftingTable))
                {
                    SetActionLookAt(collider.transform.position); // 충돌한 오브젝트를 바라보도록 회전
                    UIManager.Show<CraftingUI>(UIList.CraftingUI);
                }

            }

        }
    }
}
