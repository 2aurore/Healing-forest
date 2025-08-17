using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HF
{
    public class NPCToastSensor : MonoBehaviour
    {
        public GameObject toastPrefab; // 토스트 UI 프리팹
        public TextMeshProUGUI toastText; // 토스트 UI 텍스트 컴포넌트

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                // 플레이어가 NPC의 센서 영역에 들어왔을 때 처리
                toastPrefab.SetActive(true);
                toastText.text = "안녕!"; // 토스트 텍스트 설정
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                // 플레이어가 NPC의 센서 영역을 벗어났을 때 처리
                toastPrefab.SetActive(false);
                toastText.text = string.Empty; // 토스트 텍스트 초기화
            }
        }
    }
}
