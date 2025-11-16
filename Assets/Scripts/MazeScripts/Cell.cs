using UnityEngine;

namespace MazeScripts
{
    public class Cell : MonoBehaviour
    {
        // Orden: 0 = WEST, 1 = NORTH, 2 = EAST, 3 = SOUTH
        public GameObject[] walls;

        [HideInInspector]
        public bool isVisited = false;

        private bool[] _wallsActive = { true, true, true, true };

        public void HideWall(WallOrientation orientation)
        {
            int index = (int)orientation;

            if (walls[index] != null)
            {
                Destroy(walls[index]);
                walls[index] = null;
                _wallsActive[index] = false;
            }
        }

        public bool IsWallActive(WallOrientation orientation)
        {
            int index = (int)orientation;
            return _wallsActive[index];
        }
    }
}