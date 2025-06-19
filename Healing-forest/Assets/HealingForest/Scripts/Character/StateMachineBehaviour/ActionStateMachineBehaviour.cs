using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class ActionStateMachineBehaviour : StateMachineBehaviour
    {
        [SerializeField] private CharacterBase characterBase;

        public void SetCharacterBase(CharacterBase character)
        {
            characterBase = character;
        }

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineEnter(animator, stateMachinePathHash);

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            int upperBodyLayerIndex = animator.GetLayerIndex("Upper Body Layer");
            animator.SetLayerWeight(upperBodyLayerIndex, 1f);
            characterBase.IsProgressingAction = false;
        }
    }
}
