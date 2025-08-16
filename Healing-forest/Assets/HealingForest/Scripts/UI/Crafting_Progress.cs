using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HF
{
    public class Crafting_Progress : MonoBehaviour
    {
        [SerializeField] private Slider progressBar; // 진행률 UI 오브젝트
        [SerializeField] private float progressDuration; // 제작 진행 시간 (초 단위)

        private void OnEnable()
        {
            EventSystem.OnCraftingStarted += CraftingStart;
        }
        private void OnDisable()
        {
            EventSystem.OnCraftingStarted -= CraftingStart;
        }

        public void CraftingStart(float craftingTime)
        {
            progressDuration = craftingTime;
            StartCoroutine(CraftingTimer());
        }

        private IEnumerator CraftingTimer()
        {
            float remainingTime = 0f;

            while (remainingTime < progressDuration)
            {
                remainingTime += Time.deltaTime;

                float progress = Mathf.Clamp01(remainingTime / progressDuration);

                progressBar.value = progress;


                yield return null;
            }

            // 제작 완료 시 100%로 설정
            progressBar.value = 1f;

            // 제작 완료 이벤트 발생
            EventSystem.OnCraftingCompleted?.Invoke();
        }

    }
}