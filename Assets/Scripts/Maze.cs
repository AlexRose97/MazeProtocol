using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.AI.Navigation; 

public class Maze : MonoBehaviour
{
    [Header("View")]
    public Transform cameraTransform;         // Cámara top-down
    public GameObject characterPrefab;        // Prefab del jugador (con su cámara)
    public TextMeshProUGUI countdownText;     // Texto para mensajes y contador
    public float previewDuration = 5f;        // Segundos de vista desde arriba

    [Header("Prefabs")]
    public GameObject cellPrefab;

    [Header("Size")]
    public int width = 10;
    public int height = 10;

    [Header("Cell")]
    public float cellSize = 4f;               // Cada celda mide cellSize x cellSize
    public float spawnStepDelay = 0.02f;      // Tiempo entre instancias de celdas

    [Header("Random")]
    public int randomSeed = 0;                // Si es 0, se usa un seed aleatorio
    
    [Header("Navigation")]
    public NavMeshSurface navMeshSurface;
    

    [Header("Enemies")]
    public GameObject enemyPrefab;
    [Range(0f, 1f)]
    public float enemyDensity = 0.25f; // 0.25 = 1 enemigo cada 4 celdas
    
    private Cell[,] cellGrid;

    private IEnumerator Start()
    {
        cellGrid = new Cell[width, height];

        SetupCameraTopDown();
        ShowMessage("Generando...");

        int? seedToUse = randomSeed == 0 ? (int?)null : randomSeed;
        var generator = new MazeGenerator(width, height, seedToUse);
        generator.Generate(); // Lógica pura, rápida, sin visual

        yield return StartCoroutine(SpawnCellsFromData(generator));
        
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

    private void ShowMessage(string message)
    {
        if (countdownText == null) return;

        countdownText.gameObject.SetActive(true);
        countdownText.text = message;
    }

    private void HideText()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
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
                cellGrid[x, y] = cell;

                MazeCellData data = generator.Cells[x, y];

                // Abrir paredes según los datos generados
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
        Cell start = cellGrid[0, 0];
        Cell end = cellGrid[width - 1, height - 1];

        start.HideWall(WallOrientation.WEST);
        end.HideWall(WallOrientation.EAST);
    }

    private IEnumerator PreviewAndSpawn()
    {
        float remaining = previewDuration;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        while (remaining > 0f)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.CeilToInt(remaining).ToString();
            }

            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        HideText();
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (cameraTransform != null)
        {
            Destroy(cameraTransform.gameObject);
        }

        float x = 0 * cellSize;
        float z = 0 * cellSize;
        Vector3 position = new Vector3(x, 1.5f, z);

        GameObject player = null;

        if (characterPrefab != null)
        {
            player = Instantiate(characterPrefab, position, Quaternion.identity);
        }

        if (player != null)
        {
            var chasers = FindObjectsOfType<SimpleChaser>();
            foreach (var c in chasers)
            {
                c.SetTarget(player.transform);
            }
        }
    }
    private void SpawnEnemies()
    {
        if (enemyPrefab == null) return;

        int totalCells = width * height;
        int enemyCount = Mathf.FloorToInt(totalCells * enemyDensity);

        if (enemyCount <= 0) return;

        var usedPositions = new HashSet<Vector2Int>();
        var rng = new System.Random(randomSeed == 0 ? System.Environment.TickCount : randomSeed);

        // Evitar esquina inicio y esquina final
        usedPositions.Add(new Vector2Int(0, 0));
        usedPositions.Add(new Vector2Int(width - 1, height - 1));

        int maxAttempts = totalCells * 3;
        int spawned = 0;
        int attempts = 0;

        while (spawned < enemyCount && attempts < maxAttempts)
        {
            attempts++;

            int x = rng.Next(0, width);
            int y = rng.Next(0, height);

            var cellPos = new Vector2Int(x, y);
            if (usedPositions.Contains(cellPos)) continue;

            Cell cell = cellGrid[x, y];
            if (cell == null) continue;

            Vector3 pos = cell.transform.position + new Vector3(0f, 1.5f, 0f);
            Instantiate(enemyPrefab, pos, Quaternion.identity);

            usedPositions.Add(cellPos);
            spawned++;
        }
    }

    private void OnDrawGizmos()
    {
        if (cellGrid == null) return;

        Gizmos.color = Color.red;

        foreach (var cell in cellGrid)
        {
            if (cell != null)
            {
                Gizmos.DrawSphere(cell.transform.position, 0.1f);
            }
        }
    }
}
