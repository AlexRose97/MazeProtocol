using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScripts
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameSceneName = "MazeLevel";

        public void PlayEasy()
        {
            GameDifficulty.SetDifficulty(DifficultyLevel.Easy);
            SceneManager.LoadScene(gameSceneName);
        }

        public void PlayMedium()
        {
            GameDifficulty.SetDifficulty(DifficultyLevel.Medium);
            SceneManager.LoadScene(gameSceneName);
        }

        public void PlayHard()
        {
            GameDifficulty.SetDifficulty(DifficultyLevel.Hard);
            SceneManager.LoadScene(gameSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}