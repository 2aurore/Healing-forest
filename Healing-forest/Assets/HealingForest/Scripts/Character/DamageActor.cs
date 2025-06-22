using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class DamageActor : MonoBehaviour
    {
        public CharacterBase Owner { get; set; }
        void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.TryGetComponent(out IDamage damageInterface))
            {
                damageInterface.Damage(Owner);
            }
        }
    }
}
