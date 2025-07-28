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

        public Light mainLight;
        public Gradient lightGradient;
        public AnimationCurve lightIntensityCurve;

        public Material skyboxMaterial; // 원본 Material 파일
        public AnimationCurve skyboxBlendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public Volume dayVolume; // Day Volume
        public Volume nightVolume; // Night Volume

        private Material skyboxInstatnce; // 원본을 복사한 Material 인스턴스


        private void Awake()
        {
            Instance = this;

            skyboxInstatnce = Instantiate(skyboxMaterial);
            RenderSettings.skybox = skyboxInstatnce;
        }

        private void Start()
        {
            // 이벤트 구독
            EventSystem.OnLightToggle += SetActive;
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

            mainLight.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            mainLight.intensity = lightIntensityCurve.Evaluate(timeOfDay);

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

            float skyBlend = skyboxBlendCurve.Evaluate(normalizedBlend);
            skyboxInstatnce.SetFloat("_BlendCubemaps", skyBlend);

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
