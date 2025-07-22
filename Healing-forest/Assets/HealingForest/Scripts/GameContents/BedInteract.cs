using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class BedInteract : FieldObjectBase, IInteractable
    {
        public Transform enterPoint; // 침대에 눕기 위한 위치
        public Transform exitPoint; // 침대에서 일어나기 위한 위치

        [Header("회전 보정 설정")]
        [SerializeField]
        private Vector3 rotationOffset = new Vector3(0, 180, 0); // Y축 180도 보정이 기본값

        private CharacterBase currentCharacter; // 현재 침대를 사용 중인 캐릭터
        private bool isCharacterSleeping = false; // 캐릭터가 잠들어 있는지 여부
        private bool isProgressInAction = false;
        private float bedExitDelay = 1f; // 침대에서 일어날 때 딜레이 시간
        private float bedEnterTime = 0f;

        public void Interact(CharacterBase actor)
        {
            if (!isCharacterSleeping)
            {
                // 침대에 눕기
                GoToBed(actor);
            }
        }

        private void GoToBed(CharacterBase actor)
        {
            currentCharacter = actor;
            isCharacterSleeping = true;

            actor.EquipTool(null); // 도구를 해제

            // 잠자는 애니메이션 재생
            actor.animator.SetTrigger("Sleeping Trigger");
            actor.animator.SetBool("IsSleeping", true);

            // 캐릭터를 침대 위치로 이동
            Vector3 correctedRotation = enterPoint.rotation.eulerAngles + rotationOffset;
            actor.transform.SetPositionAndRotation(enterPoint.position, Quaternion.Euler(correctedRotation));

            // 캐릭터의 움직임을 제한
            actor.IsProgressingAction = true;
            bedEnterTime = Time.time;
        }

        private void Update()
        {
            if (Time.time > bedEnterTime + bedExitDelay)
            {
                // 캐릭터가 잠들어 있을 때만 입력 확인
                if (isCharacterSleeping && currentCharacter != null && !isProgressInAction)
                {
                    CheckForWakeUpInput();
                }
            }
        }

        private void CheckForWakeUpInput()
        {
            // Horizontal(A/D 키 또는 좌우 방향키)과 Vertical(W/S 키 또는 상하 방향키) 입력 확인
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // 입력이 감지되면 (0이 아닌 값이면) 일어나기
            Vector2 input = new Vector2(horizontal, vertical);
            if (input.sqrMagnitude > 0.1f) // 0.1f는 입력 감도 임계값
            {
                StartCoroutine(WakeUpCoroutine());
            }
        }

        private IEnumerator WakeUpCoroutine()
        {
            isProgressInAction = true; // 애니메이션 진행 중임을 표시
            if (currentCharacter == null)
                yield break;

            // 잠자는 애니메이션 해제
            currentCharacter.animator.SetBool("IsSleeping", false);

            bool isLandingComlpete = false;

            // 애니메이션이 끝날 때까지 대기
            while (!isLandingComlpete)
            {

                Vector3 lerpPosition = Vector3.Lerp(
                    currentCharacter.transform.position,
                     exitPoint != null ? exitPoint.position : currentCharacter.transform.position,
                     Time.deltaTime);
                currentCharacter.transform.position = lerpPosition;

                yield return null;

                float curve = currentCharacter.animator.GetFloat("TransformLerpCurve");
                if (curve >= 1f)
                {
                    // exitPoint가 설정되어 있으면 해당 위치로, 없으면 현재 위치에서 일어남
                    if (exitPoint != null)
                    {
                        currentCharacter.transform.SetPositionAndRotation(exitPoint.position, exitPoint.rotation);
                    }
                    isLandingComlpete = true;
                }
            }

            // 상태 초기화
            isCharacterSleeping = false;
            currentCharacter = null;

            Debug.Log("캐릭터가 침대에서 일어났습니다!");
            isProgressInAction = false; // 애니메이션 진행 완료 표시
        }

    }
}