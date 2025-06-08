using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace HF
{
    public class CameraSystem : SingletonBase<CameraSystem>
    {
        [Header("Cameras")]
        public CinemachineFreeLook playerCamera;
        public CinemachineVirtualCamera craftCamera;

        private void OnEnable()
        {
            // 이벤트 구독
            EventSystem.OnCameraSwitch += SwitchToCamera;
        }

        private void OnDisable()
        {
            // 이벤트 구독 해제
            EventSystem.OnCameraSwitch -= SwitchToCamera;
        }


        private void SwitchToCamera(string cameraName)
        {
            SetCameraPriority(cameraName);
        }

        private void SetCameraPriority(string cameraName)
        {
            // 모든 카메라 우선순위 초기화
            playerCamera.Priority = 0;
            craftCamera.Priority = 0;

            // 선택된 카메라만 활성화
            switch (cameraName)
            {
                case "Player": playerCamera.Priority = 10; break;
                case "Craft": craftCamera.Priority = 10; break;
            }
        }
    }
}
