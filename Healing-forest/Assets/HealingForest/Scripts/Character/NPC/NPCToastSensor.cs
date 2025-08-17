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

        [Header("NPC 인사말 설정")]
        public List<string> greetingMessages = new List<string>()
        {
            "안녕하세요!",
            "좋은 하루네요~",
            "어서오세요!",
            "반가워요!",
            "오늘 날씨가 참 좋네요!",
            "평화로운 하루에요~",
        };

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                // 플레이어가 NPC의 센서 영역에 들어왔을 때 처리
                toastPrefab.SetActive(true);

                // 랜덤 인사말 선택
                if (greetingMessages.Count > 0)
                {
                    int randomIndex = Random.Range(0, greetingMessages.Count);
                    toastText.text = greetingMessages[randomIndex];
                }
                else
                {
                    toastText.text = "안녕!"; // 기본 인사말 (리스트가 비어있을 때)
                }
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
