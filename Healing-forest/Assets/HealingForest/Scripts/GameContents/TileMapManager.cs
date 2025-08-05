using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HF
{
    public class TileMapManager : MonoBehaviour
    {

        public static TileMapManager Instance { get; private set; } = null;// 싱글톤 인스턴스
        public Grid grid; // 그리드 시스템
        public Tilemap groundMap;
        public Tilemap objectMap;

        // manager에서 드롭 아이템에 사용한 위치값 관리
        private HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();

        [SerializeField, Sirenix.OdinInspector.ReadOnly] private SerializableDictionary<Vector3Int, GameObject> objectMapData = new();

        private void Awake()
        {
            Instance = this; // 싱글톤 인스턴스 설정
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 시
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    Vector3Int clickCellPos = grid.WorldToCell(hitInfo.point);
                    bool isEmptyCell = IsEmptyGroundCell(hitInfo.point);
                    Debug.Log("Clicked Cell Position: " + clickCellPos + ", Is Empty: " + isEmptyCell);
                }
            }


        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null; // 싱글톤 인스턴스 해제
            }
        }

        public Vector3Int GetWorldToCell(Vector3 position)
        {
            // 월드 좌표를 그리드 셀 좌표로 변환
            return grid.WorldToCell(position);
        }

        public Vector3 GetCellToWorld(Vector3Int cellPosition)
        {
            // 그리드 셀 좌표를 월드 좌표로 변환
            return grid.GetCellCenterWorld(cellPosition);
        }

        public Vector3Int GetClockwiseEmptyCellFromObjectMap(Vector3Int pivot)
        {
            Vector3Int[] directions = new Vector3Int[]
            {
                new Vector3Int(0, 1, 0),    // 12시 방향
                new Vector3Int(1, 1, 0),    // 1시 방향
                new Vector3Int(1, 0, 0),    // 3시 방향
                new Vector3Int(1, -1, 0),   // 5시 방향
                new Vector3Int(0, -1, 0),   // 6시 방향
                new Vector3Int(-1, -1, 0),  // 7시 방향
                new Vector3Int(-1, 0, 0),   // 9시 방향
                new Vector3Int(-1, 1, 0),   // 11시 방향
            };

            foreach (var direction in directions)
            {
                Vector3Int targetCell = pivot + direction;
                if (!objectMap.HasTile(targetCell) && !usedPositions.Contains(targetCell))
                {
                    usedPositions.Add(targetCell); // 사용된 위치에 추가
                    return targetCell;
                }
            }

            return pivot; // 못 찾으면 pivot 반환
        }

        public void ResetUsedPositions(Vector3Int pivot)
        {
            usedPositions.Remove(pivot); // 특정 pivot 위치 제거
        }

        public void RegistObejctToObjectMap(GameObject go, Vector3 position)
        {
            Vector3Int cellPosition = objectMap.WorldToCell(position);
            objectMapData.Add(cellPosition, go);
        }

        public bool IsEmptyGroundCell(Vector3 worldPos)
        {
            return groundMap.HasTile(groundMap.WorldToCell(worldPos))
                && !objectMapData.ContainsKey(objectMap.WorldToCell(worldPos));
        }
    }
}
