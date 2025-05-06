using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public interface IHit : IToolInteraction
    {
        public void OnDamaged(CharacterBase actor);

        public void OnDestroyed(CharacterBase actor);
    }

}
