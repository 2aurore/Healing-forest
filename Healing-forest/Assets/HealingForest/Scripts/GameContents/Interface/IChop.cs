using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public interface IChop : IToolInteraction
    {
        public void OnDamaged(CharacterBase actor);
    }
}
