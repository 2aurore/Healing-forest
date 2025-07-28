using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class HouseInteractionHandler : IInteractionHandler
    {
        public int Priority => 5;

        public bool CanHandle(Collider collider, CharacterBase character)
        {
            return collider.TryGetComponent(out HouseInteract _);
        }

        public void Handle(Collider collider, CharacterBase character)
        {
            // HouseInteract 컴포넌트를 가져와서 상호작용 처리
            var houseInteract = collider.GetComponent<HouseInteract>();
            if (houseInteract != null)
            {
                character.SetActionLookAt(collider.transform.position);
                houseInteract.Interact(character);
            }
            else
            {
                Debug.LogWarning("HouseInteract component not found on collider.");
            }
        }
    }
}
