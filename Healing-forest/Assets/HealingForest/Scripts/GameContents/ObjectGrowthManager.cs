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

        [Header("Growth Constraints")]
        [Tooltip("빈 공간을 찾기 위한 최대 시도 횟수")]
        public int maxAttempts = 5;
        [Tooltip("기존 오브젝트 주변에서 제외할 거리 (1 = 8방향)")]
        public int exclusionRadius = 1;

        [Header("Ground Tile Sampling")]
        [Tooltip("Ground 타일 위치들을 미리 캐시할지 여부 (성능 향상)")]
        public bool cacheGroundTiles = true;
        [Tooltip("캐시 업데이트 간격 (초) - 0이면 시작시 한번만")]
        public float cacheUpdateInterval = 0f;

        private TileMapManager tileMapManager;
        private List<Vector3Int> cachedGroundTilePositions = new List<Vector3Int>();
        private float lastCacheUpdateTime = 0f;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            tileMapManager = TileMapManager.Instance;

            if (tileMapManager == null)
            {
                Debug.LogError("TileMapManager instance not found!");
                return;
            }

            // Ground 타일 캐시 초기화
            if (cacheGroundTiles)
            {
                UpdateGroundTileCache();
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

        private void Update()
        {
            // 캐시 업데이트 체크
            if (cacheGroundTiles && cacheUpdateInterval > 0f &&
                Time.time - lastCacheUpdateTime > cacheUpdateInterval)
            {
                UpdateGroundTileCache();
            }
        }

        private void OnDestroy()
        {
            if (DayNightCycleController.Instance != null)
            {
                DayNightCycleController.Instance.OnDayChanged -= OnDayComplete;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Ground Tilemap에서 실제 타일이 있는 모든 위치를 캐시
        /// </summary>
        private void UpdateGroundTileCache()
        {
            cachedGroundTilePositions.Clear();

            if (tileMapManager?.groundMap == null)
            {
                Debug.LogError("Ground Tilemap not found!");
                return;
            }

            BoundsInt bounds = tileMapManager.groundMap.cellBounds;

            Debug.Log($"Ground Tilemap 스캔 시작: {bounds.min} ~ {bounds.max}");

            int totalCells = 0;
            int groundTileCells = 0;

            // Ground Tilemap의 모든 셀을 순회
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    totalCells++;
                    Vector3Int position = new Vector3Int(x, y, 0);

                    // 해당 위치에 타일이 있는지 확인
                    TileBase tile = tileMapManager.groundMap.GetTile(position);
                    if (tile != null)
                    {
                        cachedGroundTilePositions.Add(position);
                        groundTileCells++;
                    }
                }
            }

            lastCacheUpdateTime = Time.time;

            Debug.Log($"Ground 타일 캐시 완료: {groundTileCells}/{totalCells} 셀 ({(float)groundTileCells / totalCells * 100f:F1}%)");
        }

        /// <summary>
        /// Ground 타일이 있는 위치에서 랜덤하게 선택
        /// </summary>
        private Vector3Int GetRandomGroundTilePosition()
        {
            if (cacheGroundTiles)
            {
                // 캐시 사용
                if (cachedGroundTilePositions.Count == 0)
                {
                    Debug.LogWarning("캐시된 Ground 타일이 없습니다. 캐시를 업데이트합니다.");
                    UpdateGroundTileCache();
                }

                if (cachedGroundTilePositions.Count > 0)
                {
                    int randomIndex = Random.Range(0, cachedGroundTilePositions.Count);
                    return cachedGroundTilePositions[randomIndex];
                }
            }
            else
            {
                // 실시간 검색 (느리지만 항상 최신)
                return GetRandomGroundTilePositionRealtime();
            }

            Debug.LogError("Ground 타일을 찾을 수 없습니다!");
            return Vector3Int.zero;
        }

        /// <summary>
        /// 실시간으로 Ground 타일 위치를 찾는 방법 (캐시 미사용)
        /// </summary>
        private Vector3Int GetRandomGroundTilePositionRealtime()
        {
            BoundsInt bounds = tileMapManager.groundMap.cellBounds;
            int attempts = 0;
            int maxSearchAttempts = 1000; // 무한루프 방지

            while (attempts < maxSearchAttempts)
            {
                attempts++;

                int randomX = Random.Range(bounds.xMin, bounds.xMax);
                int randomY = Random.Range(bounds.yMin, bounds.yMax);
                Vector3Int position = new Vector3Int(randomX, randomY, 0);

                TileBase tile = tileMapManager.groundMap.GetTile(position);
                if (tile != null)
                {
                    return position;
                }
            }

            Debug.LogWarning($"실시간 검색으로 Ground 타일을 찾지 못했습니다. ({attempts}회 시도)");
            return Vector3Int.zero;
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

                // Ground 타일이 있는 랜덤한 위치 선택
                Vector3Int randomPosition = GetRandomGroundTilePosition();

                // 해당 위치가 유효한지 확인 (오브젝트 겹침, 거리 등)
                if (IsValidGrowthPosition(randomPosition))
                {
                    // 나무 또는 돌 중 랜덤하게 선택
                    GameObject selectedPrefab = SelectRandomPrefab();

                    if (selectedPrefab != null)
                    {
                        // 오브젝트 생성
                        Vector3 worldPosition = tileMapManager.GetCellToWorld(randomPosition);

                        GameObject newObject = Instantiate(selectedPrefab, Vector3.zero,
                            Quaternion.Euler(0, Random.Range(0, 360), 0),
                            tileMapManager.objectMap.transform);

                        // 위치 설정
                        Vector3 localPosition = tileMapManager.objectMap.transform.InverseTransformPoint(worldPosition);
                        localPosition.y = 0f;
                        newObject.transform.localPosition = localPosition;

                        // 컴포넌트 및 태그 설정
                        if (IsTreePrefab(selectedPrefab) && newObject.GetComponent<TreeObject>() == null)
                        {
                            newObject.AddComponent<TreeObject>();
                        }

                        if (IsRockPrefab(selectedPrefab) && newObject.GetComponent<Rock>() == null)
                        {
                            newObject.AddComponent<Rock>();
                        }

                        // TileMapManager에 등록
                        Vector3 finalWorldPosition = newObject.transform.position;
                        tileMapManager.RegistObejctToObjectMap(newObject, finalWorldPosition);

                        // 등록 후 위치 보정
                        Vector3 correctedPosition = newObject.transform.position;
                        Vector3 correctedLocal = tileMapManager.objectMap.transform.InverseTransformPoint(correctedPosition);
                        correctedLocal.y = 0f;
                        newObject.transform.localPosition = correctedLocal;

                        objectsGrown++;
                        string objectType = IsTreePrefab(selectedPrefab) ? "Tree" : "Rock";
                        Debug.Log($"{objectType} {objectsGrown} grown at position: {randomPosition}");

                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }

            Debug.Log($"Object growth completed! {objectsGrown} objects grown in {attempts} attempts.");
        }

        /// <summary>
        /// 위치 유효성 검사 - Ground 타일 체크는 이미 GetRandomGroundTilePosition에서 했으므로 제외
        /// </summary>
        private bool IsValidGrowthPosition(Vector3Int position)
        {
            Debug.Log($"=== Validating Position {position} ===");

            // 1. Object Tilemap에 이미 타일이 있는지 확인
            bool hasObjectTile = tileMapManager.objectMap.HasTile(position);
            Debug.Log($"1. Object Tile exists at {position}: {hasObjectTile}");
            if (hasObjectTile)
            {
                Debug.Log($"❌ Object tile already exists at {position}");
                return false;
            }

            // 2. TileMapManager의 objectMapData에 이미 등록된 오브젝트가 있는지 확인
            bool hasObjectData = tileMapManager.objectMapData.ContainsKey(position);
            Debug.Log($"2. ObjectMapData contains {position}: {hasObjectData}");
            if (hasObjectData)
            {
                Debug.Log($"❌ ObjectMapData already contains {position}");
                return false;
            }

            // 3. 기존 오브젝트들과의 exclusionRadius 거리 확인
            bool isNearObjects = IsNearExistingObjects(position);
            Debug.Log($"3. IsNearExistingObjects: {isNearObjects}");
            if (isNearObjects)
            {
                Debug.Log($"❌ Too close to existing objects at {position}");
                return false;
            }

            Debug.Log($"✅ Position {position} is valid for growth");
            return true;
        }

        private bool IsNearExistingObjects(Vector3Int position)
        {
            for (int x = -exclusionRadius; x <= exclusionRadius; x++)
            {
                for (int y = -exclusionRadius; y <= exclusionRadius; y++)
                {
                    Vector3Int checkPosition = position + new Vector3Int(x, y, 0);

                    if (tileMapManager.objectMapData.ContainsKey(checkPosition))
                    {
                        GameObject existingObject = tileMapManager.objectMapData[checkPosition];

                        if (existingObject != null)
                        {
                            return true;
                        }
                        else
                        {
                            tileMapManager.objectMapData.Remove(checkPosition);
                        }
                    }
                }
            }

            return false;
        }

        private GameObject SelectRandomPrefab()
        {
            List<GameObject> availablePrefabs = new List<GameObject>();

            if (treePrefabs.Count > 0 && Random.Range(0f, 100f) < treeSpawnChance)
            {
                availablePrefabs.AddRange(treePrefabs);
            }

            if (rockPrefabs.Count > 0 && Random.Range(0f, 100f) < rockSpawnChance)
            {
                availablePrefabs.AddRange(rockPrefabs);
            }

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

        /// <summary>
        /// 사용 가능한 빈 공간의 개수를 반환
        /// </summary>
        public int GetAvailableSpaceCount()
        {
            if (cacheGroundTiles)
            {
                int availableCount = 0;
                foreach (Vector3Int position in cachedGroundTilePositions)
                {
                    if (IsValidGrowthPosition(position))
                    {
                        availableCount++;
                    }
                }
                return availableCount;
            }
            else
            {
                // 실시간 계산 (느림)
                BoundsInt bounds = tileMapManager.groundMap.cellBounds;
                int count = 0;

                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    for (int y = bounds.yMin; y < bounds.yMax; y++)
                    {
                        Vector3Int position = new Vector3Int(x, y, 0);
                        TileBase tile = tileMapManager.groundMap.GetTile(position);

                        if (tile != null && IsValidGrowthPosition(position))
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
        }


    }
}