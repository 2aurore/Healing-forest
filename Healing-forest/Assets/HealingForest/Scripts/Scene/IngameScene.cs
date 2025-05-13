using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HF
{
    public class IngameScene : SceneBase
    {

        bool isLoadLevelComplete = false;

        public override IEnumerator OnStart()
        {
            // Ingame 씬을 비동기로 로드한다.
            AsyncOperation asyncLoadGame = SceneManager.LoadSceneAsync("Ingame", LoadSceneMode.Single);

            // 로드가 완료될 때 까지 yield return null 을 하면서 기다린다
            while (!asyncLoadGame.isDone)
            {
                yield return null;
            }

            // 1 Frame 대기 후, Ingame Scene안에 있는 GameObject의 Awake()를 한번 수행시키기 위해서서
            yield return null;

            LevelLoader.Instance.OnLevelLoadComplete += OnLevelLoadCompleted;
            if (isLoadLevelComplete)
            {
                yield return new WaitUntil(() => isLoadLevelComplete);
            }
        }

        private void OnLevelLoadCompleted()
        {
            isLoadLevelComplete = true;
        }

        public override IEnumerator OnEnd()
        {
            LevelLoader.Instance.OnLevelLoadComplete -= OnLevelLoadCompleted;

            yield return null;

        }


    }
}
