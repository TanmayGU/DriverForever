using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class GameOverManager : MonoBehaviour
{
    public string arSceneName = "Test Scene Rasti";
    private bool isRestarting = false;

    public void TryAgain()
    {
        if (isRestarting) return;

        Debug.Log("TryAgain() called! Restarting game...");
        isRestarting = true;
        StartCoroutine(FullResetAndRestart());
    }

    private IEnumerator FullResetAndRestart()
    {
        Debug.Log("Starting full reset and restart...");
        GameManager.ResetGame();

        yield return new WaitForSeconds(1f);

        Debug.Log("Loading scene: " + arSceneName);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(arSceneName);

        if (asyncLoad == null)
        {
            Debug.LogError("ERROR: SceneManager.LoadSceneAsync() returned null!");
            isRestarting = false;
            yield break;
        }

        yield return new WaitUntil(() => asyncLoad.isDone);
        Debug.Log("AR scene loaded. Waiting before rescanning...");

        yield return new WaitForSeconds(3f);

        Debug.Log("Looking for ARCarPlacement after scene reload...");
        ARCarPlacement arCarPlacement = null;
        while (arCarPlacement == null)
        {
            arCarPlacement = FindObjectOfType<ARCarPlacement>();
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("ARCarPlacement found. Calling ResetPlacement...");
        arCarPlacement.StartCoroutine(arCarPlacement.ResetARSessionCompletely());

        isRestarting = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game pressed! Exiting...");
        Application.Quit(); 

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // ✅ Stop play mode in Unity Editor
#endif
    }
}
