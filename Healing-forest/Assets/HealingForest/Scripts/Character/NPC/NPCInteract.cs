using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class NPCInteract : MonoBehaviour, IInteractable
    {
        private NPCBase npcBase;

        private void Awake()
        {
            npcBase = GetComponentInParent<NPCBase>();
        }

        public void Interact(CharacterBase actor)
        {
            // TODO: actor 값은 항상 Player 캐릭터로 들어온다.
            npcBase.NotifyOnPlayerInteract(actor);
        }
    }
}
