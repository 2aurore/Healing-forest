using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class NPCVisualCharacter : MonoBehaviour
    {
        [field: SerializeField] public HeadAimingIK headAimingIK { get; private set; }
        [field: SerializeField] public Animator animator { get; private set; }

        private void Awake()
        {
            // NPC의 시각적 요소를 초기화
            if (headAimingIK == null)
            {
                headAimingIK = GetComponent<HeadAimingIK>();
            }
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void OnValidate()   // OnValidate : 에디터에서만 호출됩니다.
        {
            // 에디터에서만 실행되는 초기화 로직
            if (headAimingIK == null)
            {
                headAimingIK = GetComponent<HeadAimingIK>();
            }
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

    }
}
