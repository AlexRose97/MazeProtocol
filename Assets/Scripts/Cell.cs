using UnityEngine;

public enum WallOrientation
{
    WEST = 0,
    NORTH = 1,
    EAST = 2,
    SOUTH = 3
}

public class Cell : MonoBehaviour
{
    // Orden: 0 = WEST, 1 = NORTH, 2 = EAST, 3 = SOUTH
    public GameObject[] walls;

    [HideInInspector]
    public bool isVisited = false;

    private bool[] wallsActive = { true, true, true, true };

    public void HideWall(WallOrientation orientation)
    {
        int index = (int)orientation;

        if (walls[index] != null)
        {
            Destroy(walls[index]);
            walls[index] = null;
            wallsActive[index] = false;
        }
    }

    public bool IsWallActive(WallOrientation orientation)
    {
        int index = (int)orientation;
        return wallsActive[index];
    }
}
