using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CraftingUI : UIBase
    {
        private void OnEnable()
        {
            InputSystem.Singleton.OnEscapeInput += CloseCrafting;
        }
        private void OnDisable()
        {
            InputSystem.Singleton.OnEscapeInput -= CloseCrafting;
        }


        public void CloseCrafting()
        {
            UIManager.Hide<CraftingUI>(UIList.CraftingUI);
            // TODO : 캐릭터가 이동할 수 있도록 설정
            EventSystem.OnCameraSwitch?.Invoke("Player");
        }
    }
}
