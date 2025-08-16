using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class HouseExitSensor : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var character = other.GetComponent<CharacterBase>();
                if (character != null)
                {
                    // LevelLoader의 MoveToField 메서드를 사용하여
                    // 저장된 Field 위치로 복귀
                    LevelLoader.Instance.MoveToField();
                }
            }
        }

    }
}
