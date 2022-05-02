using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {
    public static SceneSwitcher instance;

    public static int currentScene, lastScene;

    void Awake() {
        if (instance == null)
            instance = this;
    }

    public void A_ExitButton() {
        Application.Quit();
    }

    public void A_LoadScene(int i) {
        lastScene = currentScene;
        currentScene = i;
        SceneManager.LoadScene(i);
        Cursor.lockState = CursorLockMode.None;
    }

    public void A_LoadNextScene(float timeDelay) {
        lastScene = currentScene;
        currentScene++;
        SceneManager.LoadScene(currentScene);
        Cursor.lockState = CursorLockMode.None;
    }

    public void A_LoadCurrentScene() {
        SceneManager.LoadScene(currentScene);
    }
    public void A_LoadLastScene() {
        SceneManager.LoadScene(lastScene);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && currentScene == 0)
            A_ExitButton();
        if (Input.GetKeyDown(KeyCode.Escape))
            A_LoadScene(0);
    }
}
