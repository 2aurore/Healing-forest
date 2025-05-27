using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HF
{
    /// <summary> RequireComponent: NavMeshAgent 컴포넌트 의존성을 강제함 </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIBrain : MonoBehaviour
    {
        private NavMeshAgent navAgent;
        private CharacterBase character;

        private void Awake()
        {
            character = GetComponent<CharacterBase>();

            navAgent = GetComponent<NavMeshAgent>();
            navAgent.updatePosition = false; // NavMeshAgent가 Transform을 직접 업데이트하지 않도록 설정
            navAgent.updateRotation = false; // NavMeshAgent가 회전을 직접 업데이트하지 않도록 설정
        }

        private void Update()
        {
            // NavMeshAgent의 위치를 CharacterBase의 위치로 업데이트
            navAgent.nextPosition = transform.position;

            if (navAgent.pathStatus == NavMeshPathStatus.PathComplete && RemainingDistance() <= navAgent.stoppingDistance)
            {
                // TODO: 목표 지점에 도착했을 때의 처리
                character.Move(Vector2.zero); // 이동을 멈춤
            }
            else    // TODO: 아직 목표 지점에 도착하지 않은 경우
            {
                if (navAgent.hasPath)
                {
                    Vector3 moveDir = navAgent.steeringTarget - transform.position;
                    if (moveDir.sqrMagnitude > 0.01f) // 작은 움직임은 무시
                    {
                        Vector3 moveDirection = moveDir.normalized;
                        Vector2 input = new Vector2(moveDirection.x, moveDirection.z);

                        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                        float angleDiff = Quaternion.Angle(transform.rotation, targetRotation);
                        if (angleDiff > 1f) // 회전이 충분히 크면 회전
                        {
                            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                        }

                        character.Move(input); // 이동 입력을 CharacterBase에 전달
                    }
                    else
                    {
                        character.Move(Vector2.zero); // 이동을 멈춤
                    }
                }
                else
                {
                    character.Move(Vector2.zero); // 경로가 없으면 이동을 멈춤
                }
            }
        }

        float RemainingDistance()
        {
            if (!navAgent.isOnNavMesh || navAgent.pathPending)
            {
                return float.MaxValue; // 경로가 아직 계산 중인 경우
            }

            return navAgent.remainingDistance; // 남은 거리 반환
        }

        // Start cycle은 IEnumerator 사용 가능
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2f); // Wait for NavMesh to be ready

            Vector2 random = Random.insideUnitCircle;
            SetDestination(transform.position + new Vector3(random.x, 0, random.y) * 10f);
        }



        public void SetDestination(Vector3 position)
        {
            navAgent.SetDestination(position);

        }
    }
}
