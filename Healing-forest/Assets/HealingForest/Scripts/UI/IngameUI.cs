using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HF
{
    public class IngameUI : UIBase
    {
        [Header("Time Display")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI dateText;

        private string timeFormat = "tt hh:mm";
        private string dateFormat = "MM월 dd일 (dddd)"; // dddd = 전체 요일명, ddd = 축약 요일명

        private void Start()
        {
            // 시간 업데이트를 1초마다 실행
            InvokeRepeating(nameof(UpdateTimeDisplay), 0f, 1f);
        }

        private void UpdateTimeDisplay()
        {
            DateTime currentTime = DateTime.Now;

            // 시간 표시 업데이트
            if (timeText != null)
            {
                timeText.text = currentTime.ToString(timeFormat);
            }

            // 날짜 표시 업데이트
            if (dateText != null)
            {
                dateText.text = currentTime.ToString(dateFormat);
            }


        }

        /// <summary>
        /// 시간 형식을 런타임에 변경
        /// </summary>
        /// <param name="newFormat">새로운 시간 형식 (예: "HH:mm", "hh:mm tt")</param>
        public void SetTimeFormat(string newFormat)
        {
            timeFormat = newFormat;
        }

        /// <summary>
        /// 날짜 형식을 런타임에 변경
        /// </summary>
        /// <param name="newFormat">새로운 날짜 형식 (예: "MM/dd/yyyy", "dd-MM-yyyy")</param>
        public void SetDateFormat(string newFormat)
        {
            dateFormat = newFormat;
        }

        private void OnDestroy()
        {
            // InvokeRepeating 정리
            CancelInvoke(nameof(UpdateTimeDisplay));
        }
    }
}
