using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{
    public static bool gameStarted = false;
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void StartGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            Debug.Log("Game started.");
        }
        else
        {
            Debug.LogWarning("Game already started.");
        }
    }

    public static void ResetGame()
    {
        gameStarted = false;
        Debug.Log("Game reset.");
    }

    public static void ResetARSession()
    {
        ARSession arSession = FindObjectOfType<ARSession>();
        if (arSession != null)
        {
            Debug.Log("Resetting ARSession...");
            arSession.Reset();
        }
        else
        {
            Debug.LogWarning("ARSession not found. Cannot reset.");
        }
    }

}
