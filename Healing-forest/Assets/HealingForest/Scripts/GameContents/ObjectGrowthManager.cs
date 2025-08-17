using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HF
{
    public class ObjectGrowthManager : MonoBehaviour
    {
        public static ObjectGrowthManager Instance { get; private set; }

        [Header("Growth Settings")]
        [Tooltip("하루당 생성할 오브젝트의 최소 개수")]
        public int minObjectsPerDay = 1;
        [Tooltip("하루당 생성할 오브젝트의 최대 개수")]
        public int maxObjectsPerDay = 10;

        [Header("Tree Settings")]
        [Tooltip("생성할 나무 프리팹들")]
        public List<GameObject> treePrefabs = new List<GameObject>();
        [Tooltip("나무가 생성될 확률 (0~100)")]
        [Range(0f, 100f)] public float treeSpawnChance = 70f;

        [Header("Rock Settings")]
        [Tooltip("생성할 돌 프리팹들")]
        public List<GameObject> rockPrefabs = new List<GameObject>();
        [Tooltip("돌이 생성될 확률 (0~100)")]
        [Range(0f, 100f)] public float rockSpawnChance = 30f;

        [Header("Growth Area Settings")]
        [Tooltip("오브젝트가 자랄 수 있는 최소 범위")]
        public Vector2Int growthAreaMin = new Vector2Int(-50, -50);
        [Tooltip("오브젝트가 자랄 수 있는 최대 범위")]
        public Vector2Int growthAreaMax = new Vector2Int(50, 50);

        [Header("Growth Constraints")]
        [Tooltip("빈 공간을 찾기 위한 최대 시도 횟수")]
        public int maxAttempts = 100;
        [Tooltip("기존 오브젝트 주변에서 제외할 거리 (1 = 8방향)")]
        public int exclusionRadius = 1;

        private TileMapManager tileMapManager;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // TileMapManager 참조 획득
            tileMapManager = TileMapManager.Instance;

            if (tileMapManager == null)
            {
                Debug.LogError("TileMapManager instance not found!");
                return;
            }

            // DayNightCycleController의 OnDayChanged 이벤트에 구독
            if (DayNightCycleController.Instance != null)
            {
                DayNightCycleController.Instance.OnDayChanged += OnDayComplete;
            }
            else
            {
                Debug.LogError("DayNightCycleController instance not found!");
            }
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (DayNightCycleController.Instance != null)
            {
                DayNightCycleController.Instance.OnDayChanged -= OnDayComplete;
            }
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnDayComplete()
        {
            Debug.Log("날짜가 변경됩니다. 오브젝트를 성장시킵니다.");
            StartCoroutine(GrowObjectsCoroutine());
        }

        private IEnumerator GrowObjectsCoroutine()
        {
            if (treePrefabs.Count == 0 && rockPrefabs.Count == 0)
            {
                Debug.LogWarning("생성에 필요한 프리팹이 없습니다. 나무나 돌 프리팹을 추가해주세요.");
                yield break;
            }

            // 랜덤한 오브젝트 개수 결정
            int objectsToGrow = Random.Range(minObjectsPerDay, maxObjectsPerDay + 1);
            Debug.Log($"Growing {objectsToGrow} objects...");

            int objectsGrown = 0;
            int attempts = 0;

            while (objectsGrown < objectsToGrow && attempts < maxAttempts)
            {
                attempts++;

                // 랜덤한 위치 선택
                Vector3Int randomPosition = GetRandomGrowthPosition();

                // 해당 위치가 유효한지 확인
                if (IsValidGrowthPosition(randomPosition))
                {
                    // 나무 또는 돌 중 랜덤하게 선택
                    GameObject selectedPrefab = SelectRandomPrefab();

                    if (selectedPrefab != null)
                    {
                        // 디버그: Object Tilemap Transform 확인
                        Debug.Log($"Object Tilemap Transform Y: {tileMapManager.objectMap.transform.position.y}");

                        // 오브젝트 생성 - Object Tilemap의 자식으로 설정
                        Vector3 worldPosition = tileMapManager.GetCellToWorld(randomPosition);
                        Debug.Log($"Grid Cell {randomPosition} to World: {worldPosition}");

                        // Object Tilemap의 자식으로 생성 (임시로 원점에 생성)
                        GameObject newObject = Instantiate(selectedPrefab, Vector3.zero,
                        Quaternion.Euler(0, Random.Range(0, 360), 0), // 랜덤 회전
                        tileMapManager.objectMap.transform); // Object Tilemap의 자식으로 설정

                        Debug.Log($"After Instantiate - Object World Position: {newObject.transform.position}");
                        Debug.Log($"After Instantiate - Object Local Position: {newObject.transform.localPosition}");

                        // 자식 오브젝트의 로컬 좌표 설정 (Y=0으로 고정)
                        Vector3 localPosition = tileMapManager.objectMap.transform.InverseTransformPoint(worldPosition);
                        Debug.Log($"Calculated Local Position (before Y fix): {localPosition}");

                        localPosition.y = 0f; // 로컬 Y 좌표를 0으로 설정
                        newObject.transform.localPosition = localPosition;

                        Debug.Log($"Final Local Position: {newObject.transform.localPosition}");
                        Debug.Log($"Final World Position: {newObject.transform.position}");

                        // TreeObject 컴포넌트가 없다면 추가 (나무인 경우)
                        if (IsTreePrefab(selectedPrefab) && newObject.GetComponent<TreeObject>() == null)
                        {
                            newObject.AddComponent<TreeObject>();
                        }

                        // 돌인 경우 태그 설정
                        if (IsRockPrefab(selectedPrefab))
                        {
                            if (!newObject.CompareTag("Rock") && !newObject.CompareTag("Stone"))
                            {
                                newObject.tag = "Rock";
                            }
                        }

                        // TileMapManager의 objectMapData에 등록 (월드 좌표 사용)
                        Vector3 finalWorldPosition = newObject.transform.position;
                        Vector3 positionBeforeReg = finalWorldPosition;

                        tileMapManager.RegistObejctToObjectMap(newObject, finalWorldPosition);

                        Vector3 positionAfterReg = newObject.transform.position;
                        Debug.Log($"Before Registration: {positionBeforeReg}");
                        Debug.Log($"After Registration: {positionAfterReg}");

                        // RegistObejctToObjectMap에서 위치가 변경되었다면 다시 올바른 Y 좌표로 수정
                        if (positionBeforeReg != positionAfterReg)
                        {
                            Debug.Log("Position changed by RegistObejctToObjectMap - fixing Y coordinate");
                            Vector3 correctedPosition = newObject.transform.position;

                            // Object Tilemap의 자식이니까 로컬 Y=0으로 수정
                            Vector3 correctedLocal = tileMapManager.objectMap.transform.InverseTransformPoint(correctedPosition);
                            correctedLocal.y = 0f;
                            newObject.transform.localPosition = correctedLocal;

                            Debug.Log($"Corrected Final Position: {newObject.transform.position}");
                        }

                        objectsGrown++;
                        string objectType = IsTreePrefab(selectedPrefab) ? "Tree" : "Rock";
                        Debug.Log($"{objectType} {objectsGrown} grown at position: {randomPosition} (Local: {localPosition}, World: {finalWorldPosition})");

                        // 약간의 지연을 주어 자연스럽게 보이도록
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }

            Debug.Log($"Object growth completed! {objectsGrown} objects grown in {attempts} attempts.");
        }

        private GameObject SelectRandomPrefab()
        {
            // 사용 가능한 프리팹 리스트 생성
            List<GameObject> availablePrefabs = new List<GameObject>();

            // 확률에 따라 나무 추가
            if (treePrefabs.Count > 0 && Random.Range(0f, 100f) < treeSpawnChance)
            {
                availablePrefabs.AddRange(treePrefabs);
            }

            // 확률에 따라 돌 추가
            if (rockPrefabs.Count > 0 && Random.Range(0f, 100f) < rockSpawnChance)
            {
                availablePrefabs.AddRange(rockPrefabs);
            }

            // 아무것도 선택되지 않은 경우 기본적으로 나무 선택
            if (availablePrefabs.Count == 0)
            {
                if (treePrefabs.Count > 0)
                {
                    availablePrefabs.AddRange(treePrefabs);
                }
                else if (rockPrefabs.Count > 0)
                {
                    availablePrefabs.AddRange(rockPrefabs);
                }
            }

            if (availablePrefabs.Count > 0)
            {
                return availablePrefabs[Random.Range(0, availablePrefabs.Count)];
            }

            return null;
        }

        private bool IsTreePrefab(GameObject prefab)
        {
            return treePrefabs.Contains(prefab);
        }

        private bool IsRockPrefab(GameObject prefab)
        {
            return rockPrefabs.Contains(prefab);
        }

        private Vector3Int GetRandomGrowthPosition()
        {
            // Ground Tilemap의 범위를 고려하여 더 효율적인 위치 선택
            BoundsInt groundBounds = tileMapManager.groundMap.cellBounds;

            // Growth Area와 Ground Tilemap 범위의 교집합 계산
            int minX = Mathf.Max(growthAreaMin.x, groundBounds.xMin);
            int maxX = Mathf.Min(growthAreaMax.x, groundBounds.xMax - 1);
            int minY = Mathf.Max(growthAreaMin.y, groundBounds.yMin);
            int maxY = Mathf.Min(growthAreaMax.y, groundBounds.yMax - 1);

            // 유효한 범위가 있는지 확인
            if (minX > maxX || minY > maxY)
            {
                Debug.LogWarning("Growth area does not overlap with Ground Tilemap bounds!");
                // 기본 방식으로 폴백
                int x = Random.Range(growthAreaMin.x, growthAreaMax.x + 1);
                int y = Random.Range(growthAreaMin.y, growthAreaMax.y + 1);
                return new Vector3Int(x, y, 0);
            }

            // 교집합 범위 내에서 랜덤 위치 선택
            int randomX = Random.Range(minX, maxX + 1);
            int randomY = Random.Range(minY, maxY + 1);
            return new Vector3Int(randomX, randomY, 0);
        }

        private bool IsValidGrowthPosition(Vector3Int position)
        {
            Debug.Log($"=== Validating Position {position} ===");

            // 1. Ground Tilemap에 실제로 타일이 있는지 확인 (물 위 생성 방지)
            TileBase groundTile = tileMapManager.groundMap.GetTile(position);
            Debug.Log($"1. Ground Tile at {position}: {(groundTile != null ? groundTile.name : "NULL")}");
            if (groundTile == null)
            {
                Debug.Log($"❌ Ground tile is NULL at {position}");
                return false;
            }

            // 2. Object Tilemap에 이미 타일이 있는지 확인 (충돌 방지)
            bool hasObjectTile = tileMapManager.objectMap.HasTile(position);
            Debug.Log($"2. Object Tile exists at {position}: {hasObjectTile}");
            if (hasObjectTile)
            {
                Debug.Log($"❌ Object tile already exists at {position}");
                return false;
            }

            // 3. TileMapManager의 objectMapData에 이미 등록된 오브젝트가 있는지 확인
            bool hasObjectData = tileMapManager.objectMapData.ContainsKey(position);
            Debug.Log($"3. ObjectMapData contains {position}: {hasObjectData}");
            if (hasObjectData)
            {
                Debug.Log($"❌ ObjectMapData already contains {position}");
                return false;
            }

            // 4. TileMapManager의 IsEmptyGroundCell을 활용하여 추가 검사 (이중 안전장치)
            Vector3 worldPosition = tileMapManager.GetCellToWorld(position);
            bool isEmpty = tileMapManager.IsEmptyGroundCell(worldPosition);
            Debug.Log($"4. IsEmptyGroundCell at {worldPosition}: {isEmpty}");
            if (!isEmpty)
            {
                Debug.Log($"❌ IsEmptyGroundCell returned false for {position}");
                return false;
            }

            // 5. 기존 오브젝트들과의 exclusionRadius 거리 확인
            bool isNearObjects = IsNearExistingObjects(position);
            Debug.Log($"5. IsNearExistingObjects: {isNearObjects}");
            if (isNearObjects)
            {
                Debug.Log($"❌ Too close to existing objects at {position}");
                return false;
            }

            Debug.Log($"✅ Position {position} is valid for growth");
            return true;
        }

        /// <summary>
        /// 특정 위치가 기존 오브젝트들의 exclusionRadius 범위 내에 있는지 확인
        /// </summary>
        /// <param name="position">확인할 셀 좌표</param>
        /// <returns>기존 오브젝트 근처면 true, 아니면 false</returns>
        private bool IsNearExistingObjects(Vector3Int position)
        {
            // TileMapManager의 objectMapData를 활용하여 효율적으로 검사
            for (int x = -exclusionRadius; x <= exclusionRadius; x++)
            {
                for (int y = -exclusionRadius; y <= exclusionRadius; y++)
                {
                    Vector3Int checkPosition = position + new Vector3Int(x, y, 0);

                    // objectMapData에 해당 위치에 오브젝트가 등록되어 있는지 확인
                    if (tileMapManager.objectMapData.ContainsKey(checkPosition))
                    {
                        GameObject existingObject = tileMapManager.objectMapData[checkPosition];

                        // 오브젝트가 실제로 존재하는지 확인 (삭제되었을 수도 있음)
                        if (existingObject != null)
                        {
                            return true;
                        }
                        else
                        {
                            // 삭제된 오브젝트면 objectMapData에서 제거
                            tileMapManager.objectMapData.Remove(checkPosition);
                        }
                    }
                }
            }

            return false;
        }


    }
}
