using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class HouseExitSensor : MonoBehaviour
    {
        [SerializeField] private Vector3 exitPosition;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var character = other.GetComponent<CharacterBase>();
                if (character != null)
                {
                    // 플레이어가 센서 영역에 들어왔을 때 처리
                    UserDataModel.Singleton.SetCharacterPosition(exitPosition);
                    LevelLoader.Instance.LoadLevel(LevelType.Field);
                }
            }
        }

    }
}
