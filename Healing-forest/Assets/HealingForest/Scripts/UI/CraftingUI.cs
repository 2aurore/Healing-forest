using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CraftingUI : UIBase
    {
        private void OnEnable()
        {
            InputSystem.Singleton.OnEscapeInput += OnEscapteInput;
        }
        private void OnDisable()
        {
            InputSystem.Singleton.OnEscapeInput -= OnEscapteInput;
        }


        private void OnEscapteInput()
        {
            UIManager.Hide<CraftingUI>(UIList.CraftingUI);
        }
    }
}
