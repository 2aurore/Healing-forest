using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public interface IInteractable
    {
        public void Interact(CharacterBase actor);
    }
}
