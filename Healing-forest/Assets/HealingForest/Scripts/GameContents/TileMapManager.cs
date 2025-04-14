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
            // 시계 방향으로 빈 셀을 찾는 메소드
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
                if (!objectMap.HasTile(targetCell))
                {
                    return targetCell; // 빈 셀을 찾으면 반환
                }
            }

            return pivot; // 빈 셀이 없으면 원래 위치 반환
        }
    }
}
