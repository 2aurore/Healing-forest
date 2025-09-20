using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class GizmoRender : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            GizmoUtility.DrawArrowHandle(transform.position, transform.forward, 1f, Color.green);
        }
    }
}
