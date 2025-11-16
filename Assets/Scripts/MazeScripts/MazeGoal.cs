using UnityEngine;

namespace MazeScripts
{
    public class MazeGoal : MonoBehaviour
    {
        [SerializeField] private Maze maze;

        private void Awake()
        {
            // Si no se asigna por inspector, lo buscamos
            if (!maze)
            {
                maze = FindObjectOfType<Maze>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (maze)
            {
                maze.OnGoalReached();
            }
        }
    }
}