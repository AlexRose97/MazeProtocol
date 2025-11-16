using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;

namespace MazeScripts
{
    public class Maze : MonoBehaviour
    {
        [Header("View")]
        public Transform cameraTransform;         // Cámara top-down inicial
        public GameObject characterPrefab;        // Prefab del jugador (con su cámara)
        public TextMeshProUGUI countdownText;     // Texto para mensajes y contador
        public float previewDuration = 5f;

        [Header("Prefabs")]
        public GameObject cellPrefab;

        [Header("Size")]
        public int width = 10;
        public int height = 10;

        [Header("Cell")]
        public float cellSize = 4f;
        public float spawnStepDelay = 0.02f;

        [Header("Random")]
        public int randomSeed = 0;                // Si es 0, se genera una semilla al vuelo

        [Header("Navigation")]
        public NavMeshSurface navMeshSurface;

        [Header("Enemies")]
        public GameObject enemyPrefab;
        [Range(0f, 1f)]
        public float enemyDensity = 0.25f;        // 0.25 = 1 enemigo cada 4 celdas

        private Cell[,] _cellGrid;
        private int _effectiveSeed;
        private System.Random _rng;

        private void Awake()
        {
            _effectiveSeed = randomSeed != 0 ? randomSeed : Environment.TickCount;
            _rng = new System.Random(_effectiveSeed);
        }

        private IEnumerator Start()
        {
            _cellGrid = new Cell[width, height];

            SetupCameraTopDown();
            SetTextVisible(true, "Generando...");

            var generator = new MazeGenerator(width, height, _effectiveSeed);
            generator.Generate();

            yield return SpawnCellsFromData(generator);

            if (navMeshSurface != null)
            {
                navMeshSurface.BuildNavMesh();
            }

            OpenEntranceAndExit();
            SpawnEnemies();

            yield return PreviewAndSpawn();
        }

        private void SetupCameraTopDown()
        {
            if (cameraTransform == null) return;

            float centerX = (width - 1) * cellSize / 2f;
            float centerZ = (height - 1) * cellSize / 2f;

            cameraTransform.position = new Vector3(
                centerX,
                Mathf.Max(width, height) * cellSize,
                centerZ
            );

            cameraTransform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        private void SetTextVisible(bool visible, string text = null)
        {
            if (countdownText == null) return;

            countdownText.gameObject.SetActive(visible);
            if (visible && text != null)
            {
                countdownText.text = text;
            }
        }

        private IEnumerator SpawnCellsFromData(MazeGenerator generator)
        {
            for (int x = 0; x < generator.Width; x++)
            {
                for (int y = 0; y < generator.Height; y++)
                {
                    Vector3 pos = new Vector3(x * cellSize, 0f, y * cellSize);
                    GameObject cellGO = Instantiate(cellPrefab, pos, Quaternion.identity, transform);

                    Cell cell = cellGO.GetComponent<Cell>();
                    _cellGrid[x, y] = cell;

                    MazeCellData data = generator.Cells[x, y];

                    for (int i = 0; i < 4; i++)
                    {
                        if (!data.Walls[i])
                        {
                            cell.HideWall((WallOrientation)i);
                        }
                    }

                    if (spawnStepDelay > 0f)
                        yield return new WaitForSeconds(spawnStepDelay);
                    else
                        yield return null;
                }
            }
        }

        private void OpenEntranceAndExit()
        {
            Cell start = _cellGrid[0, 0];
            Cell end = _cellGrid[width - 1, height - 1];

            start.HideWall(WallOrientation.WEST);
            end.HideWall(WallOrientation.EAST);
        }

        private IEnumerator PreviewAndSpawn()
        {
            float remaining = previewDuration;
            SetTextVisible(true);

            while (remaining > 0f)
            {
                if (countdownText != null)
                {
                    countdownText.text = Mathf.CeilToInt(remaining).ToString();
                }

                yield return new WaitForSeconds(1f);
                remaining -= 1f;
            }

            SetTextVisible(false);
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            if (cameraTransform != null)
            {
                Destroy(cameraTransform.gameObject);
            }

            Vector3 position = new Vector3(0f, 1.5f, 0f);

            GameObject player = null;

            if (characterPrefab != null)
            {
                player = Instantiate(characterPrefab, position, Quaternion.identity);
            }

            if (player == null) return;

            var chasers = FindObjectsOfType<SimpleChaser>();
            foreach (var c in chasers)
            {
                c.SetTarget(player.transform);
            }
        }

        private void SpawnEnemies()
        {
            if (enemyPrefab == null) return;

            int totalCells = width * height;
            int enemyCount = Mathf.FloorToInt(totalCells * enemyDensity);
            if (enemyCount <= 0) return;

            var usedPositions = new HashSet<Vector2Int>
            {
                new Vector2Int(0, 0),
                new Vector2Int(width - 1, height - 1)
            };

            int maxAttempts = totalCells * 3;
            int spawned = 0;
            int attempts = 0;

            while (spawned < enemyCount && attempts < maxAttempts)
            {
                attempts++;

                int x = _rng.Next(0, width);
                int y = _rng.Next(0, height);

                var cellPos = new Vector2Int(x, y);
                if (usedPositions.Contains(cellPos)) continue;

                Cell cell = _cellGrid[x, y];
                if (cell == null) continue;

                Vector3 pos = cell.transform.position + new Vector3(0f, 1.5f, 0f);
                Instantiate(enemyPrefab, pos, Quaternion.identity);

                usedPositions.Add(cellPos);
                spawned++;
            }
        }

        private void OnDrawGizmos()
        {
            if (_cellGrid == null) return;

            Gizmos.color = Color.red;

            foreach (var cell in _cellGrid)
            {
                if (cell != null)
                {
                    Gizmos.DrawSphere(cell.transform.position, 0.1f);
                }
            }
        }
    }
}
