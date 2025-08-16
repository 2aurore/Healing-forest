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
        private LevelType previousLevelType = LevelType.None; // 이전 레벨 추적

        // 레벨별 기본 스폰 위치 정의
        [SerializeField] private Vector3 fieldDefaultSpawnPosition = new Vector3(0, 1.05f, 0); // Field 기본 시작 위치
        [SerializeField] private Vector3 homeSpawnPosition = new Vector3(0, 0, -3); // Home 기본 위치

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
            // 이전 레벨 정보 저장
            previousLevelType = currentLevelType;

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

            // 캐릭터 초기화를 위한 추가 대기
            yield return new WaitForSeconds(0.1f);

            // 캐릭터 컨트롤러가 제대로 초기화될 때까지 대기
            yield return new WaitUntil(() => CharacterController.Instance != null);

            // 캐릭터 위치 초기화 (다른 시스템보다 먼저 실행)
            InitializeCharacterPosition(levelType, previousLevelType);

            // 캐릭터 초기화 완료 후 다른 시스템들에게 알림
            OnLevelLoadComplete?.Invoke();
            OnCharacterInitializeComplete?.Invoke(); // 캐릭터 초기화 완료

            DayNightCycleController.Instance.SetRenderSetting(levelType); // 하늘박스 적용
        }

        /// <summary>레벨 타입에 따라 캐릭터 위치를 초기화합니다.</summary>
        private void InitializeCharacterPosition(LevelType currentLevel, LevelType previousLevel)
        {
            Vector3 spawnPosition;

            switch (currentLevel)
            {
                case LevelType.Field:
                    if (previousLevel == LevelType.Home)
                    {
                        // Home에서 Field로 복귀하는 경우: 저장된 Field 위치 사용
                        spawnPosition = UserDataModel.Singleton.GetLastFieldPosition();
                        Debug.Log($"[LevelLoader] Home에서 Field로 복귀: {spawnPosition}");
                    }
                    else
                    {
                        // 게임 시작이나 다른 경우: 기본 Field 위치 사용
                        spawnPosition = fieldDefaultSpawnPosition;
                        Debug.Log($"[LevelLoader] Field 기본 시작: {spawnPosition}");
                    }
                    break;

                case LevelType.Home:
                    // Field에서 Home으로 진입하는 경우: Home 기본 위치 사용
                    spawnPosition = homeSpawnPosition;
                    Debug.Log($"[LevelLoader] Home 진입: {spawnPosition}");
                    break;

                default:
                    spawnPosition = Vector3.zero;
                    break;
            }

            // 캐릭터 실제 위치 설정
            if (CharacterController.Instance != null && CharacterController.Instance.linkedCharacter != null)
            {
                CharacterController.Instance.linkedCharacter.transform.position = spawnPosition;
                UserDataModel.Singleton.SetCharacterPosition(spawnPosition);

                Debug.Log($"[LevelLoader] 캐릭터 위치 초기화 완료: {currentLevel} - {spawnPosition}");
            }
        }

        /// <summary>Field에서 Home으로 이동할 때 사용 (Field 위치 저장 후 이동)</summary>
        public void MoveToHome(Vector3 homeSpawnPos)
        {
            // 현재 Field에서의 위치를 저장
            if (currentLevelType == LevelType.Field && CharacterController.Instance != null)
            {
                Vector3 currentFieldPosition = CharacterController.Instance.linkedCharacter.transform.position;
                UserDataModel.Singleton.SaveFieldPosition(currentFieldPosition);
            }

            // Home 스폰 위치 업데이트
            homeSpawnPosition = homeSpawnPos;

            LoadLevel(LevelType.Home);
        }

        /// <summary>Home에서 Field로 이동할 때 사용</summary>
        public void MoveToField()
        {
            LoadLevel(LevelType.Field);
        }

        /// <summary>현재 레벨 타입을 반환합니다.</summary>
        public LevelType GetCurrentLevelType()
        {
            return currentLevelType;
        }
    }
}
