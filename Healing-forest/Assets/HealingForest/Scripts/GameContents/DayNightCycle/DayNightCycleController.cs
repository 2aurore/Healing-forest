using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class DayNightCycleController : MonoBehaviour
    {
        public float timeOfDay = 0f; // 0 to 1, where 0.0f is midnight and 1.0f is the next midnight
        public float fullDayLength = 300f; // Length of a full day in seconds

        public Light mainLight;


        private void Update()
        {
            timeOfDay += (Time.deltaTime / fullDayLength);
            if (timeOfDay >= 1f)
            {
                timeOfDay -= 1f; // Reset to midnight
            }

            UpdateLighting();
        }

        void UpdateLighting()
        {
            // Calculate the angle of the sun based on time of day
            float sunAngle = timeOfDay * 360f - 90f; // -90 to start at sunrise

            // Set the rotation of the main light (sun)
            mainLight.transform.localRotation = Quaternion.Euler(sunAngle, 0f, 0f);

            if (timeOfDay < 0.5f)
            {
                mainLight.intensity = Mathf.Lerp(0.5f, 1f, timeOfDay * 2f); // Sunrise to noon
            }
            else
            {
                mainLight.intensity = Mathf.Lerp(1f, 0.2f, (timeOfDay - 0.5f) * 2f); // Noon to sunset
            }
        }
    }
}
