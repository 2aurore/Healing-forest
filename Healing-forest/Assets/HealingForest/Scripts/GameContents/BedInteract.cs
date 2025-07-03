using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class BedInteract : MonoBehaviour, IInteractable
    {
        public Transform enterPoint; // 침대에 눕기 위한 위치
        public Transform exitPoint; // 침대에서 일어나기 위한 위치

        public void Interact(CharacterBase actor)
        {
            actor.animator.SetTrigger("Sleeping Trigger");
            actor.animator.SetBool("IsSleeping", true);
            actor.transform.position = enterPoint.position;
            // actor.transform.rotation = enterPoint.rotation;
        }
    }
}
