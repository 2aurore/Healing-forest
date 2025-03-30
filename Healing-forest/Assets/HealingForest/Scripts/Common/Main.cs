using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HF
{
    public enum SceneType
    {
        None,
        Empty,
        Title,
        Ingame,
    }

    public class Main : SingletonBase<Main>
    {
        private bool isIniaialized = false;

        public void Initialize()
        {
            if (isIniaialized)
                return;
            // 게임에 필요한 필수 시스템 초기화
            UIManager.Singleton.Initalize();

            isIniaialized = true;
        }

        private void Start()
        {
            Initialize();

#if UNITY_EDITOR
            Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (activeScene.name.Equals("Main"))
            {
                ChangeScene(SceneType.Title);
            }
#else
            ChangeScene(SceneType.Title);
#endif
        }

        bool isSceneChangeProgress = false;
        SceneBase currentSceneController = null;
        SceneType currentSceneType = SceneType.None;
        public void ChangeScene(SceneType sceneType, System.Action onSceneChangeCompletedCallback = null)
        {
            if (isSceneChangeProgress)
                return;

            if (currentSceneType == sceneType)
                return;

            currentSceneType = sceneType;
            switch (sceneType)
            {
                case SceneType.Title:
                    StartCoroutine(ChangeScene<TitleScene>(onSceneChangeCompletedCallback));
                    break;
                case SceneType.Ingame:
                    StartCoroutine(ChangeScene<IngameScene>(onSceneChangeCompletedCallback));
                    break;
            }
        }

        private IEnumerator ChangeScene<T>(System.Action onSceneChangeCompletedCallback = null) where T : SceneBase
        {
            UIManager.Show<LoadingUI>(UIList.LoadingUI);
            yield return new WaitForSeconds(3f);

            isSceneChangeProgress = true;

            // 기존에 불러두었던 씬 컨트롤러(Scene Base)가 있다면, OnEnd를 호출해주고 삭제한다.
            if (currentSceneController != null)
            {
                yield return StartCoroutine(currentSceneController?.OnEnd());
                Destroy(currentSceneController.gameObject);
                currentSceneController = null;
            }

            // Empty 씬으로 전환을 먼저 한다
            AsyncOperation emptySceneLoad = SceneManager.LoadSceneAsync("Empty", LoadSceneMode.Single);
            while (!emptySceneLoad.isDone)
            {
                yield return null;
            }

            // 새로운 씬 컨트롤러를 생성한다.
            GameObject newSceneController = new GameObject(typeof(T).Name);
            newSceneController.transform.SetParent(transform);
            currentSceneController = newSceneController.AddComponent<T>();
            yield return StartCoroutine(currentSceneController.OnStart());

            // 씬 전환을 종료했다고 플래그 값을 변경한다.
            isSceneChangeProgress = false;

            // 씬 전환 후 - 콜백 함수를 호출해준다.
            onSceneChangeCompletedCallback?.Invoke();
            UIManager.Hide<LoadingUI>(UIList.LoadingUI);

        }

        internal void SystemQuit()
        {
            // TODO: 게임 종료 전 처리할 것들을 추가

            // 게임 종료
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

        }
    }
}
