using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    bool alwaysDisplayMouse = false;
    private void Awake()
    {
        // Ensure there is only one instance of the LevelManager
        if (instance == null)
        {
            instance = this;
            // Persist across scenes
            DontDestroyOnLoad(gameObject); 

        }
        else
        {
            Destroy(gameObject);
        }

        if (!alwaysDisplayMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Method to load the next level
    public void LoadNextLevel()
    {
        // Get the current active scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the next scene
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadCurrentLevel()
    {
        // Get the current active scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the next scene
        SceneManager.LoadScene(currentSceneIndex);
    }
}
