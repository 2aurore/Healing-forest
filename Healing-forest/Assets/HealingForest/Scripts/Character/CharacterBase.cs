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
        [SerializeField] private List<IInteractionHandler> interactionHandlers;  // 상호작용 핸들러 목록


        private float animationParameterSpeed;
        private float animationParameterHorizontal;
        private float animationParameterVertical;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();

            // 애니메이터의 StatueMachineBehaviour를 초기화
            var actionStateBehaviours = animator.GetBehaviours<ActionStateMachineBehaviour>();
            foreach (var behaviour in actionStateBehaviours)
            {
                behaviour.SetCharacterBase(this);
            }
        }

        protected virtual void Update()
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

        private void OnEnable()
        {
            UserDataModel.Singleton.SetCharacterPosition(transform.position); // 캐릭터 위치 저장
            EventSystem.OnPlayerConnected += ResetAnimatorLayer;
            EventSystem.OnCraftingStarted += CraftingStart;
        }
        private void OnDisable()
        {
            EventSystem.OnPlayerConnected -= ResetAnimatorLayer;
            EventSystem.OnCraftingStarted -= CraftingStart;
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
            else
            {
                // TODO: 플레이어 앞이 water 레이어인지 판단
            }

            return false; // 충돌하지 않음
        }

        /// <summary> 캐릭터 이동 메소드 </summary>
        public void Move(Vector2 input)
        {
            if (IsProgressingAction)
            {
                // 액션 진행 중에는 이동을 하지 않음
                animationParameterSpeed = 0f;
                animationParameterHorizontal = 0f;
                animationParameterVertical = 0f;
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

                // UserDataModel을 통해 장비 해제
                UserDataModel.Singleton.ChangeEquipment(toolID);
                return;
            }

            // 툴 데이터 설정
            if (equippedTool != null)
            {
                // 기존 툴 제거
                Destroy(equippedTool);
            }

            currentToolData = GameDataModel.Singleton.GetToolData(toolID);
            UserDataModel.Singleton.ChangeEquipment(toolID);

            animator.SetInteger("ToolType", currentToolData.ToolAnimatorKey == "PropA" ? 1 : 2);
            equippedTool = Instantiate(currentToolData.VisualPrefab, toolPosition.transform);
            equippedTool.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            equippedTool.transform.SetParent(toolPosition.transform); // 툴을 툴 포지션의 자식으로 설정

            if (currentToolData.ToolType == ToolType.Net)
            {
                DamageActor damageActor = equippedTool.GetComponentInChildren<DamageActor>();
                damageActor.Owner = this; // 툴의 DamageActor에 캐릭터 설정
            }
        }

        protected virtual void InitializeInteractionHandlers()
        {
            interactionHandlers = new List<IInteractionHandler>
            {
                new DropItemHandler(),
                new NPCInteractionHandler(),
                new CraftingTableHandler(),
                new TreeShakeHandler(),
                new ToolSpecificHandler(),
                new BedInteractionHandler(),
                new HouseInteractionHandler() // 집 상호작용 핸들러 추가
            };

            // 우선순위에 따라 정렬
            interactionHandlers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }


        public void Action(Vector3 targetPoint)
        {
            if (IsProgressingAction)
            {
                // 이미 액션을 진행 중인 경우에는 아무것도 하지 않음
                return;
            }

            SetActionLookAt(targetPoint);   // 타겟 포인트를 바라보도록 설정
            DetectionMode mode = DetermineDetectionMode();  // 현재 바라보는 방향을 기준으로 상호작용 모드 결정

            DetectInteraction(mode, targetPoint);
        }

        /// <summary> 상호작용 감지 모드를 결정하는 메소드 </summary>
        private DetectionMode DetermineDetectionMode()
        {
            // 미리 주변을 탐지해서 제작대가 있는지 확인
            int layerMask = 1 << LayerMask.NameToLayer("Interactable");
            Vector3 detectionCenter = transform.position + Vector3.up * 0.3f + transform.forward * 0.5f;
            Collider[] overlapped = Physics.OverlapSphere(detectionCenter, interactRadius, layerMask);

            foreach (var collider in overlapped)
            {
                if (collider.TryGetComponent(out CraftingInteract _))
                {
                    return DetectionMode.CraftingTable;
                }
            }

            return currentToolData == null ? DetectionMode.Default : DetectionMode.ToolAction;
        }

        public void SetActionLookAt(Vector3 targetPoint)
        {
            // 애니메이터의 Upper Body Layer의 Weight를 0으로 설정
            int upperBodyLayerIndex = animator.GetLayerIndex("Upper Body Layer");
            animator.SetLayerWeight(upperBodyLayerIndex, 0f);

            IsProgressingAction = true;
            targetPoint.y = transform.position.y; // y축을 현재 캐릭터의 y축으로 설정
            transform.LookAt(targetPoint); // 타겟 포인트를 바라보도록 회전
        }

        /// <summary> 애니메이터 레이어를 초기화하는 메소드 </summary>
        public void ResetAnimatorLayer()
        {
            int upperBodyLayerIndex = animator.GetLayerIndex("Upper Body Layer");
            animator.SetLayerWeight(upperBodyLayerIndex, 1f);

            IsProgressingAction = false;
        }




        /// <summary> 통합된 상호작용 감지 메소드 </summary>
        /// <param name="detectionMode">상호작용 타입</param>
        public void DetectInteraction(DetectionMode detectionMode, Vector3? targetPoint = null)
        {
            if (interactionHandlers == null)
            {
                InitializeInteractionHandlers();
            }

            // 낚시대는 특별 처리 (충돌 감지 없이 바로 처리)
            if (detectionMode == DetectionMode.ToolAction &&
                currentToolData != null &&
                currentToolData.ToolName == "FishingRod")
            {
                var toolHandler = interactionHandlers.Find(h => h is ToolSpecificHandler);
                if (toolHandler != null && toolHandler.CanHandle(null, this))
                {
                    toolHandler.Handle(null, this);
                    return;
                }
            }

            // 일반적인 충돌 감지
            int layerMask = 1 << LayerMask.NameToLayer("Interactable");
            Vector3 detectionCenter = transform.position + Vector3.up * 0.3f + transform.forward * 0.5f;
            Collider[] overlapped = Physics.OverlapSphere(detectionCenter, interactRadius, layerMask, QueryTriggerInteraction.Collide);

            foreach (Collider collider in overlapped)
            {
                // 상호작용 타입별 필터링
                if (!ShouldProcessCollider(collider, detectionMode))
                {
                    continue;
                }

                // 핸들러를 통한 상호작용 처리
                foreach (var handler in interactionHandlers)
                {
                    if (handler.CanHandle(collider, this))
                    {
                        handler.Handle(collider, this);
                        return;
                    }
                }
            }

            // 상호작용을 처리하지 못한 경우
            HandleNoInteraction(detectionMode);
        }

        private bool ShouldProcessCollider(Collider collider, DetectionMode detectionMode)
        {
            switch (detectionMode)
            {
                case DetectionMode.CraftingTable:
                    return collider.TryGetComponent(out CraftingInteract _);
                case DetectionMode.ToolAction:
                    // 도구 액션 시에는 제작대 제외
                    return !collider.TryGetComponent(out CraftingInteract _);
                case DetectionMode.Default:
                    // 기본 상호작용 시에는 제작대 제외
                    return !collider.TryGetComponent(out CraftingInteract _);
                default:
                    return true;
            }
        }

        private void HandleNoInteraction(DetectionMode detectionMode)
        {
            switch (detectionMode)
            {
                case DetectionMode.ToolAction:
                    if (currentToolData != null)
                    {
                        // 낚시대의 경우 지면에 서 있으면 액션을 종료
                        if (currentToolData.ToolName == "FishingRod" && IsGrounded)
                        {
                            IsProgressingAction = false;
                        }
                        else
                        {
                            animator.Play($"Action {currentToolData.ToolName} Failed");
                        }
                    }
                    break;
                case DetectionMode.Default:
                case DetectionMode.CraftingTable:
                    ResetAnimatorLayer();
                    break;
            }
        }


        private void CraftingStart(float craftingTime)
        {
            animator.SetFloat("Creating Time", craftingTime);
            animator.Play("Action Create");

            StartCoroutine(CraftingTimer(craftingTime));
        }

        private IEnumerator CraftingTimer(float totalTime)
        {
            float remainingTime = totalTime;

            while (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                animator.SetFloat("Creating Time", remainingTime);

                yield return null;
            }

            EventSystem.OnCraftingCompleted?.Invoke();
        }

        public Transform GetHeadTransform()
        {
            // NPC의 머리 트랜스폼을 반환
            return GetBoneTransform(HumanBodyBones.Head);
        }
        public Transform GetBoneTransform(HumanBodyBones bone)
        {
            // 지정된 본의 트랜스폼을 반환
            return animator.GetBoneTransform(bone);
        }







    }
}
