using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HF
{
    public enum LevelType
    {
        None,
        Field,
        Home,
    }

    public class LevelLoader : MonoBehaviour
    {
        public static LevelLoader Instance { get; private set; } = null;
        public LevelType DefaultLevelType = LevelType.Field;

        // TODO: 씬이 로드되었을때 플레이어의 위치를 조정해야함

        public event System.Action OnLevelLoadStart;
        public event System.Action OnLevelLoadComplete;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            LoadLevel(DefaultLevelType);
        }

        [Button]
        public void LoadLevel(LevelType levelType)
        {
            switch (levelType)
            {
                case LevelType.Field:
                    if (SceneManager.GetSceneByName("Level_Home").isLoaded)
                    {
                        SceneManager.UnloadSceneAsync("Level_Home");
                    }
                    StartCoroutine(LoadLevel("Level_Field"));
                    break;
                case LevelType.Home:
                    if (SceneManager.GetSceneByName("Level_Field").isLoaded)
                    {
                        SceneManager.UnloadSceneAsync("Level_Field");
                    }
                    StartCoroutine(LoadLevel("Level_Home"));
                    break;

            }


        }

        IEnumerator LoadLevel(string levelName)
        {
            AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            while (!asyncLoadLevel.isDone)
            {
                yield return null;
            }

            OnLevelLoadComplete?.Invoke();
        }

    }
}
