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

        // manager에서 드롭 아이템에 사용한 위치값 관리
        private HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();

        [SerializeField, Sirenix.OdinInspector.ReadOnly] public SerializableDictionary<Vector3Int, GameObject> objectMapData = new();

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
                    // Debug.Log("Clicked Cell Position: " + clickCellPos + ", Is Empty: " + isEmptyCell);
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

                // Ground 타일이 있는지 확인
                TileBase groundTile = groundMap.GetTile(targetCell);
                bool hasGroundTile = groundTile != null;

                // 타일맵에 타일이 없고, 사용된 위치에도 없고, objectMapData에도 등록되지 않은 빈 셀
                // 그리고 Ground tilemap에 타일이 있어야 함
                if (hasGroundTile &&
                    !objectMap.HasTile(targetCell) &&
                    !usedPositions.Contains(targetCell) &&
                    !objectMapData.ContainsKey(targetCell))
                {
                    usedPositions.Add(targetCell); // 사용된 위치에 추가
                    Debug.Log($"빈 Ground 셀을 찾았습니다: {targetCell} (Ground 타일: {groundTile.name})");
                    return targetCell;
                }
            }

            Debug.LogWarning($"pivot {pivot} 주변에서 유효한 Ground 타일이 있는 빈 셀을 찾을 수 없습니다.");
            return pivot; // 못 찾으면 pivot 반환
        }

        public void ResetUsedPositions(Vector3Int pivot)
        {
            usedPositions.Remove(pivot); // 특정 pivot 위치 제거

            // objectMapData에서도 함께 제거
            if (objectMapData.ContainsKey(pivot))
            {
                objectMapData.Remove(pivot);
            }
        }

        public void RegistObejctToObjectMap(GameObject go, Vector3 position)
        {
            Vector3Int cellPosition = objectMap.WorldToCell(position);

            // 해당 셀이 이미 사용 중이면 빈 셀을 찾아서 등록
            if (objectMapData.ContainsKey(cellPosition))
            {
                Debug.LogWarning($"셀 위치 {cellPosition}에 이미 오브젝트가 존재합니다. 빈 셀을 찾아 이동합니다.");
                Vector3Int emptyCellPosition = GetClockwiseEmptyCellFromObjectMap(cellPosition);

                if (emptyCellPosition != cellPosition) // 빈 셀을 찾았다면
                {
                    // 새 위치가 실제로 Ground 타일이 있는지 다시 한 번 확인
                    TileBase groundTile = groundMap.GetTile(emptyCellPosition);
                    if (groundTile == null)
                    {
                        Debug.LogError($"새 위치 {emptyCellPosition}에 Ground 타일이 없습니다. 오브젝트 등록 실패: {go.name}");
                        return;
                    }

                    // GameObject의 위치도 새로운 셀 위치로 이동
                    Vector3 newWorldPosition = GetCellToWorld(emptyCellPosition);

                    // Object Tilemap의 자식인지 확인
                    if (go.transform.parent == objectMap.transform)
                    {
                        // 자식이라면 로컬 좌표로 설정 (Y=0 유지)
                        Vector3 newLocalPosition = objectMap.transform.InverseTransformPoint(newWorldPosition);
                        newLocalPosition.y = 0f; // 로컬 Y를 0으로 유지
                        go.transform.localPosition = newLocalPosition;
                        Debug.Log($"오브젝트를 {emptyCellPosition} 위치로 이동했습니다. (Local Y=0 유지, Ground 타일: {groundTile.name})");
                    }

                    objectMapData.Add(emptyCellPosition, go);
                }
                else
                {
                    Debug.LogError($"빈 셀을 찾을 수 없습니다. 오브젝트 등록에 실패했습니다: {go.name}");
                }
            }
            else
            {
                // 첫 번째 위치도 Ground 타일이 있는지 확인
                TileBase groundTile = groundMap.GetTile(cellPosition);
                if (groundTile == null)
                {
                    Debug.LogError($"위치 {cellPosition}에 Ground 타일이 없습니다. 오브젝트 등록 실패: {go.name}");
                    return;
                }

                objectMapData.Add(cellPosition, go);
                Debug.Log($"오브젝트를 원래 위치 {cellPosition}에 등록했습니다. (Ground 타일: {groundTile.name})");
            }
        }

        public bool IsEmptyGroundCell(Vector3 worldPos)
        {
            // Ground Tilemap에서 셀 좌표 계산
            Vector3Int groundCellPos = groundMap.WorldToCell(worldPos);

            // Ground 타일이 있는지 확인
            bool hasGroundTile = groundMap.GetTile(groundCellPos) != null;

            // Object Tilemap에서 셀 좌표 계산
            Vector3Int objectCellPos = objectMap.WorldToCell(worldPos);

            // Object 데이터에 등록되어 있는지 확인
            bool hasObjectData = objectMapData.ContainsKey(objectCellPos);

            // 디버그 로그 (필요시 주석 해제)
            // Debug.Log($"IsEmptyGroundCell({worldPos}): Ground({groundCellPos})={hasGroundTile}, Object({objectCellPos})={hasObjectData}");

            return hasGroundTile && !hasObjectData;
        }
    }
}
