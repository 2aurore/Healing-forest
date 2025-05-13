using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class LevelLoaderWaiter : MonoBehaviour
    {
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            LevelLoader.Instance.OnLevelLoadStart += () => rb.useGravity = false;
            LevelLoader.Instance.OnLevelLoadComplete += () => rb.useGravity = true;
        }


    }
}
