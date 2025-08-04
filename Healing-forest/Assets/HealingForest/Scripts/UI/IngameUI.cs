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

        [Header("Tool Display")]
        [SerializeField] private GameObject toolInfo;
        [SerializeField] private TextMeshProUGUI ToolText;

        [Header("Game Time Settings")]
        [SerializeField] private bool useGameTime = true; // 게임 시간 사용 여부
        [SerializeField] private int dayCount = 1; // 게임 내 날짜 카운터

        private string timeFormat = "tt hh:mm";
        private string dateFormat = "MM월 dd일 (dddd)";
        private DateTime baseDate; // 게임 시작 기준 날짜

        private void Start()
        {
            // 게임 시작 기준 날짜 설정 (현재 날짜 기준)
            baseDate = DateTime.Today;

            // DayNightCycleController의 날짜 변경 이벤트 구독
            if (DayNightCycleController.Instance != null)
            {
                DayNightCycleController.Instance.OnDayChanged += OnGameDayChanged;
            }

            // 시간 업데이트를 더 자주 실행 (게임 시간은 빠르게 변하므로)
            if (useGameTime)
            {
                InvokeRepeating(nameof(UpdateTimeDisplay), 0f, 0.1f); // 0.1초마다 업데이트
            }
            else
            {
                InvokeRepeating(nameof(UpdateTimeDisplay), 0f, 1f); // 1초마다 업데이트
            }
        }

        private void Update()
        {
            UpdateCurrentToolDisplay();
        }

        private void OnGameDayChanged()
        {
            dayCount++;
        }

        private void UpdateCurrentToolDisplay()
        {
            UserDataModel.Singleton.GetCurrentEquipment(out ToolDataSO currentTool);

            if (currentTool != null)
            {
                toolInfo.SetActive(true);
                ToolText.text = currentTool.ToolName;
            }
            else
            {
                toolInfo.SetActive(false);
                ToolText.text = string.Empty;
            }
        }

        private void UpdateTimeDisplay()
        {
            DateTime displayTime;

            if (useGameTime && DayNightCycleController.Instance != null)
            {
                // 게임 시간 계산
                displayTime = CalculateGameTime();
            }
            else
            {
                // 실제 시간 사용
                displayTime = DateTime.Now;
            }

            // 시간 표시 업데이트
            if (timeText != null)
            {
                timeText.text = displayTime.ToString(timeFormat);
            }

            // 날짜 표시 업데이트
            if (dateText != null)
            {
                dateText.text = displayTime.ToString(dateFormat);
            }
        }

        private DateTime CalculateGameTime()
        {
            var controller = DayNightCycleController.Instance;

            // timeOfDay (0~1)를 24시간으로 변환
            // 0.0 = 자정(00:00), 0.5 = 정오(12:00)
            float totalHours = controller.timeOfDay * 24f;

            // 시, 분, 초 계산
            int hours = Mathf.FloorToInt(totalHours);
            int minutes = Mathf.FloorToInt((totalHours - hours) * 60f);
            int seconds = Mathf.FloorToInt(((totalHours - hours) * 60f - minutes) * 60f);

            // 기준 날짜에서 경과된 날수를 더함
            DateTime gameTime = baseDate.AddDays(dayCount - 1);
            gameTime = gameTime.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);

            return gameTime;
        }

        /// <summary>
        /// 게임 시간 사용 여부를 토글
        /// </summary>
        /// <param name="useGame">true: 게임 시간, false: 실제 시간</param>
        public void ToggleTimeMode(bool useGame)
        {
            useGameTime = useGame;

            // 업데이트 주기 변경
            CancelInvoke(nameof(UpdateTimeDisplay));
            if (useGameTime)
            {
                InvokeRepeating(nameof(UpdateTimeDisplay), 0f, 0.1f);
            }
            else
            {
                InvokeRepeating(nameof(UpdateTimeDisplay), 0f, 1f);
            }
        }

        /// <summary>
        /// 게임을 특정 시간에서 시작하고 싶을 때 DayNightCycleController의 초기 timeOfDay 설정
        /// </summary>
        /// <param name="hour">시작 시간 (0-23)</param>
        /// <param name="minute">시작 분 (0-59)</param>
        public void SetInitialGameTime(int hour, int minute = 0)
        {
            if (DayNightCycleController.Instance != null)
            {
                DayNightCycleController.Instance.SetTime(hour, minute);
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
            // 이벤트 구독 해제
            if (DayNightCycleController.Instance != null)
            {
                DayNightCycleController.Instance.OnDayChanged -= OnGameDayChanged;
            }

            // InvokeRepeating 정리
            CancelInvoke(nameof(UpdateTimeDisplay));
        }
    }
}