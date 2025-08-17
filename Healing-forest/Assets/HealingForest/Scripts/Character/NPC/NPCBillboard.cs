using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class NPCBillboard : MonoBehaviour
    {
        private Transform mainCam;

        private void Start()
        {
            mainCam = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + mainCam.rotation * Vector3.forward,
                mainCam.rotation * Vector3.up);
        }
    }
}
