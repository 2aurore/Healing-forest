using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class LevelLoaderWaiter : MonoBehaviour
    {
        private Rigidbody rb;
        private CharacterBase characterBase;
        private bool isPositionSet = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            characterBase = GetComponent<CharacterBase>();
        }

        private void Start()
        {
            LevelLoader.Instance.OnLevelLoadStart += OnLevelLoadStart;
            LevelLoader.Instance.OnLevelLoadComplete += OnLevelLoadComplete;
            LevelLoader.Instance.OnCharacterInitializeComplete += OnCharacterInitializeComplete;

        }

        private void OnLevelLoadStart()
        {
            rb.useGravity = false;
        }
        private void OnLevelLoadComplete()
        {
            // transform.position = UserDataModel.Singleton.CharacterPosition; // 저장된 캐릭터 위치로 이동

            Quaternion rotation = Quaternion.identity;
            // 현재 활성화된 씬에 따라 Y 좌표 조정
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("Level_Home").isLoaded)
            {
                rotation = Quaternion.Euler(0, 0, 0);  // Y = 0도
            }
            else if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("Level_Field").isLoaded)
            {
                rotation = Quaternion.Euler(0, -90f, 0);  // Y = -90도
            }

            transform.SetLocalPositionAndRotation(UserDataModel.Singleton.CharacterPosition, rotation);
            rb.useGravity = true;

            // Coroutine으로 3초 후 UI 숨기기
            StartCoroutine(HideLoadingUIAfterDelay(2f));
        }

        private IEnumerator HideLoadingUIAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            UIManager.Hide<LoadingUI>(UIList.LoadingUI);
        }

        private void OnCharacterInitializeComplete()
        {
            if (isPositionSet)
            {
                // 지면 체크 후 물리 활성화
                StartCoroutine(EnablePhysicsAfterGroundCheck());
            }
        }

        private IEnumerator EnablePhysicsAfterGroundCheck()
        {
            // 지면 체크가 가능할 때까지 대기
            yield return new WaitUntil(() => characterBase != null && characterBase.CheckGround());

            rb.isKinematic = false;
            rb.useGravity = true;

            Debug.Log("[LevelLoaderWaiter] 물리 활성화 완료");

            // EventSystem을 통해 캐릭터 연결 완료 알림
            EventSystem.OnPlayerConnected?.Invoke();
        }

        private void OnDestroy()
        {
            if (LevelLoader.Instance != null)
            {
                LevelLoader.Instance.OnLevelLoadStart -= OnLevelLoadStart;
                LevelLoader.Instance.OnLevelLoadComplete -= OnLevelLoadComplete;
                LevelLoader.Instance.OnCharacterInitializeComplete -= OnCharacterInitializeComplete;
            }
        }

    }
}
