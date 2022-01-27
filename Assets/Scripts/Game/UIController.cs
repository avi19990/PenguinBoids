using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Button backToGameButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private BooleanVariable shouldCloseScene;

        private void Start()
        {
            backToGameButton.onClick.AddListener(UnloadScene);
            quitButton.onClick.AddListener(QuitGame);
        }

        private void OnDestroy()
        {
            backToGameButton.onClick.RemoveListener(UnloadScene);
            quitButton.onClick.RemoveListener(QuitGame);
        }

        private void QuitGame()
        {
            Application.Quit();
        }

        private void UnloadScene()
        {
            shouldCloseScene.Value = true;
        }
    }
}