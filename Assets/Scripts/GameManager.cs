using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{
    // Indicates whether the game has started or not
    public static bool gameStarted = false;

    // Singleton instance of the GameManager
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // Ensures only one instance of GameManager exists in the scene
        if (Instance == null)
        {
            Instance = this;

            // Prevents this GameObject from being destroyed when loading new scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy duplicate GameManager instances if one already exists
            Destroy(gameObject);
        }
    }

    // Starts the game if it has not already begun
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

    // Resets the game state, allowing it to be restarted
    public static void ResetGame()
    {
        gameStarted = false;
        Debug.Log("Game reset.");
    }

    // Resets the AR session to clear tracking data and refresh AR functionality
    public static void ResetARSession()
    {
        // Finds an active ARSession component in the scene
        ARSession arSession = FindObjectOfType<ARSession>();

        // If an ARSession is found, reset it
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
