using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Canvas gameCanvas;
        [SerializeField] private BooleanVariable shouldCloseUI;

        private const string UISceneName = "UI";

        private bool isUILoaded;

        private void Start()
        {
            shouldCloseUI.OnValueChanged += SwitchToGameScene;
        }
        
        private void Update()
        {
            if (!HasInput())
            {
                return;
            }

            SwitchToUIScene();
        }
        
        private static bool HasInput()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        private void SwitchToUIScene()
        {
            if (isUILoaded)
            {
                return;
            }
            
            isUILoaded = true;
            UnlockCursor();
            DisableCurrentCanvas();
            LoadUIScene();
        }

        private void SwitchToGameScene()
        {
            if (!shouldCloseUI.Value)
            {
                return;
            }
            
            LockCursor();
            UnloadUIScene();
            EnableCurrentCanvas();
            isUILoaded = false;
            shouldCloseUI.Value = false;
        }

        private static void LoadUIScene()
        {
            SceneManager.LoadScene(UISceneName, LoadSceneMode.Additive);
        }

        private void UnloadUIScene()
        {
            SceneManager.UnloadSceneAsync(UISceneName);
        }
        
        private void DisableCurrentCanvas()
        {
            gameCanvas.gameObject.SetActive(false);
        }

        private void EnableCurrentCanvas()
        {
            gameCanvas.gameObject.SetActive(true);
        }
        
        private static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        private static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}