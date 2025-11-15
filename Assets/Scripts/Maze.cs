using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Maze : MonoBehaviour
{
    private enum MazeState
    {
        None,
        Generating,
        Preview,
        Playing
    }

    [Header("View")]
    public Transform cameraTransform;        // Cámara top-down
    public GameObject characterPrefab;       // Prefab del jugador (con su cámara)
    public TextMeshProUGUI countdownText;    // Texto del contador
    public float previewDuration = 5f;       // Segundos de vista desde arriba

    [Header("Prefabs")]
    public GameObject cellPrefab;

    [Header("Size")]
    public int width = 10;
    public int height = 10;

    [Header("Cell")]
    public float cellSize = 4f;              // Cada celda mide cellSize x cellSize

    private Cell[,] _cellGrid;
    private Stack<Cell> _stack;
    private MazeState _state = MazeState.None;

    private struct NeighborInfo
    {
        public Cell cell;
        public WallOrientation fromCurrent;
        public WallOrientation fromNeighbor;

        public NeighborInfo(Cell cell, WallOrientation fromCurrent, WallOrientation fromNeighbor)
        {
            this.cell = cell;
            this.fromCurrent = fromCurrent;
            this.fromNeighbor = fromNeighbor;
        }
    }

    private void Start()
    {
        _cellGrid = new Cell[width, height];
        _stack = new Stack<Cell>();

        SetupCameraTopDown();
        HideCountdown();

        StartCoroutine(InstantiateCells());
    }

    private void Update()
    {
        switch (_state)
        {
            case MazeState.Generating:
                StepGeneration();
                break;

            case MazeState.Preview:
                // La lógica de preview está en la corrutina PreviewAndSpawn
                break;

            case MazeState.Playing:
                // Aquí podrías manejar lógica adicional cuando el jugador ya está activo
                break;
        }
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

    private void HideCountdown()
    {
        if (countdownText)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    private IEnumerator InstantiateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0f, y * cellSize);
                GameObject cellGO = Instantiate(cellPrefab, pos, Quaternion.identity, transform);

                Cell cell = cellGO.GetComponent<Cell>();
                _cellGrid[x, y] = cell;
            }
        }

        // Celda inicial en la esquina (0,0)
        Cell start = _cellGrid[0, 0];
        start.isVisited = true;
        _stack.Push(start);

        _state = MazeState.Generating;
        yield return null;
    }

    private void StepGeneration()
    {
        if (_stack.Count == 0)
        {
            OpenEntranceAndExit();
            _state = MazeState.Preview;
            StartCoroutine(PreviewAndSpawn());
            return;
        }

        Cell current = _stack.Peek();
        List<NeighborInfo> neighbors = GetUnvisitedNeighbors(current);

        if (neighbors.Count > 0)
        {
            NeighborInfo chosen = neighbors[Random.Range(0, neighbors.Count)];

            current.HideWall(chosen.fromCurrent);
            chosen.cell.HideWall(chosen.fromNeighbor);

            chosen.cell.isVisited = true;
            _stack.Push(chosen.cell);
        }
        else
        {
            _stack.Pop();
        }
    }

    private List<NeighborInfo> GetUnvisitedNeighbors(Cell current)
    {
        var result = new List<NeighborInfo>();

        Vector3 pos = current.transform.position;
        int x = Mathf.RoundToInt(pos.x / cellSize);
        int y = Mathf.RoundToInt(pos.z / cellSize);

        // Oeste
        if (x > 0)
        {
            Cell next = _cellGrid[x - 1, y];
            if (!next.isVisited)
            {
                result.Add(new NeighborInfo(
                    next,
                    WallOrientation.WEST,
                    WallOrientation.EAST
                ));
            }
        }

        // Norte
        if (y < height - 1)
        {
            Cell next = _cellGrid[x, y + 1];
            if (!next.isVisited)
            {
                result.Add(new NeighborInfo(
                    next,
                    WallOrientation.NORTH,
                    WallOrientation.SOUTH
                ));
            }
        }

        // Este
        if (x < width - 1)
        {
            Cell next = _cellGrid[x + 1, y];
            if (!next.isVisited)
            {
                result.Add(new NeighborInfo(
                    next,
                    WallOrientation.EAST,
                    WallOrientation.WEST
                ));
            }
        }

        // Sur
        if (y > 0)
        {
            Cell next = _cellGrid[x, y - 1];
            if (!next.isVisited)
            {
                result.Add(new NeighborInfo(
                    next,
                    WallOrientation.SOUTH,
                    WallOrientation.NORTH
                ));
            }
        }

        return result;
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
        if (countdownText)
        {
            countdownText.gameObject.SetActive(true);
        }

        float remaining = previewDuration;

        while (remaining > 0f)
        {
            if (countdownText)
            {
                countdownText.text = Mathf.CeilToInt(remaining).ToString();
            }

            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        HideCountdown();
        SpawnPlayer();

        _state = MazeState.Playing;
    }

    private void SpawnPlayer()
    {
        if (cameraTransform)
        {
            Destroy(cameraTransform.gameObject);
        }

        float x = 0 * cellSize;
        float z = 0 * cellSize;
        Vector3 position = new Vector3(x, 1.5f, z);

        if (characterPrefab)
        {
            Instantiate(characterPrefab, position, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        if (_stack == null) return;

        Gizmos.color = Color.red;
        foreach (var cell in _stack)
        {
            if (cell != null)
            {
                Gizmos.DrawSphere(cell.transform.position, 0.25f);
            }
        }
    }
}
