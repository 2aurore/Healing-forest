using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class HouseInteract : FieldObjectBase, IInteractable
    {
        public void Interact(CharacterBase actor)
        {

            UserDataModel.Singleton.SetCharacterPosition(new Vector3(0, 0, 4));
            LevelLoader.Instance.LoadLevel(LevelType.Home);
            // EventSystem.OnLightToggle(false);   // 라이트를 끄는 이벤트 호출
        }
    }
}
