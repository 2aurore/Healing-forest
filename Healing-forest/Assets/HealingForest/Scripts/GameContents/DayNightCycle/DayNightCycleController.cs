using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class DayNightCycleController : MonoBehaviour
    {
        public static DayNightCycleController Instance { get; private set; }

        public System.Action OnDayChanged;
        public System.Action OnHourChanged;

        public float timeOfDay = 0f; // 0 to 1, where 0.0f is midnight and 1.0f is the next midnight
        public float fullDayLength = 300f; // Length of a full day in seconds

        public Light mainLight;
        public Gradient lightGradient;

        public Material skyboxMaterial; // 원본 Material 파일
        private Material skyboxInstatnce; // 원본을 복사한 Material 인스턴스
        public AnimationCurve skyboxBlendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private void Awake()
        {
            Instance = this;

            skyboxInstatnce = Instantiate(skyboxMaterial);
            RenderSettings.skybox = skyboxInstatnce;
        }

        private void Update()
        {
            timeOfDay += (Time.deltaTime / fullDayLength);
            // TODO: timeOfDay 값을 계산해서 24시간 현실시간을 기준으로 1시간이 바뀌었는지 확인
            // OnHourChanged?.Invoke(); // Notify - Hour changed event


            if (timeOfDay >= 1f)
            {
                timeOfDay -= 1f; // Reset to midnight
                OnDayChanged?.Invoke(); // Notify - Day changed event
            }

            UpdateLighting();
        }

        void UpdateLighting()
        {
            // Calculate the angle of the sun based on time of day
            float sunAngle = timeOfDay * 360f - 90f; // -90 to start at sunrise

            // Set the rotation of the main light (sun)
            mainLight.transform.localRotation = Quaternion.Euler(sunAngle, 0f, 0f);

            // intensity - light 의 밝기 조절
            if (timeOfDay < 0.5f)  // Day time
            {
                mainLight.intensity = Mathf.Lerp(0.5f, 1f, timeOfDay * 2f);
            }
            else    // Night time
            {
                mainLight.intensity = Mathf.Lerp(1f, 0.2f, (timeOfDay - 0.5f) * 2f);
            }

            // Color - light 의 색상 조절
            Color color = lightGradient.Evaluate(timeOfDay);
            mainLight.color = color;

            // Skybox - skybox material 변경
            float normalizedBlend = Mathf.Abs(timeOfDay - 0.5f) * 2f; // 0 to 1 for day, 1 to 0 for night
            //
            float skyBlend = skyboxBlendCurve.Evaluate(normalizedBlend);
            skyboxInstatnce.SetFloat("_BlendCubemaps", skyBlend);
        }
    }
}
