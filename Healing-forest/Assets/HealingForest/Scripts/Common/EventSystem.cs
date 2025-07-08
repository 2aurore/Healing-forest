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
        public static System.Action OnPlayerCrafting;

        // 인벤토리 관련 이벤트
        public static System.Action OnInventoryChanged;
        public static System.Action<string, int> OnItemCountChanged; // 아이템 ID, 새로운 수량

        // 제작 관련 이벤트
        public static System.Action<string> OnItemCrafted; // 제작된 아이템 ID
        public static System.Action<string> OnCraftingStarted; // 제작 시작된 레시피 ID
        public static System.Action OnCraftingCompleted; // 제작 완료
    }
}
