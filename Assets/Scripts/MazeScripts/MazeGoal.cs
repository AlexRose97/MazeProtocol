using UnityEngine;

namespace MazeScripts
{
    public class MazeGoal : MonoBehaviour
    {
        [SerializeField] private Maze maze;

        private void Awake()
        {
            // Si no se asigna por inspector, lo buscamos
            if (maze == null)
            {
                maze = FindObjectOfType<Maze>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (maze != null)
            {
                maze.OnGoalReached();
            }
        }
    }
}