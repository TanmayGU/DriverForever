using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class GameOverManager : MonoBehaviour
{
    // The name of the AR scene to load when restarting
    public string arSceneName = "Test Scene Rasti";

    // Flag to prevent multiple restart attempts at the same time
    private bool isRestarting = false;

    // Public method to trigger a restart of the game
    public void TryAgain()
    {
        // Prevent multiple restarts from being triggered simultaneously
        if (isRestarting) return;

        Debug.Log("TryAgain() called! Restarting game...");
        isRestarting = true;

        // Start the restart process using a coroutine
        StartCoroutine(FullResetAndRestart());
    }

    // Coroutine to handle full reset and restart of the game
    private IEnumerator FullResetAndRestart()
    {
        Debug.Log("Starting full reset and restart...");

        // Reset the game state using the GameManager
        GameManager.ResetGame();

        // Wait for a short delay before proceeding
        yield return new WaitForSeconds(1f);

        Debug.Log("Loading scene: " + arSceneName);

        // Load the specified AR scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(arSceneName);

        // If scene loading fails, log an error and stop the restart process
        if (asyncLoad == null)
        {
            Debug.LogError("ERROR: SceneManager.LoadSceneAsync() returned null!");
            isRestarting = false;
            yield break;
        }

        // Wait until the scene has finished loading
        yield return new WaitUntil(() => asyncLoad.isDone);
        Debug.Log("AR scene loaded. Waiting before rescanning...");

        // Wait a few seconds before looking for the AR placement component
        yield return new WaitForSeconds(3f);

        Debug.Log("Looking for ARCarPlacement after scene reload...");
        ARCarPlacement arCarPlacement = null;

        // Continuously check for the ARCarPlacement component until it is found
        while (arCarPlacement == null)
        {
            arCarPlacement = FindObjectOfType<ARCarPlacement>();
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("ARCarPlacement found. Calling ResetPlacement...");

        // Reset the AR session completely to ensure a fresh start
        arCarPlacement.StartCoroutine(arCarPlacement.ResetARSessionCompletely());

        // Allow future restart attempts
        isRestarting = false;
    }

    // Method to quit the game application
    public void QuitGame()
    {
        Debug.Log("Quit Game pressed! Exiting...");
        Application.Quit();

        // If running in the Unity Editor, stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
