using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class Tree : MonoBehaviour, IChop, IInteractable
    {
        public GameObject[] fruits; // 나무에서 떨어질 열매

        public void Interact(CharacterBase actor)
        {
            // TODO: 나무에서 열매가 떨어지도록 처리

            for (int i = 0; i < fruits.Length; i++)
            {
                // TODO: 열매를 떨어뜨리는 로직 구현
                Vector3Int pivot = TileMapManager.Instance.GetWorldToCell(transform.position);
                Vector3Int emptyCellPosition = TileMapManager.Instance.GetClockwiseEmptyCellFromObjectMap(pivot);

                Vector3 dropPosition = TileMapManager.Instance.GetCellToWorld(emptyCellPosition);
                // 과일 dropPosition에 떨구기
            }
        }

        public void OnDamaged(CharacterBase actor)
        {
            Debug.Log($"<color=red>{actor.name}이 나무를 찍었습니다.</color>");
        }
    }
}
