using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class HouseInteract : FieldObjectBase, IInteractable
    {
        [SerializeField] private Vector3 homeSpawnPosition = new Vector3(0, 0, -3);

        public void Interact(CharacterBase actor)
        {
            actor.IsProgressingAction = true;
            // LevelLoader의 MoveToHome 메서드를 사용하여
            // 현재 Field 위치를 저장하고 Home으로 이동
            LevelLoader.Instance.MoveToHome(homeSpawnPosition);
        }
    }
}
