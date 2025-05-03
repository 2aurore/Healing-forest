using System.Collections;
using System.Collections.Generic;
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

        // TODO: HashSet<Vector3Int> used manager에서 관리하도록 변경
        private HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();


        private void Awake()
        {
            Instance = this; // 싱글톤 인스턴스 설정
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

        // TODO: 사용된 위치를 초기화하는 메서드 추가 필요

    }
}
