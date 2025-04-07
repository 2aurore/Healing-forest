using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CharacterController : MonoBehaviour
    {
        public CharacterBase linkedCharacter;
        public LayerMask groundLayer;

        private void Awake()
        {
            linkedCharacter = GetComponent<CharacterBase>();
        }

        private void Start()
        {
            InputSystem.Singleton.OnLeftMouseButtonDown += LeftMouseButtonEvent;
            // InputSystem.Singleton.OnRightMouseButtonDown += RightMouseButtonEvent;
        }

        private void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }

            // float mouseX = Input.GetAxis("Mouse X");
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            linkedCharacter.IsRunning = Input.GetKey(KeyCode.LeftShift);

            Vector2 input = new Vector2(horizontal, vertical);
            linkedCharacter.Move(input);
            // linkedCharacter.Rotate(mouseX);


        }

        private void LeftMouseButtonEvent()
        {
            // 마우스 포인터 방향으로 액션 수행하도록 적용
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 마우스 클릭한 위치에 Raycast를 쏘아서 충돌한 오브젝트의 정보를 가져옴
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, 1000F, groundLayer, QueryTriggerInteraction.Ignore))
            {
                // 캐릭터가 클릭한 위치를 바라보도록 회전
                Vector3 direction = (hitInfo.point - linkedCharacter.transform.position).normalized;
                float yAxisAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                linkedCharacter.Action(yAxisAngle);
            }
        }
    }
}
