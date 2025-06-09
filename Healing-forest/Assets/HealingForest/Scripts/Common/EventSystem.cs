using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class EventSystem : SingletonBase<EventSystem>
    {
        // 카메라 관련 이벤트
        public static System.Action<string> OnCameraSwitch;

        // 캐릭터 동작 관련 이벤트
        public static System.Action OnPlayerConnected;
        public static System.Action ReleaseTool;
    }
}
