using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScripts
{
    public class InGameMenuController : MonoBehaviour
    {
        [Header("UI")] [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Scenes")] [SerializeField] private string mainMenuSceneName = "MainMenu";

        private bool _isEndMenu;

        public bool IsOpen => panel != null && panel.activeSelf;

        private void Awake()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        private void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ShowPauseMenu()
        {
            _isEndMenu = false;

            if (titleText)
                titleText.text = "Pausa";

            if (panel)
                panel.SetActive(true);

            UnlockCursor();
        }

        public void ShowEndMenu(bool won, bool timeOut = false)
        {
            _isEndMenu = true;

            if (won)
                titleText.text = "¡Ganaste!";
            else if (timeOut)
                titleText.text = "Tiempo agotado";
            else
                titleText.text = "Game Over";

            if (panel)
                panel.SetActive(true);

            UnlockCursor();
        }

        public void OnResumePressed()
        {
            if (_isEndMenu) return; // no reanudar si la partida terminó

            Time.timeScale = 1f;

            if (panel)
                panel.SetActive(false);

            LockCursor();
        }

        public void OnMainMenuPressed()
        {
            Time.timeScale = 1f;
            UnlockCursor();
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}