using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class SoundManager : SingletonBase<SoundManager>
    {
        public float MasterVolume
        {
            get => AudioController.GetGlobalVolume();
            set => AudioController.SetGlobalVolume(value);
        }

        public float BGMVolume
        {
            get => AudioController.GetCategoryVolume("BGM");
            set => AudioController.SetCategoryVolume("BGM", value);
        }

        public float SFXVolume
        {
            get => AudioController.GetCategoryVolume("SFX");
            set => AudioController.SetCategoryVolume("SFX", value);
        }

        private void Start()
        {
            PlayBGM("garden");
            MasterVolume = 0.7f; // 마스터 볼륨 설정
        }

        public void PlayBGM(string bgmName)
        {
            AudioController.PlayMusic(bgmName);
        }

        public void PlaySFX(string sfxName)
        {
            AudioController.Play(sfxName);  // 그냥 사운드를 2D로 재생

            // AudioController.Play(sfxName, new Vector3(20,10,20));  // 3D 사운드로 특정 위치에 있는 사운드로 재생
            // AudioController.Play(sfxName, Camera.main.transform); // 무조건 2D 사운드로 재생
        }
    }
}
