using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    /// <summary> 드롭 아이템 처리 핸들러 </summary>
    public class DropItemHandler : IInteractionHandler
    {
        public int Priority => 1;

        public bool CanHandle(Collider collider, CharacterBase character)
        {
            return collider.TryGetComponent(out DropItem _);
        }

        public void Handle(Collider collider, CharacterBase character)
        {
            var dropItem = collider.GetComponent<DropItem>();
            character.SetActionLookAt(collider.transform.position);
            character.animator.Play("PickInPocket");
            dropItem.Interact(character);
        }
    }
}
