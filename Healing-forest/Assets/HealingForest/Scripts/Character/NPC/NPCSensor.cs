using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class NPCSensor : MonoBehaviour
    {
        public event System.Action<Transform> OnDetectedPlayer;
        public event System.Action OnLostPlayer;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                var character = other.GetComponent<CharacterBase>();
                // 플레이어가 NPC의 센서 영역에 들어왔을 때 처리
                OnDetectedPlayer?.Invoke(character.GetHeadTransform());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                // 플레이어가 NPC의 센서 영역을 벗어났을 때 처리
                OnLostPlayer?.Invoke();
            }
        }
    }
}
