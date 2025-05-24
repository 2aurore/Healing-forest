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
            LevelLoader.Instance.OnLevelLoadStart += OnLevelLoadStart;
            LevelLoader.Instance.OnLevelLoadComplete += OnLevelLoadComplete;
        }

        private void OnLevelLoadStart()
        {
            rb.useGravity = false;
        }
        private void OnLevelLoadComplete()
        {
            transform.position = UserDataModel.Singleton.CharacterPosition; // 저장된 캐릭터 위치로 이동
            rb.useGravity = true;
        }


    }
}
