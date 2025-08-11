using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class HouseInteract : FieldObjectBase, IInteractable
    {
        public void Interact(CharacterBase actor)
        {
            actor.IsProgressingAction = true;
            UserDataModel.Singleton.SetCharacterPosition(new Vector3(0, 0, -3));
            LevelLoader.Instance.LoadLevel(LevelType.Home);
        }
    }
}
