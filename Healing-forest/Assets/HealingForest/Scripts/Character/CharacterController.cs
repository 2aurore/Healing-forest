using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CharacterController : MonoBehaviour
    {
        public static CharacterController Instance { get; private set; } = null;

        public CharacterBase linkedCharacter;
        [SerializeField] private LayerMask groundLayer;

        private void Awake()
        {
            Instance = this;
            linkedCharacter = GetComponent<CharacterBase>();
        }

        private void Start()
        {
            // 씬 로딩 완료 후 InputSystem 연결을 보장
            StartCoroutine(InitializeInputSystem());
        }

        private IEnumerator InitializeInputSystem()
        {
            // InputSystem 초기화 대기
            yield return new WaitUntil(() => InputSystem.Singleton != null);

            // 기존 이벤트 구독 해제 (중복 방지)
            if (InputSystem.Singleton != null)
            {
                InputSystem.Singleton.OnLeftMouseButtonDown -= LeftMouseButtonEvent;
                InputSystem.Singleton.OnRightMouseButtonDown -= RightMouseButtonEvent;
            }

            // 새로 이벤트 구독
            InputSystem.Singleton.OnLeftMouseButtonDown += LeftMouseButtonEvent;
            InputSystem.Singleton.OnRightMouseButtonDown += RightMouseButtonEvent;
            EventSystem.ReleaseTool += UnEqqipTool;

            Debug.Log($"[CharacterController] InputSystem 연결 완료 - {gameObject.name}");
        }

        private void OnDestroy()
        {
            if (InputSystem.Singleton != null)
            {
                InputSystem.Singleton.OnLeftMouseButtonDown -= LeftMouseButtonEvent;
                InputSystem.Singleton.OnRightMouseButtonDown -= RightMouseButtonEvent;
            }
            EventSystem.ReleaseTool -= UnEqqipTool;

            Instance = null;
        }

        private void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            linkedCharacter.IsRunning = Input.GetKey(KeyCode.LeftShift);

            Vector2 input = new Vector2(horizontal, vertical);
            linkedCharacter.Move(input);
        }

        private void LeftMouseButtonEvent()
        {
            InventoryUI inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);
            if (inventoryUI != null && inventoryUI.gameObject.activeSelf)
            {
                // 인벤토리가 열려있으면 클릭 이벤트 무시
                return;
            }

            // 마우스 포인터 방향으로 액션 수행하도록 적용
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 마우스 클릭한 위치에 Raycast를 쏘아서 충돌한 오브젝트의 정보를 가져옴
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, 1000F, groundLayer, QueryTriggerInteraction.Ignore))
            {
                // 캐릭터가 클릭한 위치를 바라보도록 회전
                linkedCharacter.Action(hitInfo.point);
            }
        }

        [SerializeField] private ToolType currentToolType = ToolType.None;

        private void RightMouseButtonEvent()
        {
            // 캐릭터가 잠자는 상태인지 확인
            if (IsCharacterSleeping())
            {
                Debug.Log("잠자는 상태에서는 도구를 변경할 수 없습니다.");
                return; // 잠자는 상태에서는 도구 변경 무시
            }

            currentToolType++;
            if (currentToolType >= ToolType.End)
            {
                currentToolType = ToolType.None;
                EquipTool(null);
            }
            else
            {
                if (UserDataModel.Singleton.IsExistTool(currentToolType, out UserItemDataDTO existTooluserData))
                {
                    string toolId = existTooluserData.itemID;
                    EquipTool(toolId);
                }
            }

        }

        /// <summary>
        /// 캐릭터가 현재 잠자는 상태인지 확인하는 메서드
        /// </summary>
        /// <returns>잠자는 상태이면 true, 그렇지 않으면 false</returns>
        private bool IsCharacterSleeping()
        {
            if (linkedCharacter == null || linkedCharacter.animator == null)
                return false;

            // 애니메이터의 IsSleeping 파라미터를 확인
            return linkedCharacter.animator.GetBool("IsSleeping");
        }

        /// <summary> 도구 장착 </summary>
        private void EquipTool(string toolId)
        {
            // 캐릭터에 장비 적용
            linkedCharacter.EquipTool(toolId);
        }

        /// <summary> 도구 해제 </summary>
        private void UnEqqipTool()
        {
            // 캐릭터가 도구를 들고 있다면 해제함
            if (linkedCharacter.equippedTool != null)
            {
                currentToolType = ToolType.None;
                EquipTool(null);
            }
        }
    }
}
