using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HF
{
    public class CraftingInteract : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform standingPoint;



        public void Interact(CharacterBase actor)
        {
            UIManager.Show<CraftingUI>(UIList.CraftingUI);
            StartCoroutine(MoveToPositionSmooth(actor, standingPoint.position));
        }

        private IEnumerator MoveToPositionSmooth(CharacterBase actor, Vector3 targetPosition)
        {
            Vector3 startPosition = actor.transform.position;
            float duration = 1f; // 이동 시간 (초 단위)
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                // 더 자연스러운 easing curve
                t = Mathf.SmoothStep(0f, 1f, t);

                actor.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                // 이동하면서 점진적으로 바라보기
                Vector3 lookDirection = Vector3.Lerp(
                    actor.transform.forward,
                    (gameObject.transform.position - actor.transform.position).normalized,
                    t
                );
                if (lookDirection != Vector3.zero)
                {
                    actor.transform.rotation = Quaternion.LookRotation(lookDirection);
                }

                yield return null;
            }

            actor.transform.position = targetPosition;
            actor.transform.LookAt(gameObject.transform.position);
        }


        // TODO : NavMesh를 사용해서 장애물을 피하고 부드럽게 이동 - 이동 후 캐릭터가 움직이지 않는 문제 있음음
        private IEnumerator SmoothNavMeshMovement(CharacterBase actor, Vector3 targetPosition)
        {
            // NavMesh로 경로 계산
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(actor.transform.position, targetPosition, NavMesh.AllAreas, path))
            {
                // 계산된 경로를 따라 부드럽게 이동
                yield return StartCoroutine(FollowNavMeshPath(actor, path.corners));
            }

            actor.transform.LookAt(gameObject.transform.position);
        }

        private IEnumerator FollowNavMeshPath(CharacterBase actor, Vector3[] waypoints)
        {
            float moveSpeed = 3f;

            for (int i = 1; i < waypoints.Length; i++) // 0은 현재 위치라 건너뜀
            {
                Vector3 startPos = actor.transform.position;
                Vector3 endPos = waypoints[i];
                float journeyLength = Vector3.Distance(startPos, endPos);
                float journeyTime = journeyLength / moveSpeed;
                float elapsedTime = 0f;

                while (elapsedTime < journeyTime)
                {
                    elapsedTime += Time.deltaTime;
                    float fractionOfJourney = elapsedTime / journeyTime;

                    actor.transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);

                    // 이동 방향 바라보기
                    Vector3 direction = (endPos - startPos).normalized;
                    if (direction != Vector3.zero)
                    {
                        actor.transform.rotation = Quaternion.LookRotation(direction);
                    }

                    yield return null;
                }
            }
        }


        // TODO: 수동 경로 계산
        private IEnumerator MoveAvoidingObstacles(CharacterBase actor, Vector3 targetPosition)
        {
            Vector3 currentPos = actor.transform.position;
            float moveSpeed = 3f;
            LayerMask obstacleLayer = LayerMask.GetMask("Obstacle"); // 장애물 레이어

            while (Vector3.Distance(currentPos, targetPosition) > 0.5f)
            {
                Vector3 direction = (targetPosition - currentPos).normalized;
                float rayDistance = 2f;

                // 전방에 장애물이 있는지 체크
                if (Physics.Raycast(currentPos, direction, out RaycastHit hit, rayDistance, obstacleLayer))
                {
                    // 장애물을 우회하는 방향 찾기
                    Vector3 avoidDirection = GetAvoidanceDirection(currentPos, direction, hit.normal);
                    direction = avoidDirection;
                }

                // 이동
                Vector3 movement = direction * moveSpeed * Time.deltaTime;
                currentPos += movement;
                actor.transform.position = currentPos;

                // 이동 방향을 바라보기
                if (movement != Vector3.zero)
                {
                    actor.transform.rotation = Quaternion.LookRotation(movement);
                }

                yield return null;
            }

            // 최종 위치 설정
            actor.transform.position = targetPosition;
            actor.transform.LookAt(gameObject.transform.position);
        }

        private Vector3 GetAvoidanceDirection(Vector3 position, Vector3 originalDirection, Vector3 hitNormal)
        {
            // 충돌 법선의 수직 방향으로 우회
            Vector3 rightDirection = Vector3.Cross(originalDirection, Vector3.up);
            Vector3 leftDirection = -rightDirection;

            // 오른쪽과 왼쪽 중 더 나은 경로 선택
            float rightDistance = GetClearDistance(position, rightDirection);
            float leftDistance = GetClearDistance(position, leftDirection);

            return rightDistance > leftDistance ? rightDirection : leftDirection;
        }

        private float GetClearDistance(Vector3 position, Vector3 direction)
        {
            LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
            if (Physics.Raycast(position, direction, out RaycastHit hit, 5f, obstacleLayer))
            {
                return hit.distance;
            }
            return 5f; // 최대 거리
        }

    }
}
