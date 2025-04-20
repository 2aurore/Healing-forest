using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HF
{
    public class Tree : MonoBehaviour, IChop, IInteractable
    {
        public List<GameObject> fruits = new List<GameObject>(); // 과일들을 담을 리스트
        public GameObject fruitPrefab; // 열매 프리팹

        private void Start()
        {
            CollectFruits();
        }

        // 과일을 수집하는 메소드
        private void CollectFruits()
        {
            fruits.Clear();

            foreach (Transform child in transform)
            {
                if (child.CompareTag("Fruit"))
                {
                    fruits.Add(child.gameObject);
                }
            }
        }

        public void Interact(CharacterBase actor)
        {
            int fruitCount = fruits.Count;
            Debug.Log($"<color=red>{actor.name}이 나무를 흔들었습니다.</color>");

            // 나무에서 열매가 떨어지도록 처리
            foreach (GameObject fruit in fruits)
            {
                fruit.GetComponent<Rigidbody>().useGravity = true; // 중력 사용
                Destroy(fruit, 1f); // 열매 파괴
            }



            StartCoroutine(DropFruits(fruitCount));
        }

        private IEnumerator DropFruits(int fruitCount)
        {
            HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();
            Vector3Int pivot = TileMapManager.Instance.GetWorldToCell(transform.position);

            for (int i = 0; i < fruitCount; i++)
            {
                // 잠시 대기하여 시간차 생성
                yield return new WaitForSeconds(0.2f);

                // 셀 유효성 체크 및 사용되지 않은 빈 셀 찾기
                Vector3Int emptyCellPosition = TileMapManager.Instance.GetClockwiseEmptyCellFromObjectMap(pivot, usedPositions);

                if (emptyCellPosition != pivot) // 유효한 위치를 찾은 경우
                {
                    usedPositions.Add(emptyCellPosition);

                    Vector3 dropPosition = TileMapManager.Instance.GetCellToWorld(emptyCellPosition);
                    dropPosition.y += fruitPrefab.transform.localScale.y + 0.3f; // 과일이 떨어질 위치 조정

                    GameObject fruit = Instantiate(fruitPrefab, dropPosition, Quaternion.identity, TileMapManager.Instance.objectMap.transform);
                    yield return null;
                }
            }
        }



        public void OnDamaged(CharacterBase actor)
        {
            Debug.Log($"<color=red>{actor.name}이 나무를 찍었습니다.</color>");
        }
    }
}
