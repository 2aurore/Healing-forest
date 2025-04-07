using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class InputSystem : SingletonBase<InputSystem>
    {
        public bool IsForceCursorVisible { get; set; }


        public System.Action OnLeftMouseButtonDown;
        public System.Action OnRightMouseButtonDown;
        public System.Action OnEscapeInput;
        public System.Action OnTab;
        public System.Action<float> OnScrollWheel;

        private void Start()
        {
        }

        private void Update()
        {
            // 마우스 버튼 입력 처리
            if (Input.GetMouseButtonDown(0)) OnLeftMouseButtonDown?.Invoke();
            if (Input.GetMouseButtonDown(1)) OnRightMouseButtonDown?.Invoke();

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                OnEscapeInput?.Invoke();
            }
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                OnTab?.Invoke();
            }

            // Debug.Log("mouseScrollDelta Y : " + Input.mouseScrollDelta.y);
            if (Input.mouseScrollDelta.y > 0)
            {
                OnScrollWheel?.Invoke(Input.mouseScrollDelta.y);
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                OnScrollWheel?.Invoke(Input.mouseScrollDelta.y);
            }

        }


    }
}
