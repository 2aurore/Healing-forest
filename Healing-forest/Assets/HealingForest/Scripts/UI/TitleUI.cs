using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class TitleUI : UIBase
    {
        public void OnClickStartButton()
        {
            Main.Singleton?.ChangeScene(SceneType.Ingame);
        }

        public void OnClickExitButton()
        {
            Main.Singleton?.SystemQuit();
        }
    }
}
