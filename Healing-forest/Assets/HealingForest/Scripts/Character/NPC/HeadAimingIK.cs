using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace HF
{
    public class HeadAimingIK : MonoBehaviour
    {
        public bool isActiveHeadAiming
        {
            set => headAimingRig.weight = value ? 1f : 0f;
        }

        public Rig headAimingRig; // Rig 컨트롤러
        public Transform headRiggingTarget; // 머리 타겟

        public Vector3 HeadAimingPoint
        {
            set => headRiggingTarget.position = value;

        }
    }
}
