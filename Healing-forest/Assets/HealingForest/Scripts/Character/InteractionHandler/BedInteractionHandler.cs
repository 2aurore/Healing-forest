using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class BedInteractionHandler : IInteractionHandler
    {
        public int Priority => 2;

        public bool CanHandle(Collider collider, CharacterBase character)
        {
            return collider.TryGetComponent(out BedInteract _);
        }

        public void Handle(Collider collider, CharacterBase character)
        {
            var bedInteract = collider.GetComponent<BedInteract>();
            bedInteract.Interact(character);
        }
    }
}
