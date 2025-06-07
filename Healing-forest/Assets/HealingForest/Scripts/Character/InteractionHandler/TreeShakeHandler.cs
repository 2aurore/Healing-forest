using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    /// <summary> 나무 흔들기 상호작용 처리 핸들러 </summary>
    public class TreeShakeHandler : IInteractionHandler
    {
        public int Priority => 4;

        public bool CanHandle(Collider collider, CharacterBase character)
        {
            return collider.TryGetComponent(out TreeObject _);
        }

        public void Handle(Collider collider, CharacterBase character)
        {
            var treeObject = collider.GetComponent<TreeObject>();
            character.SetActionLookAt(collider.transform.position);
            character.animator.Play("Tree Shake");
            treeObject.Interact(character);
        }
    }
}
