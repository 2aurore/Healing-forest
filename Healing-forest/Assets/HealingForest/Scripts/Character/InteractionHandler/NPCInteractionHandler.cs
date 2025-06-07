using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    /// <summary> NPC 상호작용 처리 핸들러 </summary>
    public class NPCInteractionHandler : IInteractionHandler
    {
        public int Priority => 2;

        public bool CanHandle(Collider collider, CharacterBase character)
        {
            return collider.TryGetComponent(out NPCInteract _);
        }

        public void Handle(Collider collider, CharacterBase character)
        {
            var npcInteract = collider.GetComponent<NPCInteract>();
            character.SetActionLookAt(collider.transform.position);
            npcInteract.Interact(character);
        }
    }
}
