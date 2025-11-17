using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using TMPro;
using UnityEngine;
using Unity.AI.Navigation;
using Enemies;
using GameScripts;

namespace MazeScripts
{
    public class Maze : MonoBehaviour
    {
        [Header("View")]
        public Transform cameraTransform;
        public GameObject characterPrefab;
        public TextMeshProUGUI countdownText;
        public float previewDuration = 5f;

        [Header("Timer")]
        public TextMeshProUGUI timerText;
        
        [Header("Menus")]
        [SerializeField] private InGameMenuController inGameMenu;
        
        [Header("Prefabs")]
        public GameObject cellPrefab;

        [Header("Size")]
        public int width = 10;
        public int height = 10;

        [Header("Cell")]
        public float cellSize = 4f;
        public float spawnStepDelay = 0.02f;

        [Header("Random")]
        public int randomSeed = 0;

        [Header("Navigation")]
        public NavMeshSurface navMeshSurface;

        [Header("Enemies")]
        public GameObject enemyPrefab;
        [Range(0f, 1f)]
        public float enemyDensity = 0.25f;

        [Header("Goal")]
        public GameObject goalPrefab;

        private Cell[,] _cellGrid;
        private int _effectiveSeed;
        private System.Random _rng;

        private bool _gameEnded;
        private bool _gameRunning;
        private float _timeRemaining;

        private void Awake()
        {
            _effectiveSeed = randomSeed != 0 ? randomSeed : Environment.TickCount;
            _rng = new System.Random(_effectiveSeed);

            ApplyDifficulty();
        }

        private void ApplyDifficulty()
        {
            switch (GameDifficulty.Current)
            {
                case DifficultyLevel.Easy:
                    width = 5;
                    height = 5;
                    enemyDensity = 0.10f;
                    _timeRemaining = 60f;
                    break;
                case DifficultyLevel.Medium:
                    width = 10;
                    height = 10;
                    enemyDensity = 0.15f;
                    _timeRemaining = 45f;
                    break;
                case DifficultyLevel.Hard:
                    width = 15;
                    height = 15;
                    enemyDensity = 0.25f;
                    _timeRemaining = 30f;
                    break;
            }

            UpdateTimerUI();
        }

        private IEnumerator Start()
        {
            _cellGrid = new Cell[width, height];

            SetupCameraTopDown();
            SetMessageVisible(true, "Generando...");

            var generator = new MazeGenerator(width, height, _effectiveSeed);
            generator.Generate();

            yield return SpawnCellsFromData(generator);

            if (navMeshSurface != null)
            {
                navMeshSurface.BuildNavMesh();
            }

            OpenEntranceAndExit();
            SpawnGoal();
            SpawnEnemies();

            yield return PreviewAndSpawn();
        }

        private void Update()
        {
            HandlePauseInput();
            if (_gameEnded || !_gameRunning) return;

            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0f)
            {
                _timeRemaining = 0f;
                UpdateTimerUI();
                OnTimeOut();
            }
            else
            {
                UpdateTimerUI();
            }
        }

        private void SetupCameraTopDown()
        {
            if (!cameraTransform) return;

            float centerX = (width - 1) * cellSize / 2f;
            float centerZ = (height - 1) * cellSize / 2f;

            cameraTransform.position = new Vector3(
                centerX,
                Mathf.Max(width, height) * cellSize,
                centerZ
            );

            cameraTransform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        private void SetMessageVisible(bool visible, string text = null)
        {
            if (!countdownText) return;

            countdownText.gameObject.SetActive(visible);
            if (visible && text != null)
            {
                countdownText.text = text;
            }
        }

        private void UpdateTimerUI()
        {
            if (!timerText) return;

            int seconds = Mathf.CeilToInt(_timeRemaining);
            timerText.text = seconds.ToString();
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

            //start.HideWall(WallOrientation.WEST);
            end.HideWall(WallOrientation.EAST);
        }

        private void SpawnGoal()
        {
            if (!goalPrefab) return;

            Cell end = _cellGrid[width - 1, height - 1];
            Vector3 pos = end.transform.position + new Vector3(2f, 0.5f, 0f);

            Instantiate(goalPrefab, pos, Quaternion.identity);
        }

        private IEnumerator PreviewAndSpawn()
        {
            float remaining = previewDuration;
            SetMessageVisible(true);

            while (remaining > 0f)
            {
                if (countdownText)
                {
                    countdownText.text = Mathf.CeilToInt(remaining).ToString();
                }

                yield return new WaitForSeconds(1f);
                remaining -= 1f;
            }

            SetMessageVisible(false);
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            if (cameraTransform)
            {
                Destroy(cameraTransform.gameObject);
            }

            Vector3 position = new Vector3(0f, 1.5f, 0f);

            GameObject player = null;

            if (characterPrefab)
            {
                player = Instantiate(characterPrefab, position, Quaternion.identity);
            }

            if (!player) return;

            var playerTransform = player.transform;

            var chasers = FindObjectsOfType<SimpleChaser>();
            foreach (var c in chasers)
            {
                c.SetTarget(playerTransform);
            }

            var health = player.GetComponent<Health>();
            if (health != null)
            {
                health.OnDeath += OnPlayerDied;
            }

            _gameRunning = true;
        }

        private void SpawnEnemies()
        {
            if (!enemyPrefab) return;

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

        public void OnGoalReached()
        {
            if (_gameEnded) return;
            _gameEnded = true;
            _gameRunning = false;

            Time.timeScale = 0f;

            if (inGameMenu != null)
            {
                inGameMenu.ShowEndMenu(won: true);
            }
            else
            {
                Debug.Log("Ganaste");
            }
        }

        private void OnPlayerDied()
        {
            if (_gameEnded) return;
            _gameEnded = true;
            _gameRunning = false;

            Time.timeScale = 0f;

            if (inGameMenu != null)
            {
                inGameMenu.ShowEndMenu(won: false);
            }
            else
            {
                Debug.Log("Game Over");
            }
        }

        private void OnTimeOut()
        {
            if (_gameEnded) return;
            _gameEnded = true;
            _gameRunning = false;

            Time.timeScale = 0f;

            if (inGameMenu)
            {
                inGameMenu.ShowEndMenu(won: false, timeOut: true);
            }
            else
            {
                Debug.Log("Tiempo agotado");
            }
        }
        
        private void HandlePauseInput()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!inGameMenu) return;

                // Si ya terminó la partida, solo dejamos que usen el menú para ir al main
                if (_gameEnded)
                {
                    // El menú ya debería estar abierto en este punto
                    return;
                }

                if (!inGameMenu.IsOpen)
                {
                    // Pausar
                    _gameRunning = false;
                    Time.timeScale = 0f;
                    inGameMenu.ShowPauseMenu();
                }
                else
                {
                    // Reanudar
                    inGameMenu.OnResumePressed(); // esto apaga panel y pone timescale=1
                    _gameRunning = true;
                }
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
