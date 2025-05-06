using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class Rock : MonoBehaviour, IHit
    {
        public GameObject[] stonePrefabs; // 돌 프리팹들


        public void OnDamaged(CharacterBase actor)
        {
            Vector3Int pivot = TileMapManager.Instance.GetWorldToCell(transform.position);

            Vector3Int emptyCellPosition = TileMapManager.Instance.GetClockwiseEmptyCellFromObjectMap(pivot);

            actor.animator.Play($"Action {actor.currentToolData.ToolName} Hit");
            if (emptyCellPosition == pivot) // 유효한 위치를 찾지 못한 경우
            {
                return;
            }

            // 랜덤하게 프리팹 선택
            int randomIndex = Random.Range(0, stonePrefabs.Length);
            GameObject selectedStone = stonePrefabs[randomIndex];

            Vector3 dropPosition = TileMapManager.Instance.GetCellToWorld(emptyCellPosition);
            dropPosition.y += selectedStone.transform.localScale.y; // 바위가 떨어질 위치 조정

            GameObject stone = Instantiate(selectedStone, dropPosition, Quaternion.identity, TileMapManager.Instance.objectMap.transform);
        }


        public void OnDestroyed(CharacterBase actor)
        {
            // 바위를 부수는 행동을 처리하는 메소드
            StartCoroutine(DestroyRock());
        }

        private IEnumerator DestroyRock()
        {
            // 바위를 부수는 코루틴
            yield return new WaitForSeconds(0.2f); // 잠시 대기 후 바위 파괴
            Destroy(gameObject); // 바위 파괴
        }
    }

}
