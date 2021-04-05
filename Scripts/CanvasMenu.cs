using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceX
{
    public class CanvasMenu : MonoBehaviour
    {
        public Button SimulationButton;
        public Button SpaceXButton;
        public Button UserProfileButton;

        public static CanvasMenu SINGLETON;

        private void Awake()
        {
            if (SINGLETON != null) Destroy(gameObject);
            else SINGLETON = this;
        }

        void Start()
        {
            SimulationButton.onClick.AddListener(() => LoadScene(2));
            SpaceXButton.onClick.AddListener(() => LoadScene(1));
            //Every time menu scene starts add a listener to load the user profile method to the menu user image
            //UserProfileButton.onClick.AddListener(() => UserData.SINGLETON.LoadUserProfile());
        }

        void LoadScene(int sceneIndex)
        {
            CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(true);
            CanvasAllScenes.SINGLETON.MainPanel.enabled = sceneIndex == 2 ? false : true;
            LoadSceneManager.LoadScene(sceneIndex);
        }
    }
}
