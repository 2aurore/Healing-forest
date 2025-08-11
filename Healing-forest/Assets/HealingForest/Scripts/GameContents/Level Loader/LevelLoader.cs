using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Sirenix.OdinInspector;
using StylizedWater2;
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

        public event System.Action OnLevelLoadStart;
        public event System.Action OnLevelLoadComplete;
        public event System.Action OnCharacterInitializeComplete;

        private LevelType currentLevelType = LevelType.None;

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
            UIManager.Show<LoadingUI>(UIList.LoadingUI);
            StartCoroutine(LoadLevelAsync(levelType));
        }

        IEnumerator LoadLevelAsync(LevelType levelType)
        {
            if (currentLevelType != LevelType.None)
            {
                string currentLevelName = $"Level_{currentLevelType}";
                Scene prevScene = SceneManager.GetSceneByName(currentLevelName);
                if (prevScene.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(currentLevelName);
                }
            }

            currentLevelType = levelType;
            string nextLevelName = $"Level_{levelType}";
            AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(nextLevelName, LoadSceneMode.Additive);
            while (!asyncLoadLevel.isDone)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextLevelName));
            OnLevelLoadComplete?.Invoke();


            // 캐릭터 초기화를 위한 추가 대기
            yield return new WaitForSeconds(0.1f);

            // 캐릭터 컨트롤러가 제대로 초기화될 때까지 대기
            yield return new WaitUntil(() => CharacterController.Instance != null);

            OnCharacterInitializeComplete?.Invoke(); // 캐릭터 초기화 완료

            DayNightCycleController.Instance.SetRenderSetting(levelType); // 하늘박스 적용


        }

    }
}
