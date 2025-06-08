using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    /// <summary> 제작대 상호작용 처리 핸들러 </summary>
    public class CraftingTableHandler : IInteractionHandler
    {
        public int Priority => 3;

        public bool CanHandle(Collider collider, CharacterBase character)
        {
            return collider.TryGetComponent(out CraftingInteract _);
        }

        public void Handle(Collider collider, CharacterBase character)
        {
            var craftingTable = collider.GetComponent<CraftingInteract>();
            craftingTable.Interact(character);
        }
    }

}
