using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HF
{
    public class DayNightCycleController : MonoBehaviour
    {
        public static DayNightCycleController Instance { get; private set; }

        public System.Action OnDayChanged;
        public System.Action OnHourChanged;

        [Range(0f, 1f)] public float timeOfDay = 0f; // 0 to 1, where 0.0f is midnight and 1.0f is the next midnight
        public float fullDayLength = 300f; // Length of a full day in seconds

        public Light fieldLight;
        public Light homeLight;
        public Gradient lightGradient;
        public AnimationCurve lightIntensityCurve;

        public Material skyboxMaterial_field; // field용 skybox Material 파일
        public Material skyboxMaterial_home; // home용 skybox Material 파일
        public AnimationCurve skyboxBlendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public Volume dayVolume; // Day Volume
        public Volume nightVolume; // Night Volume
        public Volume homeVolume; // home Volume

        private Material skyboxInstance_field; // 원본을 복사한 Material 인스턴스
        private Material skyboxInstance_home; // 원본을 복사한 Material 인스턴스
        private int lastHour = -1; // 이전 시간 추적용

        public void SetRenderSetting(LevelType levelType)
        {
            switch (levelType)
            {
                case LevelType.Field:
                    RenderSettings.skybox = skyboxInstance_field;
                    fieldLight.gameObject.SetActive(true);
                    homeLight.gameObject.SetActive(false);
                    dayVolume.gameObject.SetActive(true);
                    nightVolume.gameObject.SetActive(true);
                    homeVolume.gameObject.SetActive(false);
                    break;
                case LevelType.Home:
                    RenderSettings.skybox = skyboxInstance_home;
                    fieldLight.gameObject.SetActive(false);
                    homeLight.gameObject.SetActive(true);
                    dayVolume.gameObject.SetActive(false);
                    nightVolume.gameObject.SetActive(false);
                    homeVolume.gameObject.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("Unknown LevelType for setting skybox.");
                    break;
            }
        }

        private void Awake()
        {
            Instance = this;

            skyboxInstance_field = Instantiate(skyboxMaterial_field);
            skyboxInstance_home = Instantiate(skyboxMaterial_home);

            SetRenderSetting(LevelType.Field);
        }

        private void Start()
        {
            // 이벤트 구독
            EventSystem.OnLightToggle += SetActive;

            // 초기 시간 설정
            lastHour = GetCurrentHour();
        }

        private void Update()
        {
            timeOfDay += (Time.deltaTime / fullDayLength);

            // 시간 변경 체크
            CheckHourChange();

            if (timeOfDay >= 1f)
            {
                timeOfDay -= 1f; // Reset to midnight
                OnDayChanged?.Invoke(); // Notify - Day changed event
            }

            UpdateLighting();
        }

        /// <summary>
        /// 현재 게임 시간의 시(hour)를 반환 (0-23)
        /// </summary>
        /// <returns>현재 시간</returns>
        public int GetCurrentHour()
        {
            return Mathf.FloorToInt(timeOfDay * 24f);
        }

        /// <summary>
        /// 현재 게임 시간의 분(minute)를 반환 (0-59)
        /// </summary>
        /// <returns>현재 분</returns>
        public int GetCurrentMinute()
        {
            float totalMinutes = (timeOfDay * 24f * 60f) % 60f;
            return Mathf.FloorToInt(totalMinutes);
        }

        /// <summary>
        /// 현재 게임 시간을 TimeSpan으로 반환
        /// </summary>
        /// <returns>현재 게임 시간</returns>
        public TimeSpan GetCurrentGameTime()
        {
            float totalHours = timeOfDay * 24f;
            int hours = Mathf.FloorToInt(totalHours);
            int minutes = Mathf.FloorToInt((totalHours - hours) * 60f);
            int seconds = Mathf.FloorToInt(((totalHours - hours) * 60f - minutes) * 60f);

            return new TimeSpan(hours, minutes, seconds);
        }

        /// <summary>
        /// 시간 변경을 체크하고 이벤트를 발생시킴
        /// </summary>
        private void CheckHourChange()
        {
            int currentHour = GetCurrentHour();

            if (currentHour != lastHour)
            {
                lastHour = currentHour;
                OnHourChanged?.Invoke(); // Notify - Hour changed event
            }
        }

        /// <summary>
        /// 특정 시간으로 설정 (0-1 범위)
        /// </summary>
        /// <param name="time">설정할 시간 (0: 자정, 0.5: 정오)</param>
        public void SetTimeOfDay(float time)
        {
            timeOfDay = Mathf.Clamp01(time);
            lastHour = GetCurrentHour();
            UpdateLighting();
        }

        /// <summary>
        /// 특정 시간으로 설정 (시간 단위)
        /// </summary>
        /// <param name="hour">시간 (0-23)</param>
        /// <param name="minute">분 (0-59)</param>
        public void SetTime(int hour, int minute = 0)
        {
            hour = Mathf.Clamp(hour, 0, 23);
            minute = Mathf.Clamp(minute, 0, 59);

            float timeValue = (hour + minute / 60f) / 24f;
            SetTimeOfDay(timeValue);
        }

        void UpdateLighting()
        {
            // 햇빛의 회전 각도 : X축 값을 0 ~ 180까지만 회전하도록 설정
            float xRotation = Mathf.PingPong(timeOfDay * 2f, 1f) * 180f;

            // TODO: 시간에 흐름에 따라 그림자 각도 조절 RnD 해볼것
            // float xRotation;
            // if (timeOfDay < 0.25f || timeOfDay > 0.75f) // 밤 시간 (6시간씩)
            // {
            //     // 밤에는 태양을 더 낮게 (하지만 완전히 수평선 아래는 아님)
            //     xRotation = Mathf.Lerp(10f, 30f, timeOfDay < 0.25f ? timeOfDay * 4f : (1f - timeOfDay) * 4f);
            // }
            // else // 낮 시간
            // {
            //     // 낮에는 정상적인 각도
            //     float dayProgress = (timeOfDay - 0.25f) / 0.5f; // 0.25~0.75를 0~1로 정규화
            //     xRotation = Mathf.Lerp(30f, 150f, dayProgress);
            // }

            fieldLight.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            fieldLight.intensity = lightIntensityCurve.Evaluate(timeOfDay);

            // intensity - light 의 밝기 조절
            if (timeOfDay < 0.5f)  // Day time
            {
                fieldLight.intensity = Mathf.Lerp(0.5f, 1f, timeOfDay * 2f);
            }
            else    // Night time
            {
                fieldLight.intensity = Mathf.Lerp(1f, 0.2f, (timeOfDay - 0.5f) * 2f);
            }

            // Color - light 의 색상 조절
            Color color = lightGradient.Evaluate(timeOfDay);
            fieldLight.color = color;

            // Skybox - skybox material 변경
            float normalizedBlend = Mathf.Abs(timeOfDay - 0.5f) * 2f; // 0 to 1 for day, 1 to 0 for night

            float skyBlend = skyboxBlendCurve.Evaluate(normalizedBlend);
            skyboxInstance_field.SetFloat("_BlendCubemaps", skyBlend);

            //post processing volume 변경
            nightVolume.weight = normalizedBlend;
        }

        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                gameObject.SetActive(true);
                UpdateLighting(); // 초기화 시 조명 업데이트
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}